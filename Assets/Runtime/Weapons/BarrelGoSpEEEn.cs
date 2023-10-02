using UnityEngine;

namespace CMIYC.Weapons
{
    public class BarrelGoSpEEEn : MonoBehaviour
    {
        [SerializeField]
        private float _fireTorqueStrength = 90;

        [SerializeField]
        private float _torqueFalloff = 15;

        private float _torque;

        public void OnBulletFire()
        {
            _torque = _fireTorqueStrength;
        }

        private void Update()
        {
            transform.localEulerAngles += Time.deltaTime * _torque * Vector3.back;

            _torque = Mathf.Max(0, _torque - (_torqueFalloff * Time.deltaTime));
        }
    }
}
