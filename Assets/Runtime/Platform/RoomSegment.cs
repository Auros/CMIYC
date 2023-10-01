using System;
using UnityEngine;

namespace CMIYC.Platform
{
    [Serializable]
    public class RoomSegment
    {
        [field: SerializeField]
        public Vector2Int Location { get; private set; }

        [SerializeField]
        private WallSegmentType _north;

        [SerializeField]
        private WallSegmentType _east;

        [SerializeField]
        private WallSegmentType _south;

        [SerializeField]
        private WallSegmentType _west;

        public WallSegmentType GetWallSegmentType(Cardinal cardinal)
        {
            return cardinal switch
            {
                Cardinal.North => _north,
                Cardinal.East => _east,
                Cardinal.South => _south,
                Cardinal.West => _west,
                _ => throw new ArgumentOutOfRangeException(nameof(cardinal), cardinal, null)
            };
        }
    }
}
