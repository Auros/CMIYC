using System;
using CMIYC.Projectiles;
using UnityEngine;

namespace CMIYC
{
    public class PlayerHealthController : MonoBehaviour, IProjectileTarget
    {
        public event Action PlayerTookDamage;

        [field: SerializeField]
        public float InitialHealth { get; private set; } = 100f;

        public float Health { get; private set; }

        [SerializeField]
        private DeathController _deathController = null!;

        private void Start() => Health = InitialHealth;

        public void OnProjectileHit(ProjectileHitEvent hitEvent)
        {
            if (Health <= 0) return;

            Health -= hitEvent.Instance.Damage;

            PlayerTookDamage?.Invoke();

            if (Health < 0)
            {
                Health = 0;
                _deathController.Die();
            }
        }
    }
}
