using UnityEngine;

namespace CMIYC.Items
{
    public readonly struct WeaponItemPickupEvent
    {
        /// <summary>
        /// Collider the projectile hit.
        /// </summary>
        public readonly Collider Collider { get; }

        /// <summary>
        /// The instance of the projectile triggering this event.
        /// </summary>
        public readonly WeaponItemDefinition Instance { get; }

        public WeaponItemPickupEvent(WeaponItemDefinition instance, Collider collider)
        {
            Instance = instance;
            Collider = collider;
        }
    }
}
