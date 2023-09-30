using CMIYC.Input;
using CMIYC.Projectiles;
using CMIYC.Weapons;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CMIYC.Player
{
    public class PlayerWeaponManager : MonoBehaviour, CacheInput.IShootingActions
    {
        private static readonly Vector2 _crosshairPosition = new(0.5f, 0.5f);
        private const float _crosshairMaxDistance = 100f;

        [field: SerializeField]
        public WeaponDefinition CurrentWeaponInstance { get; private set; }

        [SerializeField]
        private ProjectileDefinition _currentProjectilePrefab;

        [SerializeField]
        private Camera _mainCamera;

        private CacheInput _input;

        public void OnShoot(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            if (CurrentWeaponInstance == null || _currentProjectilePrefab == null) return;

            // Early return if we are out of ammo.
            if (CurrentWeaponInstance.Ammo <= 0)
            {
                // Throw the weapon if ammo is empty
                var selfProjectile = CurrentWeaponInstance.SelfProjectile;
                if (selfProjectile != null)
                {
                    Destroy(CurrentWeaponInstance.gameObject);
                    FireProjectile(selfProjectile, CurrentWeaponInstance.transform.position);
                }

                return;
            }

            CurrentWeaponInstance.Ammo--;
            var projectilePosition = CurrentWeaponInstance.ProjectileEmitPoint.position;
            FireProjectile(_currentProjectilePrefab, projectilePosition);
        }

        // Instantiates and fires "projectilePrefab" from startingWorldPosition, in the direction of the crosshair.
        private void FireProjectile(ProjectileDefinition projectilePrefab, Vector3 startingWorldPosition)
        {
            // Convert our 0-1 crosshair position to screen space (0,0 to screen width/height)
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
            _input = new();
            _input.Shooting.AddCallbacks(this);
            _input.Shooting.Enable();
        }
    }
}
