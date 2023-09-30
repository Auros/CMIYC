using CMIYC.Input;
using CMIYC.Projectiles;
using CMIYC.Weapons;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CMIYC.Player
{
    public class PlayerWeaponManager : MonoBehaviour, CacheInput.IShootingActions
    {
        public WeaponDefinition CurrentWeaponInstance { get; private set; }

        private CacheInput _input;
        private ProjectileDefinition _currentProjectilePrefab;

        public void OnShoot(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            if (CurrentWeaponInstance == null || _currentProjectilePrefab == null) return;

            var emitPoint = CurrentWeaponInstance.ProjectileEmitPoint;

            // Emit a new projectile at the weapon emission point, and let it loose.
            // TODO(Caeden): Consider moving this to the Weapon so that the Weapon is the source of damage, not this manager.
            // TODO(Caeden): Forward vector should point to the cursor position on screen.
            var newProjectile = Instantiate(_currentProjectilePrefab);
            newProjectile.Initialize(emitPoint.position, emitPoint.forward, OnProjectileHit);
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
