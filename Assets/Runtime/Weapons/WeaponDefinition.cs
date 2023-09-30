using CMIYC.Projectiles;
using UnityEngine;

namespace CMIYC.Weapons
{
    // Used in the root of a prefab.
    public class WeaponDefinition : MonoBehaviour
    {
        [Tooltip("Transform where projectiles are instantiated and fired from (pointing to the cursor)")]
        [field: SerializeField]
        public Transform ProjectileEmitPoint { get; private set; }

        [Tooltip("The initial ammo capacity of the weapon.")]
        [field: SerializeField]
        public int Ammo { get; set; }

        [Tooltip("If non-null and the magazine is empty, the weapon will throw this projectile at the player.")]
        [field: SerializeField]
        public ProjectileDefinition SelfProjectile { get; private set; }
    }
}
