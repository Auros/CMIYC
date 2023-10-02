using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using AuraTween;
using CMIYC.Audio;
using CMIYC.Input;
using CMIYC.Location;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.XInput;
using UnityEngine.UI;

namespace CMIYC.UI
{
    public class StartupUIController : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _gameplayTipText;

        [SerializeField]
        private TMP_Text _locationText;

        [SerializeField]
        private Image _background = null!;

        [SerializeField]
        private float _initialDelay = 1f;

        [SerializeField]
        private float _animationOffset = 0.2f;

        [SerializeField]
        private float _animationLength = 0.5f;

        [SerializeField]
        private float _charactersPerMinute = 300f;

        [SerializeField]
        private AudioClip[] _typeSfx = null!;

        [SerializeField]
        private AudioPool _audioPool = null!;

        [SerializeField]
        private TweenManager _tweenManager = null!;

        [SerializeField]
        private InputController _inputController = null!;

        [SerializeField]
        private MusicLoop _musicLoop = null!;

        [SerializeField]
        private LocationController _locationController = null!;

        private CancellationTokenSource? _cts = null!;

        private async UniTask Start()
        {
            _musicLoop.EnableLowPass(0);
            _inputController.Disable(_inputController.Input.Player.Look);

            var gameplayGroup = GetOrCreateCanvasGroup(_gameplayTipText);
            var locationGroup = GetOrCreateCanvasGroup(_locationText);

#pragma warning disable CS4014 // We do *not* want to await TweenManager.Run calls here
            // Initial state; BG full black, text hidden
            _background.color = _background.color.WithA(1f);
            gameplayGroup.alpha = locationGroup.alpha = 0f;

            // delay 1
            await UniTask.Delay(TimeSpan.FromSeconds(_initialDelay));

            _locationText.text = $"{_locationController.GetFullLocation()} DRIVE";

            // Type-in effect
            await UniTask.WhenAll(
                TypeInEffect(_gameplayTipText),
                TypeInEffect(_locationText));

            // delay 2
            await UniTask.Delay(TimeSpan.FromSeconds(_initialDelay));
            await UniTask.Delay(TimeSpan.FromSeconds(_initialDelay));

            _musicLoop.DisableLowPass(_animationLength);
            _inputController.Enable();
            _locationController.OnLocationEnter += OnLocationEnter;

            // fade out background
            _tweenManager.Run(1f, 0f, _animationLength, a => _background.color = _background.color.WithA(a), Easer.InCubic);

            await UniTask.Delay(TimeSpan.FromSeconds(_animationOffset));

            // slowly fade out location, then tip
            _tweenManager.Run(1f, 0f, _animationLength, a => locationGroup.alpha = a, Easer.InSine);
            _tweenManager.Run(0f, -400f, _animationLength, x => _locationText.rectTransform.anchoredPosition = _locationText.rectTransform.anchoredPosition.WithX(x), Easer.InCubic);

            await UniTask.Delay(TimeSpan.FromSeconds(_animationOffset));

            _tweenManager.Run(1f, 0f, _animationLength, a => gameplayGroup.alpha = a, Easer.InSine);
            _tweenManager.Run(0, -400f, _animationLength, x => _gameplayTipText.rectTransform.anchoredPosition = _gameplayTipText.rectTransform.anchoredPosition.WithX(x), Easer.InCubic);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        private void OnLocationEnter(string obj)
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = new();
            NewLocationAsync(_cts.Token).Forget();
        }

        private async UniTask NewLocationAsync(CancellationToken cancellationToken = default)
        {
            var locationGroup = GetOrCreateCanvasGroup(_locationText);

            locationGroup.alpha = 1f;
            _locationText.rectTransform.anchoredPosition = _locationText.rectTransform.anchoredPosition.WithX(0);
            _locationText.text = _locationController.GetFullLocation();

            if (cancellationToken.IsCancellationRequested) return;

            // Type-in effect
            await TypeInEffect(_locationText, cancellationToken);

            if (cancellationToken.IsCancellationRequested) return;

            // delay
            await UniTask.Delay(TimeSpan.FromSeconds(_initialDelay), cancellationToken: cancellationToken);

            if (cancellationToken.IsCancellationRequested) return;

            // slowly fade out location
            var tweenA = _tweenManager.Run(1f, 0f, _animationLength, a => locationGroup.alpha = a, Easer.InSine);
            var tweenB = _tweenManager.Run(0f, -400f, _animationLength, x => _locationText.rectTransform.anchoredPosition = _locationText.rectTransform.anchoredPosition.WithX(x), Easer.InCubic);

            // THIS IS NOT #1 VICTORY ROYALE
            var t = 0f;
            while (t <= _animationLength)
            {
                await UniTask.Yield();

                if (cancellationToken.IsCancellationRequested)
                {
                    tweenA.Cancel();
                    tweenB.Cancel();
                    return;
                }

                t += Time.deltaTime;
            }
        }

        private async UniTask TypeInEffect(TMP_Text tmp, CancellationToken cancellationToken = default)
        {
            var characterDelay = TimeSpan.FromSeconds(1 / (_charactersPerMinute / 60));

            var text = tmp.text;
            tmp.SetText(string.Empty);
            GetOrCreateCanvasGroup(tmp).alpha = 1f;

            if (cancellationToken.IsCancellationRequested) return;

            for (var i = 0; i < text.Length; i++)
            {
                if (cancellationToken.IsCancellationRequested) return;

                var character = text[i];

                tmp.SetText(tmp.text + character);

                if (char.IsLetter(character))
                {
                    // play random mechanical keyboard sfx
                    _audioPool.Play(_typeSfx[UnityEngine.Random.Range(0, _typeSfx.Length)]);
                }

                await UniTask.Delay(characterDelay);
            }
        }

        private CanvasGroup GetOrCreateCanvasGroup(MonoBehaviour comp)
        {
            if (!comp.TryGetComponent<CanvasGroup>(out var group))
            {
                group = comp.gameObject.AddComponent<CanvasGroup>();
            }

            return group;
        }

        private void OnDestroy()
        {
            _locationController.OnLocationEnter -= OnLocationEnter;
        }
    }
}
