using CMIYC.Weapons;
using UnityEngine;

namespace CMIYC.Items
{
    public class WeaponItemDefinition : MonoBehaviour
    {
        [Tooltip("Prefab to use for weapon")]
        [field: SerializeField] public WeaponDefinition Weapon { get; private set; }

        [Tooltip("Layers for the projectile to ignore or collide with.")]
        [SerializeField] private LayerMask _layerMask;

        // If the physics system reports a collision, we should assume that we need to call back.
        private void OnCollisionEnter(Collision collision)
        {
            // Ensure we are colliding only with layers we want
            if ((_layerMask.value & 1 << collision.gameObject.layer) <= 0) return;

            var pickupEvent = new WeaponItemPickupEvent(this, collision.collider);

            Callback(pickupEvent);
            Destroy(gameObject);
        }

        private void Callback(WeaponItemPickupEvent pickupEvent)
        {
            // I think using Messages would be more performant than manually iterating through every component?
            pickupEvent.Collider
                .BroadcastMessage(nameof(IWeaponItemPickerUpper.OnItemPickup), pickupEvent, SendMessageOptions.DontRequireReceiver);
        }
    }
}
