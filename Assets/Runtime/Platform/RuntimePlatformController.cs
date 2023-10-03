using System;
using System.Collections.Generic;
using System.Linq;
using CMIYC.Enemy;
using UnityEngine;

namespace CMIYC.Platform
{
    public class RuntimePlatformController : MonoBehaviour
    {
        [SerializeField]
        private PlatformGenerator _platformGenerator = null!;

        [SerializeField]
        private EnemyController _enemyController = null!;

        [SerializeField]
        private GameObject _roof = null!;

        [SerializeField]
        private Color _defaultRoomColor;

        private void OnEnable()
        {
            _platformGenerator.OnMotherboardEntered += PlatformGenerator_OnMotherboardEntered;
        }

        private void Start()
        {
            _roof.SetActive(true);
            _platformGenerator.Advance();
            Setup(_platformGenerator.Current!);
            Setup(_platformGenerator.Next!);
        }

        private void PlatformGenerator_OnMotherboardEntered(Motherboard motherboard)
        {
            if (motherboard != _platformGenerator.Next?.Motherboard)
                return;

            _platformGenerator.Advance();

            if (_platformGenerator.Next is null)
                return;

            Setup(_platformGenerator.Next);
        }

        private void Setup(PlatformGenerator.MotherboardGenerationResult result)
        {
            Color.RGBToHSV(_defaultRoomColor, out float h, out float s, out float v);
            h = UnityEngine.Random.Range(0, 1f);

            var roomColor = Color.HSVToRGB(h, s, v);
            var rooms = result.Rooms.Select(r => r.Definition).ToArray();
            SpawnEnemies(rooms);

            foreach (var room in rooms)
            {
                room.SetColor(roomColor);
            }

            foreach (var hall in result.Hallways)
            {
                hall.Definition.SetColor(roomColor);
            }
        }

        private void OnDisable()
        {
            _platformGenerator.OnMotherboardEntered += PlatformGenerator_OnMotherboardEntered;
        }

        private void SpawnEnemies(IEnumerable<RoomDefinition> rooms)
        {
            foreach (var room in rooms)
            {
                foreach (var spawnData in room.SpawnDatas)
                {
                    _enemyController.Spawn(spawnData);
                }
            }
        }


        // a bit gross
        /*private Motherboard? _activeMotherboard;
        private List<PlatformGenerator.MotherboardGenerationResult> _setupMotherboards = new();
        private Dictionary<PlatformGenerator.MotherboardGenerationResult, Color> _colorByMotherboard = new();
        private List<GameObject> _existingFakeWalls = new();

        public void Start()
        {
            _roof?.SetActive(true);

            Regenerate();
        }

        public void OnMotherboardEntered(Motherboard motherboard)
        {
            if (motherboard != _setupMotherboards.Last().Motherboard) return;

            // close off CURRENT beginning before switching
            var current = _platformGenerator.Current;
            if (current != null && _colorByMotherboard.TryGetValue(current!, out Color roomColor))
            {
                Debug.Log("new fake wall just dropped");
                MakeFakeWall(current, roomColor);
            }
            // only 3 active at a time, close off the last one
            Debug.Log("CHANGING!!!!!!!!!!!!!!!");
            Regenerate();
        }

        private void MakeFakeWall(PlatformGenerator.MotherboardGenerationResult result, Color roomColor)
        {
            foreach (var existingWall in _existingFakeWalls)
            {
                Destroy(existingWall);
            }
            _existingFakeWalls.Clear();

            // create cube or some shit idk
            var blockOffCube = Instantiate(_closeDoor);

            var additionVector = result.Origin switch
            {
                Cardinal.North => new Vector2(0, 4.5f),
                Cardinal.East => new Vector2(4.5f, 0f),
                Cardinal.South => new Vector2(0f, -4.5f),
                Cardinal.West => new Vector2(-4.5f, 0f),
            };

            blockOffCube.transform.position = result.Entrance.transform.position + new Vector3(additionVector.x, 0, additionVector.y);
            if (result.Origin == Cardinal.East || result.Origin == Cardinal.West)
            {
                blockOffCube.transform.Rotate(new Vector3(0, 90f, 0));
            }

            foreach (var blockRenderer in blockOffCube.GetComponentsInChildren<Renderer>())
            {
                blockRenderer.material.color = roomColor;
            }

            _existingFakeWalls.Add(blockOffCube);
        }

        public void OnMotherboardReleased(Motherboard motherboard)
        {
            motherboard.OnEntered -= OnMotherboardEntered;
            motherboard.OnReleased -= OnMotherboardReleased;
        }

        private void Regenerate()
        {
            _platformGenerator.Advance();

            // figure out how this gets updated, and when  I need to do stuff
            var result = _platformGenerator.Current;
            var next = _platformGenerator.Next;

            Setup(result, true);
            Setup(next);

            _activeMotherboard = result?.Motherboard;
        }

        private void Setup(PlatformGenerator.MotherboardGenerationResult? result, bool isCurrent = false)
        {
            if (result == null || _setupMotherboards.Contains(result)) return;

            // create room color for thing
            Color.RGBToHSV(_defaultRoomColor, out float H, out float S, out float V);
            H = Random.Range(0, 1f);

            var roomColor = Color.HSVToRGB(H, S, V);

            _colorByMotherboard.TryAdd(result, roomColor);
            var halls = result.Hallways.Select(x => x.Definition);
            var rooms = result.Rooms.Select(x => x.Definition);

            // temp wall impl incase auros doesn't have time

            SpawnEnemies(rooms);

            foreach (var room in rooms)
            {
                room.SetColor(roomColor);
            }

            foreach (var hall in halls)
            {
                hall.SetColor(roomColor);
            }

            if (isCurrent)
            {
                MakeFakeWall(result, roomColor);
            }
            _setupMotherboards.Add(result);
            result.Motherboard.OnEntered += OnMotherboardEntered;
            result.Motherboard.OnReleased += OnMotherboardReleased;
        }

        private void SpawnEnemies(IEnumerable<RoomDefinition> rooms)
        {
            foreach (var room in rooms)
            {
                foreach (var spawnData in room.SpawnDatas)
                {
                    _enemyController.Spawn(spawnData);
                    // ar cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    // cube.transform.position = spawnDefinition.transform.position;
                    // _enemyController.Spawn(spawnDefinition);
                }
            }
        }

        private bool HallExists(Vector2Int location, Cardinal cardinal, List<Vector2Int> takenVectors, Dictionary<Vector2Int, Cardinal> roomCardinals)
        {
            var modifiedLocation = location;
            if (cardinal == Cardinal.North) modifiedLocation += new Vector2Int(0, 1);
            if (cardinal == Cardinal.South) modifiedLocation += new Vector2Int(0, -1);
            if (cardinal == Cardinal.East) modifiedLocation += new Vector2Int(1, 0);
            if (cardinal == Cardinal.West) modifiedLocation += new Vector2Int(-1, 0);


            if (!roomCardinals.TryGetValue(modifiedLocation, out var roomCardinal))
            {
                return takenVectors.Contains(modifiedLocation);
            }
            else
            {
                return cardinal == roomCardinal;
            }

            return false;
        }*/
    }
}
