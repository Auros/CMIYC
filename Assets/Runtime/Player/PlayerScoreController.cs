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
            var enemyScore = obj.Size;

            Score += (ulong)enemyScore;

            OnScoreIncrease?.Invoke(enemyScore);
        }

        private void OnDestroy()
        {
            _enemyController.OnEnemyDeath -= OnEnemyDeath;
        }
    }
}
