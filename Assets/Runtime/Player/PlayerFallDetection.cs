using UnityEngine;

namespace CMIYC.Player
{
    public class PlayerFallDetection : MonoBehaviour
    {
        private const float _deathPlane = -100f;

        [SerializeField]
        private PlayerController _playerController = null!;

        [SerializeField]
        private Rigidbody _playerRigidbody = null!;

        [SerializeField]
        private DeathController _deathController = null!;

        private void LateUpdate()
        {
            if (_playerController.transform.position.y <= _deathPlane)
            {
                _deathController.Die();

                _playerRigidbody.useGravity = false;
                _playerRigidbody.velocity = Vector3.zero;

                enabled = false;
            }
        }
    }
}
