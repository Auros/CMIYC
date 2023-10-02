using TMPro;
using UnityEngine;

namespace CMIYC.Projectiles
{
    public class TextProjectile : MonoBehaviour
    {
        [field: SerializeField]
        public ProjectileDefinition ProjectileDefinition { get; set; } = null!;

        [field: SerializeField]
        public TMP_Text Text { get; set; } = null!;

        private Transform? _target;

        public void SetTarget(Transform target)
        {
            _target = target;
        }

        void Update()
        {
            if (_target == null) return;
            Text.transform.LookAt(_target);
            Text.transform.Rotate(0, 180, 0);
        }
    }
}
