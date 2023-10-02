using UnityEngine;

namespace CMIYC.Items
{
    public abstract class ItemDefinition : MonoBehaviour
    {
        [Tooltip("Layers for the item to collide with.")]
        [SerializeField] private LayerMask _layerMask;

        // If the physics system reports a collision, we should assume that we need to call back.
        private void OnCollisionEnter(Collision collision)
        {
            // Ensure we are colliding only with layers we want
            if ((_layerMask.value & 1 << collision.gameObject.layer) <= 0) return;

            var pickupEvent = new ItemPickupEvent(this, collision.collider);

            Callback(pickupEvent);
            Destroy(gameObject);
        }

        private void Callback(ItemPickupEvent pickupEvent)
        {
            // I think using Messages would be more performant than manually iterating through every component?
            pickupEvent.Collider
                .BroadcastMessage(nameof(IItemPickerUpper.OnItemPickup), pickupEvent, SendMessageOptions.DontRequireReceiver);
        }
    }
}
