using UnityEngine;

namespace CMIYC.Weapons
{
    // Used in the root of a prefab.
    public class WeaponDefinition : MonoBehaviour
    {
        // Transform where projectiles are instantiated and fired from (pointing to the cursor)
        [field: SerializeField]
        public Transform ProjectileEmitPoint { get; private set; }
    }
}
