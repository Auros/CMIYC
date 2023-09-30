using AuraTween;
using CMIYC.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CMIYC.Settings
{
    public class PauseController : MonoBehaviour, CacheInput.IPauseActions
    {
        private const float _settingsDuration = 0.5f;

        [SerializeField]
        private TweenManager _tweenManager = null!;

        [SerializeField]
        private RectTransform _backgroundImage = null!;

        [SerializeField]
        private RectTransform _menuPanel = null!;

        private bool _paused;
        private CursorLockMode _cachedCursorLockMode;
        private float _cachedTimeScale;

        private CacheInput _input = null;

        private void Start()
        {
            _input = new();
            _input.Pause.AddCallbacks(this);
            _input.Pause.Enable();
        }

        public void OnPause(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            TogglePause();
        }

        public void TogglePause()
        {
            _paused = !_paused;

            // Cache cursor lock mode on pause
            if (_paused)
            {
                _cachedCursorLockMode = Cursor.lockState;
                Cursor.lockState = CursorLockMode.None;

                _cachedTimeScale = Time.timeScale;
                Time.timeScale = 0;

                PresentPauseMenu();
            }
            // Restore cursor lock mode on unpause
            else
            {
                Cursor.lockState = _cachedCursorLockMode;
                Time.timeScale = _cachedTimeScale;
                HidePauseMenu();
            }
        }

        private void PresentPauseMenu()
        {
            // Background imagine sliding from the left
            _tweenManager.Run(-0.1f, 1, _settingsDuration, t => _backgroundImage.anchorMax = _backgroundImage.anchorMax.WithX(t), Easer.OutCubic);

            // Settings panel bounce animation
            _tweenManager.Run(0, 1, _settingsDuration, t => _menuPanel.localScale = _menuPanel.localScale.WithY(t), Easer.OutBounce);
            _tweenManager.Run(0, 1, _settingsDuration, t => _menuPanel.localScale = _menuPanel.localScale.WithX(t), Easer.OutBack);
        }

        private void HidePauseMenu()
        {
            // Background imagine sliding from the left
            _tweenManager.Run(1, -0.1f, _settingsDuration, t => _backgroundImage.anchorMax = _backgroundImage.anchorMax.WithX(t), Easer.InCubic);

            // Settings panel bounce animation
            _tweenManager.Run(1, 0, _settingsDuration, t => _menuPanel.localScale = _menuPanel.localScale.WithY(t), Easer.OutBounce);
            _tweenManager.Run(1, 0, _settingsDuration, t => _menuPanel.localScale = _menuPanel.localScale.WithX(t), Easer.InBack);
        }
    }
}
