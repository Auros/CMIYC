using CMIYC.Metadata;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using AuraTween;
using CMIYC.Enemy.Behaviour;
using CMIYC.Input;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine.InputSystem.Controls;
using Random = UnityEngine.Random;

namespace CMIYC.Enemy
{
    public class EnemyController : MonoBehaviour, IInputReceiver
    {
        public event Action<EnemyBehaviour> OnEnemyDeath;

        [Tooltip("TXT File Metadata")]
        [field: SerializeField]
        public TxtMetadataScriptableObject[] TxtMetadata { get; private set; } = Array.Empty<TxtMetadataScriptableObject>();

        [Tooltip("PNG File Metadata")]
        [field: SerializeField]
        public PngMetadataScriptableObject[] PngMetadata { get; private set; } = Array.Empty<PngMetadataScriptableObject>();

        [SerializeField]
        private Transform _enemyContainer = null!;
        [SerializeField]
        private InputBroadcaster _inputBroadcaster = null!;
        [SerializeField]
        private List<EnemySpawnDefinition> _debugSpawnDefinitions = null!;
        [SerializeField]
        private EnemyTextPool _enemyTextPool = null!;
        [SerializeField]
        private TweenManager _tweenManager = null!;

        private const float _spawnOffset = 1.11f; // assuming spawn is at foot of enemy, how much height needs to be added

        // not the best place for this probably
        [SerializeField]
        private Camera _mainCamera = null!;

        private List<EnemyBehaviour> _spawnedEnemies = new();
        public void Start()
        {
            _inputBroadcaster.Register(this);

            if (_debugSpawnDefinitions != null && _debugSpawnDefinitions.Count > 0)
            {
                foreach (var spawnPosition in _debugSpawnDefinitions)
                {
                    Spawn(spawnPosition);
                }
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
                // Debug.Log($"enemy: {randomEnemy.EnemyTypeName}");
                SpawnEnemy(randomEnemy, spawnPoint);
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

        private void SpawnEnemy(EnemyScriptableObject enemy, Transform spawnPoint)
        {
            var enemyBehaviour = Instantiate(enemy.Prefab, _enemyContainer);
            enemyBehaviour.transform.position = spawnPoint.position + new Vector3(0f, _spawnOffset, 0f);
            enemyBehaviour.transform.localRotation = spawnPoint.localRotation; // ? is this even necessary?
            SetMetadata(enemyBehaviour, enemy);

            _spawnedEnemies.Add(enemyBehaviour);
        }

        private void SetMetadata(EnemyBehaviour enemyBehaviour, EnemyScriptableObject enemy)
        {
            enemyBehaviour.Setup(enemy.Health, _tweenManager, OnDeath);
            if (enemyBehaviour is TxtBehaviour txtBehaviour)
            {
                // TODO: Prevent same file from spawning twice in the same "chunk?"
                var metadata = RandomFromArray(TxtMetadata);
                txtBehaviour.SetMetadata(metadata, enemy, _mainCamera);
            }
            else if (enemyBehaviour is PngBehaviour pngBehaviour)
            {
                // TODO: Prevent same file from spawning twice in the same "chunk?"
                var metadata = RandomFromArray(PngMetadata);
                pngBehaviour.SetMetadata(metadata, enemy, _mainCamera);
            }
        }

        private T RandomFromArray<T>(T[] array)
        {
            return array[Random.Range(0, array.Length)];
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

        public void OnKeyPressed(KeyControl key)
        {
            var keyDisplayName = key.displayName;
            foreach (var enemy in _spawnedEnemies)
            {
                if (enemy is TxtBehaviour txtBehaviour)
                {
                    // old behaviour
                    // _enemyTextPool.SpawnText(txtBehaviour.transform, keyDisplayName).Forget();

                    // new behaviour
                    txtBehaviour.CreateTextProjectile(keyDisplayName);
                }
            }
            // do stuff
        }

        public void OnDeath(EnemyBehaviour enemy)
        {
            Debug.Log("Enemy is DEAD!");
            if (_spawnedEnemies.Contains(enemy)) _spawnedEnemies.Remove(enemy);

            OnEnemyDeath?.Invoke(enemy);

            Destroy(enemy.gameObject);
        }
    }
}
