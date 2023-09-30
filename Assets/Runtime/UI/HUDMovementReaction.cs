using UnityEngine;

namespace CMIYC.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class HUDMovementReaction : MonoBehaviour
    {
        [Tooltip("Transform for the HUD to react to")]
        [SerializeField] private Transform _characterTransform;

        [Header("Parameters")]
        [SerializeField] private float _reactionStrength;
        [SerializeField] private float _movementSensitivity;
        [SerializeField] private float _rotationSensitivity;

        private RectTransform _rect;

        private Vector3 _characterPreviousPos;
        private Vector3 _characterPerviousEuler;

        private Vector3 _offsetPos;
        private Vector3 _offsetEuler;


        private void Start()
        {
            _rect = transform as RectTransform;

            if (_characterTransform == null)
            {
                Debug.LogWarning($"lmao someone forgot to assign {nameof(_characterTransform)}, disabling {nameof(HUDMovementReaction)}...");
                enabled = false;
            }
        }

        private void LateUpdate()
        {
            // Retrieve position/euler rotation from our target transform
            var characterPos = _characterTransform.position;
            var characterEuler = _characterTransform.eulerAngles;

            // Calculate offsets based on normalized difference, multiplied by sensitivity
            var posOffset = _movementSensitivity * (_characterPreviousPos - characterPos).normalized;
            var eulerOffset = _rotationSensitivity * (_characterPerviousEuler - characterEuler).normalized;

            // Lerp between our old offset and the current offset
            var dT = Time.deltaTime;
            _offsetPos = Vector3.Lerp(_offsetPos, posOffset, dT * _reactionStrength);
            _offsetEuler = Vector3.Lerp(_offsetEuler, eulerOffset, dT * _reactionStrength);

            // Apply new offsets to the HUD
            _rect.anchoredPosition3D = _offsetPos;
            _rect.localEulerAngles = _offsetEuler;

            // Store the offsets for the next frame
            _characterPreviousPos = characterPos;
            _characterPerviousEuler = characterEuler;
        }
    }
}
