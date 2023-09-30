using TMPro;
using UnityEngine;

namespace CMIYC.Enemy.Behaviour
{
    public abstract class EnemyBehaviour : MonoBehaviour
    {
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

        void Update()
        {
            if (_cameraToLookAt == null) return;

            _nameTag.LookAt(_cameraToLookAt.transform);
            _nameTag.localRotation = Quaternion.Euler(0, _nameTag.localRotation.eulerAngles.y + 180, 0);
        }
    }
}
