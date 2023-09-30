using UnityEngine;

namespace CMIYC.Projectiles
{
    public class DebugProjectileTarget : MonoBehaviour, IProjectileTarget
    {
        public void OnProjectileHit(ProjectileHitEvent hitEvent)
            => Debug.Log($"Ouch! I got hit for {hitEvent.Instance.Damage} damage.");
    }
}
