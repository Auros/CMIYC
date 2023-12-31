﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

namespace CMIYC.Platform
{
    public partial class PlatformGenerator
    {
        private void GenerateHallway(
            Vector2Int start,
            Vector2Int exitTarget,
            Transform motherboardTransform,
            List<RoomInstance> roomInstances,
            ICollection<HallInstance> hallInstances,
            IDictionary<Vector2Int, Definition> definitionLookup)
        {
            bool reachedExitNode = false;

            var allRoomEntrances = roomInstances.SelectMany(r => r.Definition.GetEntranceNodes().Select(w => new RoomDoorInfo
            {
                Location = r.Definition.TransformLocation(w),
                Instance = r
            })).ToList();

            foreach (var roomEntrance in allRoomEntrances.Where(r => definitionLookup.ContainsKey(r.Location)).ToList())
            {
                var location = roomEntrance.Location;
                var roomDefinition = roomEntrance.Instance.Definition;
                allRoomEntrances.RemoveAll(r => r.Instance.Definition == roomDefinition);
                int removed = roomInstances.RemoveAll(r => r.Definition == roomDefinition);

                if (removed > 0)
                    DespawnRoomDefinition(roomEntrance.Instance, definitionLookup);

                definitionLookup.Remove(location);
            }

            int test = 0;

            bool HasRoom(int targetX, int targetY)
            {
                foreach (var room in roomInstances)
                {
                    for (int x = 0; x < room.Definition.Size.x; x++)
                        for (int y = 0; y < room.Definition.Size.y; y++)
                            if (room.Definition.TransformLocation(new Vector2Int(x, y)) == new Vector2Int(targetX, targetY))
                                return true;
                }

                return false;
            }

            while (!reachedExitNode && 1000 > test++)
            {
                var explored = DictionaryPool<Vector2Int, int>.Get();
                var unexplored = ListPool<UnexploredCell>.Get();

                for (int x = 0; x < _motherboardSize.x; x++)
                for (int y = 0; y < _motherboardSize.y; y++)
                {
                    Vector2Int cell = new(x, y);

                    // Exclude room spots
                    if (definitionLookup.TryGetValue(cell, out var localDef) && localDef is RoomDefinition || HasRoom(x, y))
                        continue;

                    var unexploredCell = _unexploredCellPool.Get();
                    unexploredCell.Value = cell == start ? 0 : int.MaxValue;
                    unexploredCell.Location = cell;

                    unexplored.Add(unexploredCell);
                }

                int attempts = 0;
                bool found = false;

                while (!found && unexplored.Count != 0 && 10_000 > attempts++)
                {
                    unexplored.Sort((a, b) => a.Value.CompareTo(b.Value));
                    var minimumUnexplored = unexplored[0];

                    var (x, y) = minimumUnexplored.Location;
                    var evaluations = ListPool<EvaluatedCell>.Get();

                    Vector2Int west = new(x - 1, y);
                    Vector2Int east = new(x + 1, y);
                    Vector2Int north = new(x, y + 1);
                    Vector2Int south = new(x, y - 1);

                    evaluations.Add(new EvaluatedCell(2, west));
                    evaluations.Add(new EvaluatedCell(2, east));
                    evaluations.Add(new EvaluatedCell(2, north));
                    evaluations.Add(new EvaluatedCell(2, south));

                    foreach (var evaluation in evaluations)
                    {
                        UnexploredCell? evalCell = null;
                        foreach (var t in unexplored)
                        {
                            if (t.Location != evaluation.location)
                                continue;
                            evalCell = t;
                            break;
                        }

                        if (evalCell is null)
                            continue;

                        int cost = evaluation.baseCost + minimumUnexplored.Value;
                        if (definitionLookup.TryGetValue(evaluation.location, out var def) && def is HallDefinition)
                            cost -= 1; // Costs LESS to go through hallways

                        // Already specified value is cheaper to execute
                        if (cost > evalCell.Value)
                            continue;

                        evalCell.Value = cost;
                    }

                    explored.Add(minimumUnexplored.Location, minimumUnexplored.Value);
                    _unexploredCellPool.Release(minimumUnexplored);
                    unexplored.Remove(minimumUnexplored);

                    var isExitNode = exitTarget == minimumUnexplored.Location;
                    RoomDoorInfo? roomEntrance = null;
                    foreach (var entran in allRoomEntrances)
                        if (entran.Location == minimumUnexplored.Location)
                            roomEntrance = entran;

                    if (roomEntrance != null || isExitNode)
                    {
                        found = true;
                        int tracker = 0;

                        // Calculate right angle path (avoid squiggily)

                        var last = minimumUnexplored.Location;
                        var path = ListPool<Vector2Int>.Get();
                        bool reachedStart = last == start;
                        path.Add(last);

                        while (!reachedStart && 1_000 > tracker)
                        {
                            tracker++;
                            var exit = last;

                            Vector2Int aWest = new(exit.x - 1, exit.y);
                            Vector2Int aEast = new(exit.x + 1, exit.y);
                            Vector2Int aNorth = new(exit.x, exit.y + 1);
                            Vector2Int aSouth = new(exit.x, exit.y - 1);

                            using (ListPool<(int, Vector2Int)>.Get(out var nextTargets))
                            {
                                if (explored.TryGetValue(aWest, out var westPow))
                                    nextTargets.Add((westPow, aWest));

                                if (explored.TryGetValue(aEast, out var eastPow))
                                    nextTargets.Add((eastPow, aEast));

                                if (explored.TryGetValue(aNorth, out var northPow))
                                    nextTargets.Add((northPow, aNorth));

                                if (explored.TryGetValue(aSouth, out var southPow))
                                    nextTargets.Add((southPow, aSouth));

                                nextTargets.Sort((a, b) => a.Item1.CompareTo(b.Item1));
                                if (nextTargets.Count <= 0)
                                    continue;

                                var next = nextTargets[0];
                                reachedStart = next.Item2 == start;
                                path.Add(next.Item2);
                                last = next.Item2;
                            }
                        }

                        if (tracker >= 1000)
                        {
                            Debug.LogWarning($"Failed to find path from {start} to {minimumUnexplored.Location}");
                        }

                        foreach (var p in path)
                        {
                            if (definitionLookup.ContainsKey(p))
                                continue;

                            const float cellOffset = daughterboardUnit / 2f;
                            var daughterCenter = new Vector3(0 * daughterboardUnit + cellOffset, 0, 0 * daughterboardUnit + cellOffset) +
                                                 new Vector3(p.x * daughterboardUnit, 0f, p.y * daughterboardUnit);

                            var def = _hallPool.Get();
                            var defTransform = def.transform;
                            // RE: .Cell

                            defTransform.position = daughterCenter;
                            defTransform.SetParent(def.Anchor);
                            def.Anchor.position = daughterCenter;
                            defTransform.SetParent(motherboardTransform);
                            def.Anchor.SetParent(defTransform, true);

                            definitionLookup[p] = def;

                            hallInstances.Add(new HallInstance
                            {
                                Definition = def,
                                Position = p
                            });
                        }

                        ListPool<Vector2Int>.Release(path);

                        // ReSharper disable once ConvertIfStatementToSwitchStatement
                        if (!isExitNode && roomEntrance != null)
                        {
                            allRoomEntrances.Remove(roomEntrance);
                            var newStart = FindRoomDoorEntrance(allRoomEntrances, roomEntrance.Instance.Definition);

                            if (newStart != null)
                            {
                                start = newStart.Location;
                                allRoomEntrances.Remove(newStart);
                            }
                        }
                        else if (isExitNode)
                        {
                            reachedExitNode = true;
                        }
                    }

                    ListPool<EvaluatedCell>.Release(evaluations);
                }

                if (attempts >= 10_000)
                    Debug.LogWarning("Failed to dijkstra");

                foreach (var cell in unexplored)
                    _unexploredCellPool.Release(cell);

                ListPool<UnexploredCell>.Release(unexplored);
                DictionaryPool<Vector2Int, int>.Release(explored);
            }

            // Cull non-connected rooms
            var nonConnected = roomInstances.Where(r => r.Definition.GetEntranceNodes().Count == allRoomEntrances.Count(e => e.Instance.Definition == r.Definition)).Select(r => r.Definition).ToList();

            foreach (var roomDefinition in nonConnected)
            {
                allRoomEntrances.RemoveAll(r => r.Instance.Definition == roomDefinition);
                foreach (var room in roomInstances.Where(r => r.Definition == roomDefinition).ToList())
                {
                    roomInstances.Remove(room);
                    definitionLookup.Remove(room.Position);
                    DespawnRoomDefinition(room, definitionLookup);
                }
            }
        }

        private RoomDoorInfo? FindRoomDoorEntrance(List<RoomDoorInfo> rooms, RoomDefinition roomDef)
        {
            foreach (var room in rooms)
                if (room.Instance.Definition == roomDef)
                    return room;
            return null;
        }

        private class RoomDoorInfo
        {
            public Vector2Int Location { get; set; }

            public RoomInstance Instance { get; set; } = null!;
        }

        private readonly struct EvaluatedCell
        {
            public readonly int baseCost;
            public readonly Vector2Int location;

            public EvaluatedCell(int baseCost, Vector2Int location)
            {
                this.baseCost = baseCost;
                this.location = location;
            }
        }

        private class UnexploredCell
        {
            public int Value { get; set; }
            public Vector2Int Location { get; set; }
        }
    }
}
