using UnityEngine;

namespace CMIYC.Projectiles
{
    public readonly struct ProjectileHitEvent
    {
        public readonly Collider Collider { get; }
        public readonly ProjectileDefinition Instance { get; }

        public ProjectileHitEvent(ProjectileDefinition instance, RaycastHit hit)
        {
            Instance = instance;
            Collider = hit.collider;
        }
    }
}
