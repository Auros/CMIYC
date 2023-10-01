using AuraTween;
using CMIYC.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CMIYC.UI.Settings
{
    /* TODO:
     * Okay, so I need to disable other sources of input during pausing, but currently every other class makes their own CacheInput instance.
     * 
     * That makes my life difficult.
     * 
     * I *could* make CacheInput a persistent singleton, so I can disable every other input action from PauseController itself,
     * however Auros will have my head. Although I genuinely think this approach makes more sense compared to having every single Input consumer
     * depend on PauseController and early return based on the Paused state.
     */
    public class PauseController : MonoBehaviour, CacheInput.IPauseActions
    {
        private const float _settingsDuration = 0.5f;

        public bool Paused { get; private set; }

        [SerializeField]
        private InputController _inputController = null!;

        [SerializeField]
        private TweenManager _tweenManager = null!;

        [SerializeField]
        private RectTransform _backgroundImage = null!;

        [SerializeField]
        private RectTransform _menuPanel = null!;

        private CursorLockMode _cachedCursorLockMode;
        private float _cachedTimeScale;

        private void Start()
        {
            HidePauseMenu(0);

            _inputController.Input.Pause.AddCallbacks(this);
        }

        public void OnPause(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            TogglePause();
        }

        public void Quit() => Application.Quit();

        public void TogglePause()
        {
            Paused = !Paused;

            // Cache cursor lock mode on pause
            if (Paused)
            {
                _cachedCursorLockMode = Cursor.lockState;
                Cursor.lockState = CursorLockMode.None;

                _cachedTimeScale = Time.timeScale;
                Time.timeScale = 0;

                PresentPauseMenu(_settingsDuration);

                _inputController.Disable(_inputController.Input.Pause.Pause);
            }
            // Restore cursor lock mode on unpause
            else
            {
                Cursor.lockState = _cachedCursorLockMode;
                Time.timeScale = _cachedTimeScale;
                _inputController.Enable();
                HidePauseMenu(_settingsDuration);
            }
        }

        private void PresentPauseMenu(float duration)
        {
            // Background imagine sliding from the left
            _tweenManager.Run(-0.1f, 1, duration, t => _backgroundImage.anchorMax = _backgroundImage.anchorMax.WithX(t), Easer.OutCubic);

            // Settings panel bounce animation
            _tweenManager.Run(0, 1, duration, t => _menuPanel.localScale = _menuPanel.localScale.WithY(t), Easer.OutBounce);
            _tweenManager.Run(0, 1, duration, t => _menuPanel.localScale = _menuPanel.localScale.WithX(t), Easer.OutBack);
        }

        private void HidePauseMenu(float duration)
        {
            // Background imagine sliding from the left
            _tweenManager.Run(1, -0.1f, duration, t => _backgroundImage.anchorMax = _backgroundImage.anchorMax.WithX(t), Easer.InCubic);

            // Settings panel bounce animation
            _tweenManager.Run(1, 0, duration, t => _menuPanel.localScale = _menuPanel.localScale.WithY(t), Easer.OutBounce);
            _tweenManager.Run(1, 0, duration, t => _menuPanel.localScale = _menuPanel.localScale.WithX(t), Easer.InBack);
        }
    }
}
