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
using CMIYC.Audio;

namespace CMIYC.Enemy
{
    public class EnemyController : MonoBehaviour, IInputReceiver
    {
        public event Action<EnemyBehaviour> OnEnemyDeath;

        [SerializeField]
        private Transform _enemyContainer = null!;
        [SerializeField]
        private Transform _player = null!;
        [SerializeField]
        private InputBroadcaster _inputBroadcaster = null!;
        //[SerializeField]
        //private List<EnemySpawnDefinition> _debugSpawnDefinitions = null!;
        //[SerializeField]
        //private EnemyTextPool _enemyTextPool = null!;
        [SerializeField]
        private TweenManager _tweenManager = null!;
        [SerializeField]
        private MusicLoop _musicLoop = null!;
        [SerializeField]
        private PlayerHealthController _playerHealthController = null!;

        [field: SerializeField]
        public List<EnemySpawnChance> SpawnedEnemies { get; set; } = new();

        [field: SerializeField]
        public TxtMetadataScriptableObjectInstance[] DefaultTxtMetadata { get; set; } = Array.Empty<TxtMetadataScriptableObjectInstance>();
        [field: SerializeField]
        public PngMetadataScriptableObjectInstance[] DefaultPngMetadata { get; set; } = Array.Empty<PngMetadataScriptableObjectInstance>();
        [field: SerializeField]
        public JpgMetadataScriptableObjectInstance[] DefaultJpgMetadata { get; set; } = Array.Empty<JpgMetadataScriptableObjectInstance>();
        [field: SerializeField]
        public FbxMetadataScriptableObjectInstance[] DefaultFbxMetadata { get; set; } = Array.Empty<FbxMetadataScriptableObjectInstance>();

        private const float _spawnOffset = 1.11f; // assuming spawn is at foot of enemy, how much height needs to be added

        // not the best place for this probably
        [SerializeField]
        private Camera _mainCamera = null!;

        private List<EnemyBehaviour> _spawnedEnemies = new();
        public void Start()
        {
            _inputBroadcaster.Register(this);
        }

        public void Update()
        {
            // only update once every 10 frames
            if (Time.frameCount % 10 != 0) return;

            // the jank continues
            var playerPosition = _playerHealthController.Health > 0
                ? _player.position
                : -1000 * Vector3.down;
            foreach (var enemy in _spawnedEnemies)
            {
                enemy.UpdatePlayerPosition(playerPosition);
            }
        }

        public void Spawn(EnemySpawnData spawnData)
        {
            if (spawnData.SpawnPoints?.Count == 0)
            {
                throw new InvalidOperationException("Cannot spawn on an EnemySpawnDefinition with no points");
            }
            /*
            if (spawnDefinition.SpawnedEnemies?.Count == 0)
            {
                throw new InvalidOperationException("Cannot spawn on an EnemySpawnDefinition with no enemies");
            }*/

            var spawnPoints = PickSpawnPoints(spawnData);

            var spawnedEnemies = spawnData.OverrideDefaultSpawns ? spawnData.SpawnedEnemies : SpawnedEnemies;
            var enemyWeights = spawnedEnemies.Select(x => x.SpawnWeight).ToList();
            foreach (var spawnPoint in spawnPoints)
            {
                var randomEnemy = spawnedEnemies[IndexFromWeights(enemyWeights)].Enemy;
                SpawnEnemy(spawnData, randomEnemy, spawnPoint);
            }
        }

        private int IndexFromWeights(List<float> weights)
        {
            var totalWeight = weights.Sum(x => x);
            var weightedRandom = Random.Range(0f, totalWeight);

            var traversedWeight = 0f;
            for (int i = 0; i < weights.Count; i++)
            {
                var currentWeightedIndex = traversedWeight + weights[i];

                if (weightedRandom <= currentWeightedIndex)
                {
                    return i;
                }
                traversedWeight = currentWeightedIndex;
            }

            Debug.Log("Weighted random failed");
            return 0;
        }

