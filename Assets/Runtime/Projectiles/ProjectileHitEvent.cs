using UnityEngine;

namespace CMIYC.Projectiles
{
    public readonly struct ProjectileHitEvent
    {
        /// <summary>
        /// Collider the projectile hit.
        /// </summary>
        public readonly Collider Collider { get; }

        /// <summary>
        /// The instance of the projectile triggering this event.
        /// </summary>
        public readonly ProjectileDefinition Instance { get; }

        public ProjectileHitEvent(ProjectileDefinition instance, Collider collider)
        {
            Instance = instance;
            Collider = collider;
        }
    }
}
