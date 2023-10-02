using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Pool;
using Random = System.Random;

namespace CMIYC.Platform
{
    [PublicAPI]
    public partial class PlatformGenerator : MonoBehaviour
    {
        public const int daughterboardUnit = 10;

        [SerializeField]
        private Transform _recyclingBin = null!;

        [SerializeField]
        private Vector2Int _motherboardSize = new(5, 5);

        [SerializeField]
        private HallDefinition _hallwayPrefab = null!;

        [SerializeField]
        private Motherboard _motherboardPrefab = null!;

        [SerializeField]
        private RoomSpawnOption[] _roomSpawnOptions = Array.Empty<RoomSpawnOption>();

        [Range(0, 1)]
        [SerializeField]
        private float _targetMotherboardVolume = 0.2f;

        [Min(0)]
        [SerializeField]
        private int _initialRoomPoolSizePerRoom = 10;

        [Min(0)]
        [SerializeField]
        private int _initialHallwayPoolSize = 100;

        [SerializeField]
        private int _sampleAttempts = 5;

        [SerializeField]
        private int _seed;

        [SerializeField]
        private bool _drawGizmos;

        [SerializeField]
        private bool _buildOnStart;

        private Random _random = new(0);
        private ObjectPool<HallDefinition> _hallPool = null!;
        private ObjectPool<Motherboard> _motherboardPool = null!;
        private Dictionary<RoomDefinition, ObjectPool<RoomDefinition>> _roomDefinitionPools = null!;

        private ObjectPool<UnexploredCell> _unexploredCellPool = new(() => new UnexploredCell());

        public MotherboardGenerationResult? Next { get; private set; }

        public MotherboardGenerationResult? Current { get; private set; }

        public MotherboardGenerationResult? Previous { get; private set; }

        private Vector2 MotherboardSize => _motherboardSize;

        private void Awake()
        {
            _random = _seed != -1 ? new Random(_seed) : new Random();

            _hallPool = new ObjectPool<HallDefinition>(
                () => Instantiate(_hallwayPrefab, transform),
                h =>
                {
                    var hT = h.transform;
                    hT.SetParent(transform);
                    hT.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                },
                h => h.transform.SetParent(_recyclingBin),
                h => Destroy(h.gameObject)
            );

            _motherboardPool = new ObjectPool<Motherboard>(
                () =>
                {
                    var mp = Instantiate(_motherboardPrefab, transform);
                    mp.SetDimensionsSize(_motherboardSize);
                    return mp;
                },
                m =>
                {
                    var mT = m.transform;
                    mT.SetParent(transform);
                    mT.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                },
                m => m.transform.SetParent(_recyclingBin),
                m => Destroy(m.gameObject)
            );

            _roomDefinitionPools = new Dictionary<RoomDefinition, ObjectPool<RoomDefinition>>(_roomSpawnOptions.Length);

            foreach (var option in _roomSpawnOptions)
            {
                var prefab = option.Prefab;
                if (_roomDefinitionPools.TryGetValue(prefab, out var pool))
                    continue;

                pool = new ObjectPool<RoomDefinition>(
                    () => Instantiate(prefab, transform),
                    r =>
                    {
                        var rT = r.transform;
                        rT.SetParent(transform);
                        rT.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                    },
                    r => r.transform.SetParent(_recyclingBin),
                    r => Destroy(r.gameObject)
                );

                using (ListPool<RoomDefinition>.Get(out var prespawned))
                {
                    for (int i = 0; i < _initialRoomPoolSizePerRoom; i++)
                        prespawned.Add(pool.Get());

                    prespawned.ForEach(pool.Release);
                }

                _roomDefinitionPools[prefab] = pool;
            }

            using (ListPool<HallDefinition>.Get(out var prespawned))
            {
                for (int i = 0; i < _initialHallwayPoolSize; i++)
                    prespawned.Add(_hallPool.Get());

                prespawned.ForEach(_hallPool.Release);
            }
        }

        private void Start()
        {
            if (!_buildOnStart)
                return;

            Advance();

            /*Cysharp.Threading.Tasks.UniTask.Void(async () =>
            {
                for (int i = 0; i < 10; i++)
                {
                    await Cysharp.Threading.Tasks.UniTask.Delay(TimeSpan.FromSeconds(1f));
                    Advance();
                }
            });*/
        }

