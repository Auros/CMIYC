using UnityEngine;
using UnityEngine.UI;

namespace CMIYC.UI
{
    public class HealthUIController : MonoBehaviour
    {
        [SerializeField]
        private Slider _slider = null!;

        [SerializeField]
        private PlayerHealthController _playerHealthController = null!;

        [SerializeField]
        private float _lerpStrength = 5f;

        private float _targetValue = 1f;

        private void Start()
        {
            _slider.value = _targetValue = 1f;

            _playerHealthController.PlayerTookDamage += PlayerTookDamage;
        }

        private void PlayerTookDamage()
        {
            _targetValue = _playerHealthController.Health / _playerHealthController.InitialHealth;
        }

        private void Update()
        {
            _slider.value = Mathf.Lerp(_slider.value, _targetValue, Time.deltaTime* _lerpStrength);
        }

        private void OnDestroy()
        {
            _playerHealthController.PlayerTookDamage -= PlayerTookDamage;
        }
    }
}
