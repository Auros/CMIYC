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
        private Transform _player = null!;

        [SerializeField]
        private GameObject _closeDoor = null!;

        [SerializeField]
        private EnemyController _enemyController = null!;

        [SerializeField]
        private GameObject _roof = null!;

        // a bit gross
        private Motherboard? _activeMotherboard;
        private List<PlatformGenerator.MotherboardGenerationResult> _setupMotherboards = new();

        public void Start()
        {
            _roof?.SetActive(true);

            Regenerate();
        }

        public void OnMotherboardEntered(Motherboard motherboard)
        {
            if (motherboard != _activeMotherboard)
            {
                Debug.Log("CHANGING!!!!!!!!!!!!!!!");
                Regenerate();
            }
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

            var halls = result.Hallways.Select(x => x.Definition);
            var rooms = result.Rooms.Select(x => x.Definition);

            // temp wall impl incase auros doesn't have time

            SpawnEnemies(rooms);

            if (isCurrent)
            {
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
        }
    }
}
