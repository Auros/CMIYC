using System;
using UnityEngine;

namespace CMIYC.Platform
{
    public abstract class Definition : MonoBehaviour
    {
        [SerializeField]
        private Vector2Int _size = new(1, 1);

        [SerializeField]
        protected Transform _originAnchorTarget = null!;

        public Vector2Int Size => _size;

        public Transform Anchor => _originAnchorTarget;

        protected virtual void OnDrawGizmos()
        {
            if (_originAnchorTarget == null)
                return;

            Gizmos.color = Color.cyan;
            var self = transform;
            Gizmos.matrix = self.localToWorldMatrix;
            var originAnchorPos = _originAnchorTarget.localPosition;
            Gizmos.DrawCube(originAnchorPos.WithY(5f), new Vector3(0.3f, 30f, 0.3f));

            Gizmos.color = Color.yellow.WithA(0.3f);
            const int dbu = PlatformGenerator.daughterboardUnit;

            for (int x = 0; x < _size.x; x++)
                for (int y = 0; y < _size.y; y++)
                    Gizmos.DrawCube(originAnchorPos.WithY(0.25f) + new Vector3(x * dbu, 0, y * dbu), new Vector3(dbu, 1f, dbu));
        }
    }
}
