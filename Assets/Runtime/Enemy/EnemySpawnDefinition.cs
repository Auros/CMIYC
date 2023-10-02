using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CMIYC.Enemy
{
    public class EnemySpawnDefinition : MonoBehaviour
    {
        [field: Tooltip("All potential spawn transforms")]
        [field: SerializeField]
        public List<Transform> SpawnPoints { get; private set; } = new();

        [field: Header ("Spawn Settings")]
        [field: Tooltip("The minimum amount of spawn transforms that will be used. Set to -1 for no minimum")]
        [field: SerializeField]
        public int MinSpawnCount { get; private set; } = 1;

        [field: Tooltip("The maximum amount of spawn transforms that will be used. Set to -1 to set to SpawnPoints.Length")]
        [field: SerializeField]
        public int MaxSpawnCount { get; private set; } = -1;

        [field: Tooltip("Percentage chance 0-1 for each spawn point to spawn an enemy")]
        [field: SerializeField]
        public float SpawnChance { get; private set; } = 0.5f;

        [field: Header("Enemies / Spawn Weight")]
        [field: SerializeField]
        public List<EnemySpawnChance> SpawnedEnemies { get; set; } = new();
    }
}