        public void Advance()
        {
            var motherboardPhysicalWidth = _motherboardSize.x * daughterboardUnit;
            var motherboardPhysicalHeight = _motherboardSize.y * daughterboardUnit;
            Next ??= BuildMotherboard(Cardinal.South, new Vector2Int(0, 0), Vector2.zero);

            if (Previous is not null)
            {
                // TODO: Despawn motherboard
                foreach (var instance in Previous.Rooms)
                    DespawnRoomDefinition(instance);

                foreach (var hall in Previous.Hallways)
                    _hallPool.Release(hall.Definition);

                Previous.Motherboard.ClearEvents();
                _motherboardPool.Release(Previous.Motherboard);
            }

            // Close off the previous motherboard
            Current?.Entrance.GetWall(Current.Origin).SetType(WallSegmentType.Wall);

            Previous = Current;
            Current = Next;

            if (Next is null || Next != Current)
                return;

            var previousDirection = Adjacant(Next.Advancement);

            var startPoint = Next.Advancement switch
            {
                Cardinal.North => new Vector2Int(Next.End.x, 0),
                Cardinal.East => new Vector2Int(0, Next.End.y),
                Cardinal.South => new Vector2Int(Next.End.x, _motherboardSize.y - 1),
                Cardinal.West => new Vector2Int(_motherboardSize.x - 1, Next.End.y),
                _ => throw new ArgumentOutOfRangeException()
            };

            var targetPos = Next.Advancement switch
            {
                Cardinal.North => new Vector2(Next.Position.x, Next.Position.y + motherboardPhysicalHeight),
                Cardinal.East => new Vector2(Next.Position.x + motherboardPhysicalWidth, Next.Position.y),
                Cardinal.South => new Vector2(Next.Position.x, Next.Position.y - motherboardPhysicalHeight),
                Cardinal.West => new Vector2(Next.Position.x - motherboardPhysicalWidth, Next.Position.y),
                _ => throw new ArgumentOutOfRangeException()
            };

            Next = BuildMotherboard(previousDirection, startPoint, targetPos);
        }

        private Random GetRandom() => _random;

