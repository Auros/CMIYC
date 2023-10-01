using CMIYC.Projectiles;
using TMPro;
using UnityEngine;

namespace CMIYC.Enemy.Behaviour
{
    public abstract class EnemyBehaviour : MonoBehaviour, IProjectileTarget
    {
        private bool _isAlive = true;

        private float _health;
        private Camera _cameraToLookAt;

        [SerializeField]
        private Transform _nameTag = null!;

        [SerializeField]
        private TMP_Text _nameText = null!;

        public void SetNameTagMetadata(string fileName, Camera cameraToLookAt)
        {
            _nameText.SetText(fileName);
            _cameraToLookAt = cameraToLookAt;
        }

        public void SetHealth(float health)
        {
            _health = health;
        }

        void Update()
        {
            if (_cameraToLookAt == null) return;

            _nameTag.LookAt(_cameraToLookAt.transform);
            _nameTag.localRotation = Quaternion.Euler(0, _nameTag.localRotation.eulerAngles.y + 180, 0);
        }

        public void OnProjectileHit(ProjectileHitEvent hitEvent)
        {
            if (!_isAlive) return;
            Debug.Log($"Hit enemy for {hitEvent.Instance.Damage} damage!");
            HandleHealthChange(hitEvent.Instance.Damage);
        }

        private void HandleHealthChange(float damage)
        {
            _health -= damage;
            if (_health < 0) _health = 0;

            if (_health == 0)
            {
                Debug.Log("you are dead lmao");
                _isAlive = false;
            }
        }
    }
}
