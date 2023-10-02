using UnityEngine;

namespace CMIYC.Items
{
    public class HealthItemDefinition : ItemDefinition
    {
        [Tooltip("Amount to heal the player on pickup")]
        [field: SerializeField] public float Health { get; private set; }
    }
}
