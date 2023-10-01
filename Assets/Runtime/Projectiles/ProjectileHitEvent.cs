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
        /// Position of the projectile collision
        /// </summary>
        public Vector3 Point { get; }

        /// <summary>
        /// Direction of the projectile collision
        /// </summary>
        public Vector3 Normal { get; }

        /// <summary>
        /// The instance of the projectile triggering this event.
        /// </summary>
        public readonly ProjectileDefinition Instance { get; }

        public ProjectileHitEvent(ProjectileDefinition instance, Collider collider, Vector3 point, Vector3 normal)
        {
            Instance = instance;
            Collider = collider;
            Point = point;
            Normal = normal;
        }
    }
}
