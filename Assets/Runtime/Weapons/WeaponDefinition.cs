﻿using System;
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

        [field: Tooltip("The time it takes to reload this weapon.")]
        [field: SerializeField]
        public float ReloadTime { get; set; } = 1f;

        // Current ammo of this weapon
        public int Ammo { get; private set; }

        // Reload state of the weapon
        public bool Reloading { get; private set; }

        private void Start() => Ammo = InitialAmmo;

        public bool Shoot(Vector3 target)
        {
            if (Reloading) return false;

            if (Ammo == 0)
            {
                FireProjectile(SelfProjectile, target);
                ReloadAsync().Forget();
                return false;
            }

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

            return true;
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

        private async UniTask ReloadAsync()
        {
            Reloading = true;

            await UniTask.Delay(TimeSpan.FromSeconds(ReloadTime));
            Ammo = InitialAmmo;

            Reloading = false;
        }
    }
}
