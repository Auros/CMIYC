using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CMIYC.Platform
{
    public class RuntimePlatformController : MonoBehaviour
    {
        [SerializeField]
        private PlatformGenerator _platformGenerator = null!;

        [SerializeField]
        private Transform _player = null!;

        public void Start()
        {
            var result = _platformGenerator.BuildMotherboardUntilMinRoomsMet(Cardinal.South, new Vector2Int(0, 0));
            if (result is null)
                return; // TODO handle

            // do whatever else here
            Debug.Log(result.Advancement);
            Debug.Log(result.End);

            var multResult = result.End * 10;
            // idk why moving player isnt working lol
            // teleport start location to player
            result.Motherboard.transform.position = new Vector3(-multResult.x - 5, 0, -multResult.y - 5);

            // lmao i'm losing it
            // i really just don't have enough time to do anything else rn
            var halls = (Object.FindObjectsOfType<HallDefinition>() as HallDefinition[]).Where(x => x.transform.parent == result.Motherboard.transform);
            var rooms = (Object.FindObjectsOfType<RoomDefinition>() as RoomDefinition[]).Where(x => x.transform.parent == result.Motherboard.transform);

            // i literally do not know why the grid direction isn't consistent
            List<Vector2Int> _takenVectors = halls.Select(x => x.Cell).Distinct().ToList();
            Dictionary<Vector2Int, Cardinal> _roomCardinals = new();


            var hall = GetConnectedHall(result.End, halls);
            if (hall != null)
            {
                _player.transform.LookAt(GetConnectedHall(result.End, halls)?.transform);
                _player.transform.localRotation = Quaternion.Euler(0, _player.transform.localRotation.eulerAngles.y, 0);
            }

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

            foreach (var otherHall in halls)
            {
                ApplyTempWalls(otherHall, Cardinal.North, HallExists(otherHall.Cell, Cardinal.North, _takenVectors, _roomCardinals));
                ApplyTempWalls(otherHall, Cardinal.South, HallExists(otherHall.Cell, Cardinal.South, _takenVectors, _roomCardinals));
                ApplyTempWalls(otherHall, Cardinal.East, HallExists(otherHall.Cell, Cardinal.East, _takenVectors, _roomCardinals));
                ApplyTempWalls(otherHall, Cardinal.West, HallExists(otherHall.Cell, Cardinal.West, _takenVectors, _roomCardinals));
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

        private void ApplyTempWalls(HallDefinition hall, Cardinal cardinal, bool hallExists)
        {
            TempWallDirection? wallDirection = null;
            if (cardinal == Cardinal.East) wallDirection = hall.TempWallsDefinition.EastWalls;
            if (cardinal == Cardinal.South) wallDirection = hall.TempWallsDefinition.SouthWalls;
            if (cardinal == Cardinal.North) wallDirection = hall.TempWallsDefinition.NorthWalls;
            if (cardinal == Cardinal.West) wallDirection = hall.TempWallsDefinition.WestWalls;

            if (wallDirection == null) return;
            foreach (var wall in wallDirection.Doors)
            {
                wall.gameObject.SetActive(hallExists);
            }
            foreach (var wall in wallDirection.Walls)
            {
                wall.gameObject.SetActive(!hallExists);
            }
        }

        // please rewrite if we have time i'm sure this data is accessible idk where tho
        public HallDefinition? GetConnectedHall(Vector2Int hallCell, IEnumerable<HallDefinition> halls)
        {
            foreach (var otherHall in halls)
            {
                if (otherHall.Cell == hallCell) continue;

                if (otherHall.Cell.x == hallCell.x)
                {
                    if (otherHall.Cell.y == hallCell.y + 1)
                    {
                        return otherHall;
                    }
                    if (otherHall.Cell.y == hallCell.y - 1)
                    {
                        return otherHall;
                    }
                }

                if (otherHall.Cell.y == hallCell.y)
                {
                    if (otherHall.Cell.x == hallCell.x + 1)
                    {
                        return otherHall;
                    }
                    if (otherHall.Cell.x == hallCell.x - 1)
                    {
                        return otherHall;
                    }
                }
            }

            return null;
        }
    }
}
