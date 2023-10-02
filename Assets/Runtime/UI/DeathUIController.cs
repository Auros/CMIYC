using System;
using System.Diagnostics.CodeAnalysis;
using AuraTween;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CMIYC.UI
{
    public class DeathUIController : MonoBehaviour
    {
        [SerializeField]
        private DeathController _deathController = null!;

        [SerializeField]
        private PlayerHealthController _healthController = null!;

        [SerializeField]
        private GameObject _wrapperObject = null!;

        [SerializeField]
        private Transform _deathPanelParent = null!;

        [SerializeField]
        private TweenManager _tweenManager = null!;

        [SerializeField]
        private Image _background = null!;

        [SerializeField]
        private float _initialDelay = 1f;

        [SerializeField]
        private float _animationOffset = 0.2f;

        [SerializeField]
        private float _animationLength = 0.5f;

        private Tween? _damageTween;

        private void Start()
        {
            _deathController.OnPlayerDeath += OnPlayerDeath;
            _healthController.PlayerTookDamage += PlayerTookDamage;
        }

        private void OnPlayerDeath() => DeathAsync().Forget();

        private void PlayerTookDamage()
        {
            _damageTween?.Cancel();

            if (_healthController.Health <= 0) return;

            // Alpha will be 0 - 0.5 depending on health of the player
            var inverseHealth = 1f - (_healthController.Health / _healthController.InitialHealth);

            var alpha = Mathf.Lerp(0.25f, 0.6f, inverseHealth);

            _damageTween = _tweenManager.Run(alpha, 0, 1f, a => _background.color = _background.color.WithA(a), Easer.OutCubic);
        }

        private void OnDestroy()
        {
            _deathController.OnPlayerDeath -= OnPlayerDeath;
            _healthController.PlayerTookDamage -= PlayerTookDamage;
        }

        private async UniTask DeathAsync()
        {
            _wrapperObject.SetActive(true);
            _damageTween?.Cancel();

#pragma warning disable CS4014 // We do *not* want to await TweenManager.Run calls here
            _background.color = _background.color.WithA(0.75f);
            _tweenManager.Run(1f, 0.5f, 2f, r => _background.color = _background.color.WithR(r), Easer.OutCubic);

            for (var i = 0; i < _deathPanelParent.childCount; i++)
            {
                var child = _deathPanelParent.GetChild(i);

                if (!child.TryGetComponent<CanvasGroup>(out var group))
                {
                    group = child.gameObject.AddComponent<CanvasGroup>();
                }

                group.alpha = 0f;
            }

            await UniTask.Delay(TimeSpan.FromSeconds(_initialDelay));

            for (var i = 0; i < _deathPanelParent.childCount; i++)
            {
                var child = _deathPanelParent.GetChild(i) as RectTransform;

                if (!child.TryGetComponent<CanvasGroup>(out var group))
                {
                    group = child.gameObject.AddComponent<CanvasGroup>();
                }

                _tweenManager.Run(0f, 1f, _animationLength, a => group.alpha = a, Easer.OutSine);
                _tweenManager.Run(-1000f, 0f, _animationLength, x => child.anchoredPosition = child.anchoredPosition.WithX(x), Easer.OutCubic);

                await UniTask.Delay(TimeSpan.FromSeconds(_animationOffset));
            }
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        public void Quit() => Application.Quit();

        public void Restart() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
