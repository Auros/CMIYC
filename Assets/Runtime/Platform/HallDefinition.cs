using System;
using UnityEngine;

namespace CMIYC.Platform
{
    public class HallDefinition : Definition
    {
        [SerializeField]
        private WallController _northWall = null!;

        [SerializeField]
        private WallController _eastWall = null!;

        [SerializeField]
        private WallController _southWall = null!;

        [SerializeField]
        private WallController _westWall = null!;

        [SerializeField]
        private Renderer[] _coloredRenderers = Array.Empty<Renderer>();

        public void SetColor(Color color)
        {
            foreach (var coloredRenderer in _coloredRenderers)
            {
                if (coloredRenderer == null || !coloredRenderer.enabled) continue;
                coloredRenderer.material.color = color;
            }
        }

        public WallController GetWall(Cardinal cardinal) => cardinal switch
        {
            Cardinal.North => _northWall,
            Cardinal.East => _eastWall,
            Cardinal.South => _southWall,
            Cardinal.West => _westWall,
            _ => throw new ArgumentOutOfRangeException(nameof(cardinal), cardinal, null)
        };
    }
}
