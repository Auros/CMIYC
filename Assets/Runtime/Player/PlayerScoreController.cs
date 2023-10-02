using System;
using CMIYC.Enemy;
using CMIYC.Enemy.Behaviour;
using UnityEngine;

namespace CMIYC.Player
{
    public class PlayerScoreController : MonoBehaviour
    {
        public event Action<int> OnScoreIncrease;

        public ulong Score { get; private set; }

        [SerializeField]
        private EnemyController _enemyController;

        private void Start()
        {
            _enemyController.OnEnemyDeath += OnEnemyDeath;
        }

        private void OnEnemyDeath(EnemyBehaviour obj)
        {
            var enemyScore = obj switch
            {
                // TXT: Length of metadata contents * hardcoded value to keep score comparable to png
                TxtBehaviour txt => txt.AssignedMetadata.TextContents.Length * 16,

                // PNG: width * height
                PngBehaviour png => png.AssignedMetadata.Texture.width * png.AssignedMetadata.Texture.height,

                // Fallback to random score
                _ => UnityEngine.Random.Range(50, 150)
            };

            Score += (ulong)enemyScore;

            OnScoreIncrease?.Invoke(enemyScore);
        }

        private void OnDestroy()
        {
            _enemyController.OnEnemyDeath -= OnEnemyDeath;
        }
    }
}
