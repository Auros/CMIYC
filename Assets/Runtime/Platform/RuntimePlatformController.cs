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
        private EnemyController _enemyController = null!;

        [SerializeField]
        private GameObject _roof = null!;

        public void Start()
        {
            _roof?.SetActive(true);

            _platformGenerator.Advance();

            // figure out how this gets updated, and when  I need to do stuff
            var result = _platformGenerator.Current;
            if (result is null)
                return; // TODO handle

            // lmao i'm losing it
            // i really just don't have enough time to do anything else rn
            var halls = result.Hallways.Select(x => x.Definition);
            var rooms = result.Rooms.Select(x => x.Definition);

            // i literally do not know why the grid direction isn't consistent
            Dictionary<Vector2Int, Cardinal> _roomCardinals = new();

            // temp wall impl incase auros doesn't have time
            foreach (var room in rooms)
            {
                var occupiedSegments = room.GetOccupiedNodes();
                for (int i = 0; i < room.Size.x; i++)
                {
                    for (int j = 0; j < room.Size.y; j++)
                    {
                        var unrotated = new Vector2Int(i, j);
                        if (!occupiedSegments.TryGetValue(unrotated, out Cardinal segmentCardinal)) continue;

                        Vector2Int location = room.AnchorLocation;
                        if (room.Cardinal == Cardinal.West) location = room.AnchorLocation + new Vector2Int(-i, j);
                        if (room.Cardinal == Cardinal.East) location = room.AnchorLocation + new Vector2Int(i, -j);
                        if (room.Cardinal == Cardinal.South) location = room.AnchorLocation + new Vector2Int(-i, -j);
                        if (room.Cardinal == Cardinal.North) location = room.AnchorLocation + new Vector2Int(i, j);

                        if ((segmentCardinal == Cardinal.East || segmentCardinal == Cardinal.West))
                        {
                            if (room.Cardinal == Cardinal.West || room.Cardinal == Cardinal.East)
                            {
                                segmentCardinal = segmentCardinal == Cardinal.East ? Cardinal.West : Cardinal.East;
                            }
                        }
                        _roomCardinals.TryAdd(location, Multiply(room.Cardinal, segmentCardinal));
                    }
                }
            }

            SpawnEnemies(rooms);
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

        // idk how auros is doing it but i can do this quickly
        private Cardinal Multiply(Cardinal a, Cardinal b)
        {
            if (a == Cardinal.West)
            {
                switch (b)
                {
                    case Cardinal.South:
                        return Cardinal.West;
                    case Cardinal.West:
                        return Cardinal.North;
                    case Cardinal.North:
                        return Cardinal.East;
                    case Cardinal.East:
                        return Cardinal.South;
                }
            }
            if (a == Cardinal.East)
            {
                switch (b)
                {
                    case Cardinal.South:
                        return Cardinal.East;
                    case Cardinal.West:
                        return Cardinal.South;
                    case Cardinal.North:
                        return Cardinal.West;
                    case Cardinal.East:
                        return Cardinal.North;
                }
            }
            if (a == Cardinal.North)
            {
                switch (b)
                {
                    case Cardinal.South:
                        return Cardinal.North;
                    case Cardinal.West:
                        return Cardinal.East;
                    case Cardinal.North:
                        return Cardinal.South;
                    case Cardinal.East:
                        return Cardinal.West;
                }
            }

            return b;
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
