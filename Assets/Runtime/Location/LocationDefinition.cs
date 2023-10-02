using CMIYC.Player;
using UnityEngine;

namespace CMIYC.Location
{
    public class LocationDefinition : MonoBehaviour
    {
        [SerializeField]
        private string _location = string.Empty;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent<PlayerController>(out var player))
            {
                player.BroadcastMessage(nameof(LocationController.EnterLocation), _location, SendMessageOptions.DontRequireReceiver);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.TryGetComponent<PlayerController>(out var player))
            {
                player.BroadcastMessage(nameof(LocationController.ExitLocation), SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}
