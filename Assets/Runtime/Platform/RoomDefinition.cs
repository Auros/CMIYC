using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace CMIYC.Platform
{
    [PublicAPI]
    public class RoomDefinition : Definition
    {
        [SerializeField]
        private RoomSegment[] _roomSegments = Array.Empty<RoomSegment>();

        public Cardinal Cardinal { get; private set; }

        public Vector2Int AnchorLocation { get; private set; }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            Gizmos.color = Color.magenta.WithA(0.2f);

            var self = transform;
            Gizmos.matrix = self.localToWorldMatrix;

            var originAnchorPos = _originAnchorTarget.localPosition;
            foreach (var segment in _roomSegments)
            {
                var target = originAnchorPos.WithY(4f) + new Vector3(PlatformGenerator.daughterboardUnit * segment.Location.x, 0f, PlatformGenerator.daughterboardUnit * segment.Location.y);
                Gizmos.DrawCube(target, new Vector3(2f, 8f, 2f));
            }
        }

        public void SetData(Cardinal cardinal, Vector2Int anchorLocation)
        {
            Cardinal = cardinal;
            AnchorLocation = anchorLocation;
        }

        public WallSegmentType GetWallSegmentType(Cardinal cardinal, Vector2Int location)
        {
            location = InverseTransformLocation(location);

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var roomSegment in _roomSegments)
            {
                if (roomSegment.Location != location)
                    continue;

                var localizedCardinal = (Cardinal)Mathf.Abs(((int)cardinal - (int)Cardinal) % 4);
                return roomSegment.GetWallSegmentType(localizedCardinal);
            }
            return WallSegmentType.None;
        }

        public Vector2Int InverseTransformLocation(Vector2Int location)
        {
            var sub = Cardinal switch
            {
                Cardinal.North => new Vector2Int(location.x - AnchorLocation.x, location.y - AnchorLocation.y),
                Cardinal.East => new Vector2Int(AnchorLocation.y - location.y,  location.x - AnchorLocation.x),
                Cardinal.South => new Vector2Int(AnchorLocation.x - location.x, AnchorLocation.y - location.y),
                Cardinal.West => new Vector2Int(location.y - AnchorLocation.y,  AnchorLocation.x - location.x),
                _ => throw new ArgumentOutOfRangeException()
            };

            return sub;
        }

        public Vector2Int TransformLocation(Vector2Int location)
        {
            var sub = Cardinal switch
            {
                Cardinal.North => new Vector2Int(AnchorLocation.x + location.x, AnchorLocation.y + location.y),
                Cardinal.East => new Vector2Int(AnchorLocation.x + location.y, AnchorLocation.y - location.x),
                Cardinal.South => new Vector2Int(AnchorLocation.x - location.x, AnchorLocation.y - location.y),
                Cardinal.West => new Vector2Int(AnchorLocation.x - location.y, AnchorLocation.y + location.x),
                _ => throw new ArgumentOutOfRangeException()
            };

            return sub;
        }

        public IReadOnlyList<Vector2Int> GetEntranceNodes()
        {
            // TODO: Optimize
            List<Vector2Int> entranceNodes = new();
            foreach (var segment in _roomSegments)
            {
                var (x, y) = segment.Location;
                if (segment.GetWallSegmentType(Cardinal.North) is WallSegmentType.Door)
                    entranceNodes.Add(new Vector2Int(x, y + 1));

                if (segment.GetWallSegmentType(Cardinal.East) is WallSegmentType.Door)
                    entranceNodes.Add(new Vector2Int(x + 1, y));

                if (segment.GetWallSegmentType(Cardinal.South) is WallSegmentType.Door)
                    entranceNodes.Add(new Vector2Int(x, y - 1));

                if (segment.GetWallSegmentType(Cardinal.West) is WallSegmentType.Door)
                    entranceNodes.Add(new Vector2Int(x - 1, y));
            }
            return entranceNodes;
        }

        public Dictionary<Vector2Int, Cardinal> GetOccupiedNodes()
        {
            Dictionary<Vector2Int, Cardinal> nodes = new();
            foreach (var segment in _roomSegments)
            {
                var location = segment.Location;
                Cardinal cardinal = Cardinal.North;
                // only supports one door direction but thats all im doing
                if (segment.GetWallSegmentType(Cardinal.East) == WallSegmentType.Door) cardinal = Cardinal.East;
                if (segment.GetWallSegmentType(Cardinal.West) == WallSegmentType.Door) cardinal = Cardinal.West;
                if (segment.GetWallSegmentType(Cardinal.South) == WallSegmentType.Door) cardinal = Cardinal.South;
                if (segment.GetWallSegmentType(Cardinal.North) == WallSegmentType.Door) cardinal = Cardinal.North;

                nodes.TryAdd(location, cardinal);
            }

            return nodes;
        }
    }
}