        private void SpawnEnemy(EnemySpawnData spawnData, EnemyScriptableObject enemy, Transform spawnPoint)
        {
            var enemyBehaviour = Instantiate(enemy.Prefab, _enemyContainer);
            enemyBehaviour.transform.position = spawnPoint.position + new Vector3(0f, _spawnOffset, 0f);
            enemyBehaviour.transform.localRotation = spawnPoint.localRotation; // ? is this even necessary?
            SetMetadata(spawnData, enemyBehaviour, enemy);

            _spawnedEnemies.Add(enemyBehaviour);
        }

        private void SetMetadata(EnemySpawnData spawnData, EnemyBehaviour enemyBehaviour, EnemyScriptableObject enemy)
        {
            // TODO: Prevent same file from spawning twice in the same "chunk?"

            enemyBehaviour.Setup(enemy.Health, _tweenManager, OnDeath);
            if (enemyBehaviour is TxtBehaviour txtBehaviour)
            {
                var metadatas = spawnData.OverrideDefaultSpawns && spawnData.TxtMetadata.Length > 0 ? spawnData.TxtMetadata : DefaultTxtMetadata;
                var metadata = metadatas[IndexFromWeights(metadatas.Select(x => x.SpawnWeight).ToList())].Metadata;
                txtBehaviour.SetMetadata(metadata, enemy, _mainCamera);
            }
            else if (enemyBehaviour is PngBehaviour pngBehaviour)
            {
                var metadatas = spawnData.OverrideDefaultSpawns && spawnData.PngMetadata.Length > 0 ? spawnData.PngMetadata : DefaultPngMetadata;
                var metadata = metadatas[IndexFromWeights(metadatas.Select(x => x.SpawnWeight).ToList())].Metadata;
                pngBehaviour.SetMetadata(metadata, enemy, _mainCamera);
            }
            else if (enemyBehaviour is JpgBehaviour jpgBehaviour)
            {
                var metadatas = spawnData.OverrideDefaultSpawns && spawnData.JpgMetadata.Length > 0 ? spawnData.JpgMetadata : DefaultJpgMetadata;
                var metadata = metadatas[IndexFromWeights(metadatas.Select(x => x.SpawnWeight).ToList())].Metadata;
                jpgBehaviour.SetMetadata(metadata, enemy, _mainCamera);
            }
            else if (enemyBehaviour is FbxBehaviour fbxBehaviour)
            {
                var metadatas = spawnData.OverrideDefaultSpawns && spawnData.FbxMetadata.Length > 0 ? spawnData.FbxMetadata : DefaultFbxMetadata;
                var metadata = metadatas[IndexFromWeights(metadatas.Select(x => x.SpawnWeight).ToList())].Metadata;
                fbxBehaviour.SetMetadata(metadata, enemy, _mainCamera);
            }
        }

        private List<Transform> PickSpawnPoints(EnemySpawnData spawnData)
        {
            // im not doing more validation than this i will simply not have bad data
            var min = spawnData.MinEnemies > 0 ? spawnData.MinEnemies : 0;
            var max = spawnData.MaxEnemies < spawnData.SpawnPoints.Count ? spawnData.MaxEnemies : spawnData.SpawnPoints.Count;
            var roomsToGenerate = Random.Range(min, max + 1); // max is exclusive, might need to add one?

            List<Transform> spawnPoints = new();

            // gross
            int breakout = 0;
            while (spawnPoints.Count < roomsToGenerate && breakout < 300)
            {
                breakout++;
                var randomSpawnPoint = spawnData.SpawnPoints[Random.Range(0, spawnData.SpawnPoints.Count)];
                if (!spawnPoints.Contains(randomSpawnPoint)) spawnPoints.Add(randomSpawnPoint);
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

            if (_musicLoop != null)
            {
                _musicLoop.ResetRiff();
            }

            Destroy(enemy.gameObject);
        }
    }
}
