using System.Collections.Generic;
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

            var allRoomEntrances = roomInstances.SelectMany(r => r.Definition.GetEntranceNodes().Select(w => new RoomDoorInfo()
            {
                Location = r.Definition.TransformLocation(w),
                Room = r.Definition
            })).ToList();

            foreach (var roomEntrance in allRoomEntrances.Where(r => definitionLookup.ContainsKey(r.Location)).ToList())
            {
                var location = roomEntrance.Location;
                var roomDefinition = roomEntrance.Room;
                allRoomEntrances.RemoveAll(r => r.Room == roomDefinition);
                roomInstances.RemoveAll(r => r.Definition == roomDefinition);
                Destroy(roomDefinition.gameObject);

                definitionLookup.Remove(location);
            }

            int test = 0;

            while (!reachedExitNode && 10 > test++)
            {
                var explored = DictionaryPool<Vector2Int, int>.Get();
                var unexplored = ListPool<UnexploredCell>.Get();

                for (int x = 0; x < _motherboardSize.x; x++)
                for (int y = 0; y < _motherboardSize.y; y++)
                {
                    Vector2Int cell = new(x, y);

                    // Exclude room spots
                    if (definitionLookup.TryGetValue(cell, out var localDef) && localDef is RoomDefinition)
                        continue;

                    unexplored.Add(new UnexploredCell
                    {
                        Location = cell,
                        Value = cell == start ? 0 : int.MaxValue
                    });
                }

                int attempts = 0;
                bool found = false;

                while (!found && unexplored.Count != 0 && 10_000 > attempts++)
                {
                    var minimumUnexplored = unexplored.OrderBy(u => u.Value).First();

                    var (x, y) = minimumUnexplored.Location;
                    var evaluations = ListPool<EvaluatedCell>.Get();

                    // Base Cost: 1
                    Vector2Int west = new(x - 1, y);
                    Vector2Int east = new(x + 1, y);
                    Vector2Int north = new(x, y + 1);
                    Vector2Int south = new(x, y - 1);

                    evaluations.Add(new EvaluatedCell(2, west));
                    evaluations.Add(new EvaluatedCell(2, east));
                    evaluations.Add(new EvaluatedCell(2, north));
                    evaluations.Add(new EvaluatedCell(2, south));

                    // Base Cost: 3
                    Vector2Int northwest = new(x - 1, y + 1);
                    Vector2Int northeast = new(x + 1, y + 1);
                    Vector2Int southwest = new(x - 1, y - 1);
                    Vector2Int southeast = new(x + 1, y - 1);

                    evaluations.Add(new EvaluatedCell(5, northwest));
                    evaluations.Add(new EvaluatedCell(5, northeast));
                    evaluations.Add(new EvaluatedCell(5, southwest));
                    evaluations.Add(new EvaluatedCell(5, southeast));

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
                    unexplored.Remove(minimumUnexplored);

                    var isExitNode = exitTarget == minimumUnexplored.Location;
                    var roomEntrance = allRoomEntrances.FirstOrDefault(r => r.Location == minimumUnexplored.Location);

                    if (roomEntrance != null || isExitNode)
                    {
                        found = true;
                        int tracker = 0;

                        // Calculate right angle path (avoid squiggily)

                        var last = minimumUnexplored.Location;
                        var path = ListPool<Vector2Int>.Get();
                        bool reachedStart = false;
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

                                var next = nextTargets.OrderBy(w => w.Item1).FirstOrDefault();
                                reachedStart = next.Item2 == start;
                                path.Add(next.Item2);
                                last = next.Item2;
                            }
                        }

                        foreach (var p in path)
                        {
                            if (definitionLookup.ContainsKey(p))
                                continue;

                            const float cellOffset = daughterboardUnit / 2f;
                            var daughterCenter = new Vector3(0 * daughterboardUnit + cellOffset, 0, 0 * daughterboardUnit + cellOffset) +
                                                 new Vector3(p.x * daughterboardUnit, 0f, p.y * daughterboardUnit);

                            var def = Instantiate(_hallwayPrefab, null, true);
                            var defTransform = def.transform;
                            def.Cell = p;

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
                            var newStart = allRoomEntrances.FirstOrDefault(r => r.Room == roomEntrance.Room);

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

                ListPool<UnexploredCell>.Release(unexplored);
                DictionaryPool<Vector2Int, int>.Release(explored);
            }

            // Cull non-connected rooms
            var nonConnected = roomInstances.Where(r => r.Definition.GetEntranceNodes().Count == allRoomEntrances.Count(e => e.Room == r.Definition)).Select(r => r.Definition).ToList();

            foreach (var roomDefinition in nonConnected)
            {
                allRoomEntrances.RemoveAll(r => r.Room == roomDefinition);
                roomInstances.RemoveAll(r => r.Definition == roomDefinition);
                Destroy(roomDefinition.gameObject);
            }

            Debug.Log(allRoomEntrances.Count);
        }

        private class RoomDoorInfo
        {
            public Vector2Int Location { get; set; }

            public RoomDefinition Room { get; set; } = null!;
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
