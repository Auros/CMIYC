using UnityEngine;

namespace CMIYC.Enemy
{
    [CreateAssetMenu(fileName = "Enemy", menuName = "ScriptableObjects/EnemyScriptableObject", order = 1)]
    public class EnemyScriptableObject : ScriptableObject
    {
        public string EnemyTypeName = string.Empty;
        public float Health = 100;
    }
}
