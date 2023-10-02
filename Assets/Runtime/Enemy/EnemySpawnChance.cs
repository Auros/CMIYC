using System;
using UnityEngine;

namespace CMIYC.Enemy
{
    [Serializable]
    public class EnemySpawnChance
    {
        [field: SerializeField]
        public EnemyScriptableObject Enemy { get; set; }

        [field: SerializeField]
        public float SpawnWeight { get; set; } = 1f;
    }
}