        private MotherboardGenerationResult? BuildMotherboard(Cardinal from, Vector2Int start, Vector2 physicalPosition)
        {
            var random = GetRandom();
            var maxDaughters = _motherboardSize.x * _motherboardSize.y;
            var targetArea = Mathf.FloorToInt(_targetMotherboardVolume * maxDaughters);

            int currentArea = 0;
            var validWeightedRooms = GetWeightedRoomOptions();

            // There are no valid rooms... womp womp
            if (validWeightedRooms.Length is 0)
                return null;

            var roomPrefabs = ListPool<RoomDefinition>.Get();

            while (targetArea > currentArea)
            {
                var index = random.Next(0, validWeightedRooms.Length);
                var roomPrefab = validWeightedRooms[index];

                // Add the area of this room to the current collected
                var roomArea = roomPrefab.Size.x * roomPrefab.Size.y;
                currentArea += roomArea;

                roomPrefabs.Add(roomPrefab);
            }

            List<RoomInstance> roomInstances = new();
            List<HallInstance> hallInstances = new();

            // We now know what rooms we want to place, now we pick random points to place them.
            foreach (var roomPrefab in roomPrefabs)
            {
                var physicalSize = roomPrefab.Size;

                // Vary the rooms by rotating them
                var cardinal = (Cardinal)random.Next(0, 4);

                // To compensate for if the room is not square, we swap the X and Y of the size when doing bound detection if it makes a right angle.
                if (cardinal is Cardinal.East or Cardinal.West)
                    physicalSize = new Vector2Int(physicalSize.y, physicalSize.x);

                for (int i = 0; i < _sampleAttempts; i++)
                {
                    var randomX = random.Next(0, _motherboardSize.x);
                    var randomY = random.Next(0, _motherboardSize.y);
                    Vector2Int pos = new(randomX, randomY);

                    if (cardinal is Cardinal.East or Cardinal.South)
                        randomY -= physicalSize.y - 1;

                    if (cardinal is Cardinal.West or Cardinal.South)
                        randomX -= physicalSize.x - 1;

                    Rect rect = new(randomX, randomY, physicalSize.x, physicalSize.y);

                    // Check if this room was placed in bounds
                    if (0 >= (int)rect.xMin || 0 >= (int)rect.yMin || (int)rect.xMax >= _motherboardSize.x || (int)rect.yMax >= _motherboardSize.y)
                        continue;

                    // Check if this room overlaps with any previously calculated room
                    bool overlappingWithPrevious = false;
                    foreach (var previousRoom in roomInstances)
                    {
                        if (!previousRoom.Rect.Overlaps(rect, true))
                            continue;

                        overlappingWithPrevious = true;
                        break;
                    }

                    if (overlappingWithPrevious)
                        continue;

                    var instance = new RoomInstance
                    {
                        Cardinal = cardinal,
                        Prefab = roomPrefab,
                        Position = pos,
                        Rect = rect
                    };

                    roomInstances.Add(instance);
                    break;
                }
            }

            var motherboard = _motherboardPool.Get();
            var motherboardTransform = motherboard.transform;
            motherboardTransform.SetParent(transform);

            var definitionLookup = DictionaryPool<Vector2Int, Definition>.Get();

            foreach (var inst in roomInstances)
            {
                var pool = _roomDefinitionPools[inst.Prefab];
                var def = pool.Get();
                inst.Definition = def;

                var defTransform = def.transform;
                def.SetData(inst.Cardinal, inst.Position);
                defTransform.SetParent(motherboardTransform);

                defTransform.localRotation = Quaternion.Euler(0f, inst.Cardinal switch
                {
                    Cardinal.North => 0f,
                    Cardinal.East => 90f,
                    Cardinal.South => 180f,
                    Cardinal.West => 270f,
                    _ => throw new ArgumentOutOfRangeException()
                }, 0f);

                const float cellOffset = daughterboardUnit / 2f;
                var daughterCenter = new Vector3(0 * daughterboardUnit + cellOffset, 0, 0 * daughterboardUnit + cellOffset) + new Vector3(inst.Position.x * daughterboardUnit, 0f, inst.Position.y * daughterboardUnit);

                defTransform.position = daughterCenter;

                def.Anchor.SetParent(null, true);
                defTransform.SetParent(def.Anchor);
                def.Anchor.position = daughterCenter;
                defTransform.SetParent(motherboardTransform);
                def.Anchor.SetParent(defTransform, true);

                for (int x = (int)inst.Rect.xMin; x < (int)inst.Rect.xMax; x++)
                for (int y = (int)inst.Rect.yMin; y < (int)inst.Rect.yMax; y++)
                    definitionLookup.Add(new Vector2Int(x, y), def);
            }

            var (exitDir, exitTarget) = GetRandomExitNodes(random, from);
            GenerateHallway(start, exitTarget, motherboardTransform, roomInstances, hallInstances, definitionLookup);

            motherboardTransform.SetLocalPositionAndRotation(new Vector3(physicalPosition.x, 0, physicalPosition.y), Quaternion.identity);

            HallDefinition entrance = null!;
            HallDefinition exit = null!;

            foreach (var hall in hallInstances)
            {
                var hallDef = hall.Definition;
                var (x, y) = hall.Position;

                if (hall.Position == start)
                    entrance = hallDef;
                if (hall.Position == exitTarget)
                    exit = hallDef;

                var wallChecks = ListPool<WallLookup>.Get();
                wallChecks.Add(new WallLookup { Position = new Vector2Int(x, y + 1), Direction = Cardinal.North });
                wallChecks.Add(new WallLookup { Position = new Vector2Int(x + 1, y), Direction = Cardinal.East });
                wallChecks.Add(new WallLookup { Position = new Vector2Int(x, y - 1), Direction = Cardinal.South });
                wallChecks.Add(new WallLookup { Position = new Vector2Int(x - 1, y), Direction = Cardinal.West });

                foreach (var wallCheck in wallChecks)
                {
                    var pos = wallCheck.Position;
                    var cardinal = wallCheck.Direction;
                    var wall = hallDef.GetWall(cardinal);
                    if (definitionLookup.TryGetValue(pos, out var defRel))
                    {
                        if (defRel is HallDefinition)
                        {
                            wall.SetType(WallSegmentType.Door);
                            continue;
                        }

                        var isValidForDoor = defRel is RoomDefinition roomDef && roomDef.GetWallSegmentType(Adjacant(cardinal), pos) is WallSegmentType.Door;
                        wall.SetType(isValidForDoor ? WallSegmentType.Door : WallSegmentType.Wall);
                    }
                    else
                    {
                        wall.SetType(WallSegmentType.Wall);
                    }
                }

                ListPool<WallLookup>.Release(wallChecks);
            }

            ListPool<RoomDefinition>.Release(roomPrefabs);
            DictionaryPool<Vector2Int, Definition>.Release(definitionLookup);

            var entranceWall = entrance.GetWall(from);
            entranceWall.SetType(WallSegmentType.Door);

            var exitWall = exit.GetWall(exitDir);
            exitWall.SetType(WallSegmentType.Door);

            return new MotherboardGenerationResult
            {
                End = exitTarget,
                Advancement = exitDir,
                Motherboard = motherboard,
                Rooms = roomInstances,
                Hallways = hallInstances,
                Position = physicalPosition,
                Entrance = entrance,
                Origin = from
            };
        }

