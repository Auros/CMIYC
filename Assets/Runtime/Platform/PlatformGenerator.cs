using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Random = System.Random;

namespace CMIYC.Platform
{
    public partial class PlatformGenerator : MonoBehaviour
    {
        public const int daughterboardUnit = 10;

        [SerializeField]
        private Vector2Int _motherboardSize = new(5, 5);

        [SerializeField]
        private RoomSpawnOption[] _roomSpawnOptions = Array.Empty<RoomSpawnOption>();

        [Range(0, 1)]
        [SerializeField]
        private float _targetMotherboardVolume = 0.2f;

        [SerializeField]
        private int _sampleAttempts = 5;

        [SerializeField]
        private int _seed;

        private Random _random = new(0);
        private readonly ObjectPool<RoomInstance> _roomInstancePool = new(() => new RoomInstance());

        private IObjectPool<Motherboard> _motherboardPool = null!;

        private Vector2 MotherboardSize => _motherboardSize;

        private void Awake()
        {
            _motherboardPool = new ObjectPool<Motherboard>(() => new GameObject("Motherboard").AddComponent<Motherboard>());
        }

        private void Start()
        {
            _random = new Random(_seed);
            BuildMotherboard(Cardinal.West);
        }

        private Random GetRandom() => _random;

        private void BuildMotherboard(Cardinal from, Vector2Int? start = null)
        {
            var random = GetRandom();
            var maxDaughters = _motherboardSize.x * _motherboardSize.y;
            var targetArea = Mathf.FloorToInt(_targetMotherboardVolume * maxDaughters);

            int currentArea = 0;
            var validWeightedRooms = GetWeightedRoomOptions();

            // There are no valid rooms... womp womp
            if (validWeightedRooms.Length is 0)
                return;

            var roomPrefabs = ListPool<RoomDefinition>.Get();

            while (targetArea > currentArea)
            {
                //Debug.Log($"Target Area: {targetArea}, Current Area: {currentArea}");
                var index = random.Next(0, validWeightedRooms.Length);
                var roomPrefab = validWeightedRooms[index];

                // Add the area of this room to the current collected
                var roomArea = roomPrefab.Size.x * roomPrefab.Size.y;
                currentArea += roomArea;

                roomPrefabs.Add(roomPrefab);
            }

            //Debug.Log($"Selected Prefab Count: {roomPrefabs.Count}");

            var roomInstances = ListPool<RoomInstance>.Get();

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
                    if (0 > (int)rect.xMin || 0 > (int)rect.yMin || (int)rect.xMax >= _motherboardSize.x || (int)rect.yMax >= _motherboardSize.y)
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

                    var instance = _roomInstancePool.Get();
                    instance.Cardinal = cardinal;
                    instance.Prefab = roomPrefab;
                    instance.Position = pos;
                    instance.Rect = rect;

                    roomInstances.Add(instance);
                    break;
                }
            }

            var motherboard = _motherboardPool.Get();
            var motherboardTransform = motherboard.transform;
            motherboardTransform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

            Dictionary<Vector2Int, Definition> definitionLookup = new();

            foreach (var inst in roomInstances)
            {
                //Debug.Log($"{inst.Prefab.name}: {inst.Rect}, {inst.Cardinal}");
                var def = inst.Definition = Instantiate(inst.Prefab);
                var defTransform = def.transform;
                defTransform.SetParent(motherboardTransform);
                def.SetData(inst.Cardinal, inst.Position);

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

                //Debug.Log("Transformed To World: " + def.TransformLocation(new Vector2Int(1, 1)));
                //Debug.Log("Transformed To Local: " + def.InverseTransformLocation(new Vector2Int(0, 0)));
                for (int x = (int)inst.Rect.xMin; x < (int)inst.Rect.xMax; x++)
                {
                    for (int y = (int)inst.Rect.yMin; y < (int)inst.Rect.yMax; y++)
                    {
                        definitionLookup.Add(new Vector2Int(x, y), def);
                    }
                }
            }

            if (!start.HasValue)
            {
                var val = from switch
                {
                    Cardinal.North => _motherboardSize.x,
                    Cardinal.South => _motherboardSize.x,
                    Cardinal.East => _motherboardSize.y,
                    Cardinal.West => _motherboardSize.y,
                    _ => throw new ArgumentOutOfRangeException(nameof(from), from, null)
                };

                // TODO: REGENERATE THE ENTIRE MAP IF NO ENTRANCE IS GENERATED

                var list = ListPool<Vector2Int>.Get();
                if (from is Cardinal.North or Cardinal.South)
                {
                    var y = from is Cardinal.North ? _motherboardSize.y - 1 : 0;
                    for (int x = 0; x < val; x++)
                    {
                        Vector2Int cell = new(x, y);
                        list.Add(cell);

                        if (!definitionLookup.TryGetValue(cell, out var definition) || definition is not RoomDefinition roomDef)
                            continue;

                        if (roomDef.GetWallSegmentType(from, cell) is not WallSegmentType.Door)
                            continue;

                        start = cell;
                        break;
                    }
                }
                else
                {
                    var x = from is Cardinal.East ? _motherboardSize.x - 1 : 0;
                    for (int y = 0; y < val; y++)
                    {
                        Vector2Int cell = new(x, y);
                        list.Add(cell);

                        if (!definitionLookup.TryGetValue(cell, out var definition) || definition is not RoomDefinition roomDef)
                            continue;

                        if (roomDef.GetWallSegmentType(from, cell) is not WallSegmentType.Door)
                            continue;

                        start = cell;
                        break;
                    }
                }

                start ??= list[random.Next(0, list.Count)];
                ListPool<Vector2Int>.Release(list);
            }

            Debug.Log(start);

            ListPool<RoomDefinition>.Release(roomPrefabs);
            ListPool<RoomInstance>.Release(roomInstances);
        }

        private void OnDrawGizmos()
        {
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

        [Serializable]
        protected struct RoomSpawnOption
        {
            [field: Min(0), SerializeField]
            public float Weight { get; private set; }

            [field: SerializeField]
            public RoomDefinition Prefab { get; private set; }
        }

        protected class RoomInstance
        {
            public Rect Rect { get; set; }

            public Vector2Int Position { get; set; }

            public Cardinal Cardinal { get; set; }

            public RoomDefinition Prefab { get; set; } = null!;

            public RoomDefinition Definition { get; set; } = null!;
        }
    }
}
