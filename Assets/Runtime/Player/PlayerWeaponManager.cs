using System;
using AuraTween;
using CMIYC.Input;
using CMIYC.Items;
using CMIYC.Weapons;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CMIYC.Player
{
    public class PlayerWeaponManager : MonoBehaviour, CacheInput.IShootingActions, IItemPickerUpper
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

        [SerializeField]
        private DeathController _deathController = null!;

        [SerializeField]
        private LayerMask _crosshairRaycastMask = default;

        private bool _inputShooting = false;

        private void Update()
        {
            if (!_inputShooting) return;

            if (CurrentWeaponInstance == null) return;

            // Convert our 0-1 crosshair constant to screen space (0,0 to screen width,height)
            var crosshairScreenSpace = new Vector3(
                _mainCamera.pixelWidth * _crosshairPosition.x,
                _mainCamera.pixelHeight * _crosshairPosition.y, 0);

            // Convert to world space to determine where our projectile should travel
            var crosshairRay = _mainCamera.ScreenPointToRay(crosshairScreenSpace);

            // Raycast against everything to see where our projectiles *should* be directed
            var crosshairWorldPosition = Physics.Raycast(crosshairRay, out var raycastHit, _crosshairMaxDistance, _crosshairRaycastMask)
                ? raycastHit.point
                : crosshairRay.GetPoint(_crosshairMaxDistance);

            // Shoot our weapon at the target
            //   Perform our reload animation if we otherwise cannot fire
            if (!CurrentWeaponInstance.Reloading && !CurrentWeaponInstance.Shoot(crosshairWorldPosition))
            {
                // not reloadable
                if (CurrentWeaponInstance.ReloadTime < 0)
                {
                    SwitchWeaponAsync().Forget();
                    RunItBack();
                    return;
                }

                ReloadAsync().Forget();
            }
        }

        public void OnShoot(InputAction.CallbackContext context)
        {
            if (context.performed)
                _inputShooting = true;
            else if (context.canceled)
                _inputShooting = false;
        }

        public void OnThrow(InputAction.CallbackContext context)
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
            var crosshairWorldPosition = Physics.Raycast(crosshairRay, out var raycastHit, _crosshairMaxDistance, _crosshairRaycastMask)
                ? raycastHit.point
                : crosshairRay.GetPoint(_crosshairMaxDistance);

            if (!CurrentWeaponInstance.Reloading)
            {
                CurrentWeaponInstance.Throw(crosshairWorldPosition);

                // not reloadable
                if (CurrentWeaponInstance.ReloadTime < 0)
                {
                    SwitchWeaponAsync().Forget();
                    RunItBack();
                    return;
                }

                ReloadAsync().Forget();
            }
        }

        // for deletign the current weapon (one that doesnt reload) and going to the previous weapon
        public void RunItBack()
        {
            DestroyImmediate(CurrentWeaponInstance.gameObject);
            if (_weaponRoot.childCount <= 0)
            {
                Debug.LogError("UHHH THERE ARE NO WEAPONS TO USE CHIEF.");
                return;
            }

            var weapon = _weaponRoot.GetChild(_weaponRoot.childCount - 1);
            weapon.gameObject.SetActive(true);
            CurrentWeaponInstance = weapon.GetComponent<WeaponDefinition>();
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

        private async UniTask SwitchWeaponAsync()
        {
            const float reappearAnimationLength = 1f;

            // ...For the time being, shove our weapon into the ground.
            _weaponRoot.localPosition = 10f * Vector3.down;

            // Bring our weapon back up and finish reloading.
            await _tweenManager.Run(1f, 0f, reappearAnimationLength,
                (t) => _weaponRoot.localPosition = t * Vector3.down, ModifiedBackOut, this);
        }

        private void Start()
        {
            _inputController.Input.Shooting.AddCallbacks(this);

            _deathController.OnPlayerDeath += OnPlayerDeath;
        }

        private void OnPlayerDeath() => Destroy(CurrentWeaponInstance.gameObject);

        private void OnDestroy()
        {
            _deathController.OnPlayerDeath -= OnPlayerDeath;
        }

        private static float ModifiedBackOut(ref float time)
        {
            const float backOvershoot = 0.5f;

            var newTime = time - 1f;
            return newTime * newTime * ((backOvershoot + 1) * newTime + backOvershoot) + 1;
        }

        public void OnItemPickup(ItemPickupEvent pickupEvent)
        {
            if (pickupEvent.Instance is not WeaponItemDefinition weaponItemDefinition)
                return;

            // GET RID OF THE WEAPON / reload it

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

            CurrentWeaponInstance.ThrowReloadInstant(crosshairWorldPosition);
            SwitchWeaponAsync().Forget();

            // if same type of weapon, thats all folks.
            if (weaponItemDefinition.Weapon.name == CurrentWeaponInstance.name)
            {
                return;
            }

            // fuck
            CurrentWeaponInstance.gameObject.SetActive(false);
            var newWeaponInstance = Instantiate(weaponItemDefinition.Weapon, _weaponRoot);
            CurrentWeaponInstance = newWeaponInstance;
        }
    }
}
