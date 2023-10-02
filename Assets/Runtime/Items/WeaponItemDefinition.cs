using CMIYC.Weapons;
using UnityEngine;

namespace CMIYC.Items
{
    public class WeaponItemDefinition : ItemDefinition
    {
        [Tooltip("Prefab to use for weapon")]
        [field: SerializeField] public WeaponDefinition Weapon { get; private set; }
    }
}
