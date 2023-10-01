using CMIYC.Projectiles;
using UnityEngine;

namespace CMIYC
{
    public class PlayerHealthController : MonoBehaviour, IProjectileTarget
    {
        [field: SerializeField]
        public float InitialHealth { get; private set; } = 100f;

        public float Health { get; private set; }

        [SerializeField]
        private DeathController _deathController = null!;

        private void Start() => Health = InitialHealth;

        public void OnProjectileHit(ProjectileHitEvent hitEvent)
        {
            Health -= hitEvent.Instance.Damage;

            if (Health < 0) _deathController.Die();
        }
    }
}
