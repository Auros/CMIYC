using UnityEngine;

namespace CMIYC.Items
{
    public readonly struct ItemPickupEvent
    {
        /// <summary>
        /// Collider the projectile hit.
        /// </summary>
        public readonly Collider Collider { get; }

        /// <summary>
        /// The instance of the projectile triggering this event.
        /// </summary>
        public readonly ItemDefinition Instance { get; }

        public ItemPickupEvent(ItemDefinition instance, Collider collider)
        {
            Instance = instance;
            Collider = collider;
        }
    }
}
