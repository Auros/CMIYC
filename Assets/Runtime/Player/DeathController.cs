using System;
using CMIYC.Audio;
using CMIYC.Input;
using UnityEngine;

namespace CMIYC
{
    public class DeathController : MonoBehaviour
    {
        public event Action OnPlayerDeath;

        [SerializeField]
        private MusicLoop _musicLoop = null!;

        [SerializeField]
        private InputController _inputController = null!;

        private bool _isDie = false;

        public void Die()
        {
            if (_isDie) return;
            _isDie = true;

            _musicLoop.EnableLowPass(2f);
            _inputController.Disable();

            Cursor.lockState = CursorLockMode.None;

            OnPlayerDeath?.Invoke();
        }

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.K)) Die();
        }
    }
}
