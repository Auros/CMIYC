using UnityEngine;

namespace CMIYC.UI
{
    public class HUDMovementReaction : MonoBehaviour
    {
        [Tooltip("Transform for the HUD to react to")]
        [SerializeField] private Transform _characterTransform;

        [SerializeField] private PlayerHealthController _playerHealthController;

        [Header("Parameters")]
        [SerializeField] private bool _useUnscaledTime;
        [SerializeField] private float _reactionStrength;
        [SerializeField] private float _movementSensitivity;
        [SerializeField] private float _rotationSensitivity;

        private Transform _transform;

        private Vector3 _characterPreviousPos;
        private Vector3 _characterPerviousEuler;

        private Vector3 _offsetPos;
        private Vector3 _offsetEuler;


        private void Start()
        {
            _transform = transform;

            if (_characterTransform == null)
            {
                Debug.LogWarning($"lmao someone forgot to assign {nameof(_characterTransform)}, disabling {nameof(HUDMovementReaction)}...");
                enabled = false;
            }

            if (_playerHealthController == null)
            {
                Debug.LogWarning($"lmao someone forgot to assign {nameof(_playerHealthController)}, disabling {nameof(HUDMovementReaction)}...");
                enabled = false;
                return;
            }

            _playerHealthController.PlayerTookDamage += PlayerTookDamage;
        }

        private void PlayerTookDamage() => _offsetEuler += 5f * Random.Range(-1, 1f) * Vector3.forward;

        private void LateUpdate()
        {
            // Retrieve position/euler rotation from our target transform
            var characterPos = _characterTransform.position;
            var characterEuler = _characterTransform.eulerAngles;

            // Calculate offsets based on normalized difference, multiplied by sensitivity
            // (we inverse transform the position offset to get away from world space, which causes some problems)
            var movementDirection = _characterPreviousPos - characterPos;
            movementDirection = Mathf.Min(movementDirection.magnitude, 1f) * movementDirection.normalized;
            var posOffset = _characterTransform.InverseTransformDirection(_movementSensitivity * movementDirection);

            var rotationDirection = _characterPerviousEuler - characterEuler;
            rotationDirection = Mathf.Min(rotationDirection.magnitude, 1f) * rotationDirection.normalized;
            var eulerOffset = _rotationSensitivity * rotationDirection;

            // Lerp between our old offset and the current offset
            var dT = _useUnscaledTime
                ? Time.unscaledDeltaTime
                : Time.deltaTime;

            _offsetPos = Vector3.Lerp(_offsetPos, posOffset, dT * _reactionStrength);
            _offsetEuler = Vector3.Lerp(_offsetEuler, eulerOffset, dT * _reactionStrength);

            // Apply new offsets to the HUD
            _transform.localPosition = _offsetPos;
            _transform.localEulerAngles = _offsetEuler;

            // Store the offsets for the next frame
            _characterPreviousPos = characterPos;
            _characterPerviousEuler = characterEuler;
        }

        private void OnDestroy()
        {
            _playerHealthController.PlayerTookDamage -= PlayerTookDamage;
        }
    }
}
