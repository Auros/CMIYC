using System;
using CMIYC.Audio;
using CMIYC.Projectiles;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CMIYC.Weapons
{
    // Used in the root of a prefab.
    // TODO:
    //   - Multiple projectile emission points
    //   - Multiple projectiles shot per click
    //   - Full auto firing
    //   - Burst fire?
    //   - Recoil?
    //   - Spread?
    public class WeaponDefinition : MonoBehaviour
    {
        [field: Tooltip("Transform where projectiles are instantiated and fired from (pointing to the cursor)")]
        [field: SerializeField]
        public Transform ProjectileEmitPoint { get; private set; }

        [field: Tooltip("The initial ammo capacity of the weapon.")]
        [field: SerializeField]
        public int InitialAmmo { get; set; }

        [field: Tooltip("Bullet projectile that is emitted from the Projectile Emit Point.")]
        [field: SerializeField]
        public ProjectileDefinition BulletProjectile { get; private set; }

        [field: Tooltip("If non-null and the magazine is empty, the weapon will throw this projectile at the player.")]
        [field: SerializeField]
        public ProjectileDefinition SelfProjectile { get; private set; }

        [field: Tooltip("Particle system to use for bullet shells")]
        [field: SerializeField]
        public ParticleSystem ShellParticles { get; private set; }

        [field: Tooltip("Particle system to use for muzzle flash")]
        [field: SerializeField]
        public ParticleSystem MuzzleFlashParticles { get; private set; }

        [field: Tooltip("The time between firing each bullet in seconds")]
        [field: SerializeField]
        public float FireSpeed { get; set; } = 0.1f;

        [field: Tooltip("The time it takes to reload this weapon.")]
        [field: SerializeField]
        public float ReloadTime { get; set; } = 1f;

        [field: Tooltip("The audiopool to use for weapon sound effects.")]
        [field: SerializeField]
        private AudioPool _audioPool = null!;

        [field: Tooltip("The audioclip to use for shooting sound effect.")]
        [field: SerializeField]
        private AudioClip _shootClip = null!;

        [field: Tooltip("The reloadclip to use for reloading sound effect")]
        [field: SerializeField]
        private AudioClip _reloadClip = null!;

        [field: Tooltip("The time to play the reload sound during the animation")]
        [field: SerializeField]
        private float ReloadSoundTime { get; set; } = 0.8f;

        // Current ammo of this weapon
        public int Ammo { get; private set; }

        // Reload state of the weapon
        public bool Reloading { get; private set; }

        // If a round is ready to be fired
        public bool RoundReady { get; private set; } = true;

        private void Start() => Ammo = InitialAmmo;

        public bool Shoot(Vector3 target)
        {
            if (Reloading) return false;
            if (!RoundReady) return true;

            if (Ammo == 0)
            {
                FireProjectile(SelfProjectile, target);
                if (ReloadTime >= 0)
                {
                    ReloadAsync().Forget();
                }
                return false;
            }

            FireCooldown().Forget();

            Ammo--;
            FireProjectile(BulletProjectile, target);

            if (ShellParticles != null)
            {
                ShellParticles.Emit(1);
            }

            if (MuzzleFlashParticles != null)
            {
                MuzzleFlashParticles.Play();
            }

            if (_audioPool != null)
            {
                if (_shootClip != null)
                {
                    _audioPool.Play(_shootClip);
                }
            }

            return true;
        }

        public void Throw(Vector3 target)
        {
            if (Reloading) return;
            FireProjectile(SelfProjectile, target);
            if (ReloadTime >= 0)
            {
                ReloadAsync().Forget();
            }

            if (_audioPool != null)
            {
                if (_reloadClip != null)
                {
                    ReloadSoundAsync().Forget();
                }
            }
        }

        public void ThrowReloadInstant(Vector3 target)
        {
            if (Reloading) return;
            FireProjectile(SelfProjectile, target);
            InstantReloadAsync().Forget();
        }

        private void FireProjectile(ProjectileDefinition projectile, Vector3 target)
        {
            if (projectile == null) return;

            // Calculate projectile direction from emission point
            var spawnPoint = ProjectileEmitPoint.position;
            var projectileForward = target - spawnPoint;

            // Emit a new projectile at the weapon emission point, and let it loose.
            var newProjectile = Instantiate(projectile);
            newProjectile.Initialize(spawnPoint, projectileForward);
        }

        private async UniTask FireCooldown()
        {
            RoundReady = false;

            await UniTask.Delay(TimeSpan.FromSeconds(FireSpeed));

            RoundReady = true;
        }

        private async UniTask InstantReloadAsync()
        {
            Reloading = true;

            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));

            if (_audioPool != null)
            {
                if (_reloadClip != null)
                {
                    _audioPool.Play(_reloadClip);
                }
            }

            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));

            Ammo = InitialAmmo;

            Reloading = false;
        }

        private async UniTask ReloadAsync()
        {
            Reloading = true;

            await UniTask.Delay(TimeSpan.FromSeconds(ReloadTime));
            Ammo = InitialAmmo;

            Reloading = false;
        }

        private async UniTask ReloadSoundAsync()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(ReloadSoundTime));
            _audioPool.Play(_reloadClip);
        }
    }
}
