using CMIYC.Metadata;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Random = UnityEngine.Random;

namespace CMIYC.Enemy
{
    public class EnemyController : MonoBehaviour
    {
        [Tooltip("TXT File Metadata")]
        [field: SerializeField]
        public TXTMetadataScriptableObject[] TxtMetadata { get; private set; } = Array.Empty<TXTMetadataScriptableObject>();

        [SerializeField]
        private EnemySpawnDefinition _debugSpawnDefinition = null!;

        public void Start()
        {
            if (_debugSpawnDefinition != null)
            {
                Spawn(_debugSpawnDefinition);
            }
        }

        public void Spawn(EnemySpawnDefinition spawnDefinition)
        {
            if (spawnDefinition.SpawnPoints?.Count == 0)
            {
                throw new InvalidOperationException("Cannot spawn on an EnemySpawnDefinition with no points");
            }
            if (spawnDefinition.SpawnedEnemies?.Count == 0)
            {
                throw new InvalidOperationException("Cannot spawn on an EnemySpawnDefinition with no enemies");
            }

            var spawnPoints = PickSpawnPoints(spawnDefinition);
            Debug.Log(spawnPoints.Count);

            foreach (var spawnPoint in spawnPoints)
            {
                var randomEnemy = PickRandomEnemyType(spawnDefinition);
                Debug.Log($"enemy: {randomEnemy.EnemyTypeName}");
            }
        }

        private EnemyScriptableObject PickRandomEnemyType(EnemySpawnDefinition spawnDefinition)
        {
            var totalWeight = spawnDefinition.SpawnedEnemies.Sum(x => x.SpawnWeight);
            var weightedRandom = Random.Range(0f, totalWeight);

            var traversedWeight = 0f;
            foreach (var spawnedEnemy in spawnDefinition.SpawnedEnemies)
            {
                if (spawnedEnemy.SpawnWeight == 0)
                {
                    Debug.LogWarning($"Enemy {spawnedEnemy.Enemy.EnemyTypeName} has a weight of 0 in {spawnDefinition.gameObject.name}! This will cause unintended consequences");
                }

                var currentEnemyWeightedIndex = traversedWeight + spawnedEnemy.SpawnWeight;

                if (weightedRandom <= currentEnemyWeightedIndex)
                {
                    return spawnedEnemy.Enemy;
                }
                traversedWeight = currentEnemyWeightedIndex;
            }

            Debug.LogWarning("Couldn't select enemy with weighted logic, returning first");
            return spawnDefinition.SpawnedEnemies.First().Enemy;
        }

        private List<Transform> PickSpawnPoints(EnemySpawnDefinition spawnDefinition)
        {
            List<Transform> spawnPoints = new();
            foreach (var spawnPoint in spawnDefinition.SpawnPoints)
            {
                if (Random.Range(0f, 1f) >= spawnDefinition.SpawnChance)
                {
                    spawnPoints.Add(spawnPoint);
                }
            }

            // gross
            int breakOut = 0;
            while (spawnDefinition.MaxSpawnCount > -1 && spawnPoints.Count >= spawnDefinition.MaxSpawnCount && breakOut < 50)
            {
                breakOut++;
                spawnPoints.RemoveAt(Random.Range(0, spawnPoints.Count));
            }

            // loop while min spawn point isn't met and spawnPoints hasn't exhausted all points,
            while (spawnDefinition.MinSpawnCount > -1 && spawnPoints.Count != spawnDefinition.SpawnPoints.Count && spawnPoints.Count <= spawnDefinition.MinSpawnCount && breakOut < 50)
            {
                breakOut++;
                var randomSpawnPoint = spawnDefinition.SpawnPoints[Random.Range(0, spawnDefinition.SpawnPoints.Count)];
                if (spawnPoints.Contains(randomSpawnPoint)) continue;
                spawnPoints.Add(randomSpawnPoint);
            }

            return spawnPoints;
        }
    }
}
