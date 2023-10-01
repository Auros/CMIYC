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

        public void Die()
        {
            _musicLoop.EnableLowPass(2f);
            _inputController.Disable();

            Cursor.lockState = CursorLockMode.None;

            OnPlayerDeath?.Invoke();
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.K)) Die();
        }
#endif
    }
}
