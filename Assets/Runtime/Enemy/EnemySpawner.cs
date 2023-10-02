using System;
using System.Collections.Generic;
using CMIYC.Metadata;
using UnityEngine;

namespace CMIYC.Enemy
{
    public class EnemySpawner : MonoBehaviour
    {
        [field: Tooltip("All potential spawn transforms")]
        [field: SerializeField]
        public List<Transform> SpawnPoints { get; private set; } = new();

        [field: Header("Spawn Counts")]
        [field: Tooltip("Minimum amount of enemies that will spawn")]
        [field: SerializeField]
        public int MinEnemies { get; private set; } = 0;

        [field: Tooltip("Maximum amount of enemies that will spawn (will not go above SpawnPoints.Length)")]
        [field: SerializeField]
        public int MaxEnemies { get; private set; } = 1;

        [field: Header("IMPORTANT: if you override default enemy type settings, you will need")]
        [field: Header("to manually assign metadata for each enemy type you spawn!")]
        [field: Tooltip("Override default enemy settings")]
        [field: SerializeField]
        public bool OverrideDefaultSpawns { get; private set; } = false;

        [field: Header("All settings beyond this point only apply if above bool is true")]
        [field: SerializeField]
        public List<EnemySpawnChance> SpawnedEnemies { get; set; } = new();

        [field: Header("If metadata is not set on a spawned enemy type, default enemies (all) will be used")]
        [field: SerializeField]
        public TxtMetadataScriptableObjectInstance[] TxtMetadata { get; set; } = Array.Empty<TxtMetadataScriptableObjectInstance>();
        [field: SerializeField]
        public PngMetadataScriptableObjectInstance[] PngMetadata { get; set; } = Array.Empty<PngMetadataScriptableObjectInstance>();
        [field: SerializeField]
        public JpgMetadataScriptableObjectInstance[] JpgMetadata { get; set; } = Array.Empty<JpgMetadataScriptableObjectInstance>();
        [field: SerializeField]
        public FbxMetadataScriptableObjectInstance[] FbxMetadata { get; set; } = Array.Empty<FbxMetadataScriptableObjectInstance>();
    }
}
