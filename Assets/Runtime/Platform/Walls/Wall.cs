using UnityEngine;

namespace CMIYC.Platform.Walls
{
    public abstract class Wall : MonoBehaviour
    {
        [SerializeField]
        private Transform _anchorTarget = null!;

        public Transform Anchor => _anchorTarget;
    }
}
