using System;
using CMIYC.Items;
using CMIYC.Projectiles;
using UnityEngine;

namespace CMIYC
{
    public class PlayerHealthController : MonoBehaviour, IProjectileTarget, IItemPickerUpper
    {
        public event Action PlayerTookDamage;
        public event Action PlayerTookJpegDamage;

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

            // ???????
            if (hitEvent.Instance.gameObject.name == "_JpgProjectile_(Clone)")
            {
                PlayerTookJpegDamage?.Invoke();
            }

            if (Health <= 0)
            {
                Health = 0;
                _deathController.Die();
            }
        }

        public void OnItemPickup(ItemPickupEvent pickupEvent)
        {
            if (pickupEvent.Instance is not HealthItemDefinition healthItemDefinition)
                return;

            Health += healthItemDefinition.Health;
            if (Health > InitialHealth)
            {
                Health = InitialHealth;
            }

            PlayerTookDamage?.Invoke();
        }
    }
}
