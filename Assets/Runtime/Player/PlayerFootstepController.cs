using CMIYC.Audio;
using UnityEngine;

namespace CMIYC.Player
{
    public class PlayerFootstepController : MonoBehaviour
    {
        [SerializeField]
        private float _stepsPerMeter = 3;

        [Space]
        [SerializeField]
        private Rigidbody _rigidbody = null!;

        [SerializeField]
        private AudioPool _audioPool = null!;

        [SerializeField]
        private AudioClip _stepClip = null!;

        [SerializeField]
        private PlayerController _playerController = null!;

        private float _timeSinceLastStep = 0f;

        private void Update()
        {
            var normalizedVelocity = _rigidbody.velocity.magnitude;
            var shouldStep = _playerController.IsGrounded && normalizedVelocity > 0;

            _timeSinceLastStep += Time.deltaTime;

            if (shouldStep && _timeSinceLastStep > 1f / (normalizedVelocity * _stepsPerMeter))
            {
                _audioPool.Play(_stepClip);
                _timeSinceLastStep = 0f;
            }
        }
    }
}
