using System;
using AuraTween;
using CMIYC.Input;
using CMIYC.Projectiles;
using CMIYC.Weapons;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;

namespace CMIYC.Player
{
    public class PlayerWeaponManager : MonoBehaviour, CacheInput.IShootingActions
    {
        private static readonly Vector2 _crosshairPosition = new(0.5f, 0.5f);
        private const float _crosshairMaxDistance = 100f;

        [field: SerializeField]
        public WeaponDefinition CurrentWeaponInstance { get; private set; }

        [SerializeField]
        private TweenManager _tweenManager = null!;

        [SerializeField]
        private Camera _mainCamera = null!;

        [SerializeField]
        private Transform _weaponRoot = null!;

        [SerializeField]
        private InputController _inputController = null!;

        public void OnShoot(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            if (CurrentWeaponInstance == null) return;

            // Convert our 0-1 crosshair constant to screen space (0,0 to screen width,height)
            var crosshairScreenSpace = new Vector3(
                _mainCamera.pixelWidth * _crosshairPosition.x,
                _mainCamera.pixelHeight * _crosshairPosition.y, 0);

            // Convert to world space to determine where our projectile should travel
            var crosshairRay = _mainCamera.ScreenPointToRay(crosshairScreenSpace);

            // Raycast against everything to see where our projectiles *should* be directed
            var crosshairWorldPosition = Physics.Raycast(crosshairRay, out var raycastHit, _crosshairMaxDistance)
                ? raycastHit.point
                : crosshairRay.GetPoint(_crosshairMaxDistance);

            // Shoot our weapon at the target
            //   Perform our reload animation if we otherwise cannot fire
            if (!CurrentWeaponInstance.Reloading && !CurrentWeaponInstance.Shoot(crosshairWorldPosition))
            {
                ReloadAsync().Forget();
            }
        }

        // This reload method differs from WeaponDefinition.
        // This reload method is strictly for the view model animation, the WeaponDefinition impl actually affects weapon stats.
        private async UniTask ReloadAsync()
        {
            const float reappearAnimationLength = 1f;

            // ...For the time being, shove our weapon into the ground.
            _weaponRoot.localPosition = 10f * Vector3.down;

            // Wait to bring our weapon back up.
            await UniTask.Delay(TimeSpan.FromSeconds(CurrentWeaponInstance.ReloadTime - reappearAnimationLength));

            // Bring our weapon back up and finish reloading.
            await _tweenManager.Run(1f, 0f, reappearAnimationLength,
                (t) => _weaponRoot.localPosition = t * Vector3.down, ModifiedBackOut, this);
        }

        private void Start()
        {
            _inputController.Input.Shooting.AddCallbacks(this);
        }

        private static float ModifiedBackOut(ref float time)
        {
            const float backOvershoot = 0.5f;

            var newTime = time - 1f;
            return newTime * newTime * ((backOvershoot + 1) * newTime + backOvershoot) + 1;
        }
    }
}
