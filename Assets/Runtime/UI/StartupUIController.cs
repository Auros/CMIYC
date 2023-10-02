using System;
using System.Diagnostics.CodeAnalysis;
using AuraTween;
using CMIYC.Audio;
using CMIYC.Input;
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

            // Type-in effect
            await UniTask.WhenAll(
                TypeInEffect(_gameplayTipText),
                TypeInEffect(_locationText));

            // delay 2
            await UniTask.Delay(TimeSpan.FromSeconds(_initialDelay));
            await UniTask.Delay(TimeSpan.FromSeconds(_initialDelay));

            _musicLoop.DisableLowPass(_animationLength);
            _inputController.Enable();

            // fade out background
            await _tweenManager.Run(1f, 0f, _animationLength, a => _background.color = _background.color.WithA(a), Easer.InCubic);

            // delay 3
            await UniTask.Delay(TimeSpan.FromSeconds(_initialDelay));

            // slowly fade out location, then tip
            _tweenManager.Run(1f, 0f, _animationLength, a => gameplayGroup.alpha = a, Easer.InSine);
            _tweenManager.Run(0, -400f, _animationLength, x => _gameplayTipText.rectTransform.anchoredPosition = _gameplayTipText.rectTransform.anchoredPosition.WithX(x), Easer.InCubic);

            await UniTask.Delay(TimeSpan.FromSeconds(_animationOffset));

            _tweenManager.Run(1f, 0f, _animationLength, a => locationGroup.alpha = a, Easer.InSine);
            _tweenManager.Run(0f, -400f, _animationLength, x => _locationText.rectTransform.anchoredPosition = _locationText.rectTransform.anchoredPosition.WithX(x), Easer.InCubic);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        private async UniTask TypeInEffect(TMP_Text tmp)
        {
            var characterDelay = TimeSpan.FromSeconds(1 / (_charactersPerMinute / 60));

            var text = tmp.text;
            tmp.SetText(string.Empty);
            GetOrCreateCanvasGroup(tmp).alpha = 1f;

            for (var i = 0; i < text.Length; i++)
            {
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
    }
}
