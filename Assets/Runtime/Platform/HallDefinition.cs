using System;
using CMIYC.Platform.Walls;
using UnityEngine;

namespace CMIYC.Platform
{
    public class HallDefinition : Definition
    {
        [field: SerializeField]
        public TempWallsDefinition TempWallsDefinition = null!;

        [SerializeField]
        private Transform _northWallAnchorTarget = null!;

        [SerializeField]
        private Transform _eastWallAnchorTarget = null!;

        [SerializeField]
        private Transform _southWallAnchorTarget = null!;

        [SerializeField]
        private Transform _westWallAnchorTarget = null!;

        [field: SerializeField]
        public Vector2Int Cell { get; set; }

        public void SetWall(Cardinal direction, Wall? wall)
        {
            // Get the right anchor target based on the wall direction
            var target = direction switch
            {
                Cardinal.North => _northWallAnchorTarget,
                Cardinal.East => _eastWallAnchorTarget,
                Cardinal.South => _southWallAnchorTarget,
                Cardinal.West => _westWallAnchorTarget,
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            };

            // idk what to do with this for now
            if (wall is null)
                return;

            var wallTransform = wall.transform;
            var positionalOffset = target.position - wallTransform.position;
            var rotationalOffset = target.rotation * Quaternion.Inverse(wallTransform.rotation);


        }

        protected override void OnDrawGizmos()
        {

        }
    }
}
