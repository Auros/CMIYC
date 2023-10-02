using UnityEngine;

namespace CMIYC.Player
{
    public class PlayerHealthRegenController : MonoBehaviour
    {
        [SerializeField]
        private PlayerHealthController _healthController;

        [SerializeField]
        private float _timeBeforeRegen = 5f;

        [SerializeField]
        private float _healthRegenPerSec = 10f;

        private float _t;

        private void Start()
        {
            _healthController.PlayerTookDamage += PlayerTookDamage;
        }

        private void Update()
        {
            _t += Time.deltaTime;

            if (_t >= _timeBeforeRegen)
            {
                _healthController.Heal(_healthRegenPerSec * Time.deltaTime);
            }
        }

        private void PlayerTookDamage()
        {
            _t = 0;
        }

        private void OnDestroy()
        {
            _healthController.PlayerTookDamage -= PlayerTookDamage;
        }
    }
}
