using System;
using AuraTween;
using CMIYC.Input;
using CMIYC.Projectiles;
using CMIYC.Weapons;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CMIYC.Player
{
    public class PlayerWeaponManager : MonoBehaviour, CacheInput.IShootingActions
    {
        private static readonly Vector2 _crosshairPosition = new(0.5f, 0.5f);
        private const float _crosshairMaxDistance = 100f;

        public int Ammo { get; private set; }

        [field: SerializeField]
        public WeaponDefinition CurrentWeaponInstance { get; private set; }

        [SerializeField]
        private ProjectileDefinition _currentProjectilePrefab;

        [SerializeField]
        private TweenManager _tweenManager;

        [SerializeField]
        private Camera _mainCamera;

        [SerializeField]
        private Transform _weaponRoot;

        private bool _reloading;
        private CacheInput _input;

        public void OnShoot(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            if (CurrentWeaponInstance == null || _currentProjectilePrefab == null) return;

            // Early return if we are out of ammo.
            if (Ammo <= 0)
            {
                // Perform our reload animation if we can.
                if (!_reloading)
                {
                    ReloadAsync().Forget();
                }


                return;
            }

            Ammo--;
            var projectilePosition = CurrentWeaponInstance.ProjectileEmitPoint.position;
            FireProjectile(_currentProjectilePrefab, projectilePosition);
        }

        private async UniTask ReloadAsync()
        {
            const float reappearAnimationLength = 1f;

            _reloading = true;

            // Throw the weapon if ammo is empty
            var selfProjectile = CurrentWeaponInstance.SelfProjectile;
            if (selfProjectile != null)
            {
                FireProjectile(selfProjectile, CurrentWeaponInstance.transform.position);
            }

            // ...For the time being, shove our weapon into the ground.
            _weaponRoot.localPosition = 10f * Vector3.down;

            // Wait to bring our weapon back up.
            await UniTask.Delay(TimeSpan.FromSeconds(CurrentWeaponInstance.ReloadTime - reappearAnimationLength));

            // Bring our weapon back up and finish reloading.
            await _tweenManager.Run(1f, 0f, reappearAnimationLength,
                (t) => _weaponRoot.localPosition = t * Vector3.down, Easer.OutCubic, this);

            _reloading = false;
            Ammo = CurrentWeaponInstance.InitialAmmo;
        }

        // Instantiates and fires "projectilePrefab" from startingWorldPosition, in the direction of the crosshair.
        private void FireProjectile(ProjectileDefinition projectilePrefab, Vector3 startingWorldPosition)
        {
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

            // Calculate a forward vector based off the raycast
            var projectileForward = crosshairWorldPosition - startingWorldPosition;

            // Emit a new projectile at the weapon emission point, and let it loose.
            // TODO(Caeden): Consider moving this to the Weapon so that the Weapon is the source of damage, not this manager.
            var newProjectile = Instantiate(projectilePrefab);
            newProjectile.Initialize(startingWorldPosition, projectileForward, OnProjectileHit);
        }

        // TODO
        private void OnProjectileHit(ProjectileHitEvent hitEvent)
        {
        }

        private void Start()
        {
            if (CurrentWeaponInstance != null)
            {
                Ammo = CurrentWeaponInstance.InitialAmmo;
            }

            _input = new();
            _input.Shooting.AddCallbacks(this);
            _input.Shooting.Enable();
        }
    }
}
