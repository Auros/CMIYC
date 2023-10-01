using AuraTween;
using UnityEngine;

namespace CMIYC
{
    public class UIFadeOnDeath : MonoBehaviour
    {
        [SerializeField]
        private DeathController _deathController = null!;

        [SerializeField]
        private TweenManager _tweenManager = null!;

        [SerializeField]
        private Transform _scaleProxy = null!;

        private CanvasGroup _canvasGroup = null!;

        private void Start()
        {
            if (!TryGetComponent(out _canvasGroup))
            {
                _canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }

            _deathController.OnPlayerDeath += OnPlayerDeath;
        }

        private void OnPlayerDeath()
        {
            const float fadeAnimation = 1.5f;

            _tweenManager.Run(1, 0, fadeAnimation, a => _canvasGroup.alpha = a, Easer.OutCubic);
            _tweenManager.Run(1, 1.5f, fadeAnimation, s => _scaleProxy.localScale = s * Vector3.one, Easer.InCirc);
        }

        private void OnDestroy()
        {
            _deathController.OnPlayerDeath -= OnPlayerDeath;
        }
    }
}
