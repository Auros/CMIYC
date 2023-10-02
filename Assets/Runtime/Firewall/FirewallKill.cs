using CMIYC.Player;
using UnityEngine;

namespace CMIYC
{
    public class FirewallKill : MonoBehaviour
    {
        [SerializeField]
        private DeathController _deathController = null!;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.GetComponent<PlayerController>())
            {
                _deathController.Die();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.GetComponent<PlayerController>())
            {
                _deathController.Die();
            }
        }
    }
}