        private static Cardinal Adjacant(Cardinal cardinal)
        {
            return cardinal switch
            {
                Cardinal.North => Cardinal.South,
                Cardinal.East => Cardinal.West,
                Cardinal.South => Cardinal.North,
                Cardinal.West => Cardinal.East,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private void DespawnRoomDefinition(RoomInstance roomInstance)
        {
            var pool = _roomDefinitionPools[roomInstance.Prefab];
            pool.Release(roomInstance.Definition);
        }

        private (Cardinal, Vector2Int) GetRandomExitNodes(Random random, Cardinal banned)
        {
            Cardinal selected;
            Vector2Int value;
            using (ListPool<Cardinal>.Get(out var cardinals))
            {
                cardinals.Add(Cardinal.North);
                cardinals.Add(Cardinal.East);
                cardinals.Add(Cardinal.South);
                cardinals.Add(Cardinal.West);

                cardinals.Remove(banned);

                selected = cardinals[random.Next(0, cardinals.Count)];
                value = selected switch
                {
                    Cardinal.North => new Vector2Int(random.Next(0, _motherboardSize.x), _motherboardSize.y - 1),
                    Cardinal.East => new Vector2Int(_motherboardSize.x - 1, random.Next(0, _motherboardSize.x)),
                    Cardinal.South => new Vector2Int(random.Next(0, _motherboardSize.x), 0),
                    Cardinal.West => new Vector2Int(0, random.Next(0, _motherboardSize.x)),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
            return (selected, value);
        }

        private void OnDrawGizmos()
        {
            if (!_drawGizmos)
            {
                return;
            }
            const int height = 10;
            var target = transform;
            Gizmos.matrix = target.localToWorldMatrix;
            var pos = target.position;

            var motherboards = ListPool<Vector2>.Get();
            var offset = MotherboardSize * daughterboardUnit;
            var physicalMotherboardSize = MotherboardSize * daughterboardUnit;

            motherboards.Add(offset);
            motherboards.Add(offset.WithY(0));
            motherboards.Add(offset.WithX(0));
            motherboards.Add(new Vector2(0, 0));

            var offsetHalf = offset / 2f;

            foreach (var board in motherboards)
            {
                var center = board + offsetHalf;
                var (x, z) = center;

                Gizmos.color = Color.white;
                Gizmos.DrawWireCube(new Vector3(x, height / 2f, z) + pos, new Vector3(physicalMotherboardSize.x, height, physicalMotherboardSize.y));

                const float cellOffset = daughterboardUnit / 2f;
                for (int mx = 0; mx < _motherboardSize.x; mx++)
                {
                    for (int my = 0; my < _motherboardSize.y; my++)
                    {
                        var daughterCenter = new Vector3(mx * daughterboardUnit + cellOffset, height / 2f, my * daughterboardUnit + cellOffset) + new Vector3(board.x, 0, board.y);

                        Gizmos.color = Color.red.WithA(0.25f);
                        Gizmos.DrawWireCube(daughterCenter + pos, new Vector3(daughterboardUnit, height, daughterboardUnit));

                        Gizmos.color = Color.blue.WithA(0.1f);
                        Gizmos.DrawLine(daughterCenter.WithY(-height) + pos, daughterCenter.WithY(height * 2));

                        Gizmos.color = Color.green;
                        Gizmos.DrawSphere(daughterCenter.WithY(0) + pos, 0.1f);
                    }
                }
            }

            ListPool<Vector2>.Release(motherboards);
        }

        private void OnDestroy()
        {
            _hallPool.Dispose();
            _motherboardPool.Dispose();
            _unexploredCellPool.Dispose();
            foreach (var objectPool in _roomDefinitionPools.Values)
                objectPool.Dispose();
        }

        private struct WallLookup
        {
            public Cardinal Direction { get; set; }

            public Vector2Int Position { get; set; }
        }

        [Serializable]
        protected class RoomSpawnOption
        {
            [field: Min(0), SerializeField]
            public float Weight { get; private set; }

            [field: SerializeField]
            public RoomDefinition Prefab { get; private set; } = null!;
        }

        public class RoomInstance
        {
            public Rect Rect { get; set; }

            public Vector2Int Position { get; set; }

            public Cardinal Cardinal { get; set; }

            public RoomDefinition Prefab { get; set; } = null!;

            public RoomDefinition Definition { get; set; } = null!;
        }

        public class HallInstance
        {
            public Vector2Int Position { get; set; }

            public HallDefinition Definition { get; set; } = null!;
        }

        public class MotherboardGenerationResult
        {
            public Cardinal Origin { get; set; }

            public Cardinal Advancement { get; set; }

            public Vector2 Position { get; set; }

            public Vector2Int End { get; set; }

            public Motherboard Motherboard { get; set; } = null!;

            public IReadOnlyList<RoomInstance> Rooms { get; set; } = null!;

            public IReadOnlyList<HallInstance> Hallways { get; set; } = null!;

            public HallDefinition Entrance { get; set; } = null!;
        }
    }
}
