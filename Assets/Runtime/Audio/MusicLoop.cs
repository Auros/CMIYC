using AuraTween;
using UnityEngine;
using UnityEngine.Audio;

namespace CMIYC.Audio
{
    public class MusicLoop : MonoBehaviour
    {
        [SerializeField]
        private AudioSource _guitarRiff;

        [SerializeField]
        private AudioSource _ambientMusic;

        [Tooltip("Lerp strength for guitar riff volume.")]
        [SerializeField]
        private float _fadeStrength = 5f;

        [Tooltip("Time that the player is allowed to be out of combat before the guitar riff fades.")]
        [SerializeField]
        private float _riffLength = 10f;

        [Tooltip("How long it takes for the guitar riff to fade out.")]
        [SerializeField]
        private float _riffFadeLength = 10f;

        [Header("Low pass filter")]
        [Space]
        [SerializeField]
        private TweenManager _tweenManager = null!;

        [SerializeField]
        private AudioMixer _audioMixer = null!;
        [SerializeField]
        private float _normalLowpassAmount = 22000f;
        [SerializeField]
        private float _pausedLowpassAmount = 377f;

        private float _targetVolume;
        private float _riffTime = 100f;

        public void ResetRiff() => _riffTime = 0;

        private void Update()
        {
            _riffTime += Time.deltaTime;

            _targetVolume = Mathf.Lerp(1, 0, (_riffTime - _riffLength) / _riffFadeLength);

            _guitarRiff.volume = Mathf.Lerp(_guitarRiff.volume, _targetVolume, Time.deltaTime * _fadeStrength);
        }

        public void EnableLowPass(float duration = 0f)
            => _tweenManager.Run(_normalLowpassAmount, _pausedLowpassAmount, duration, l => _audioMixer.SetFloat("LowPass", l), Easer.OutSine);

        public void DisableLowPass(float duration = 0f)
            => _tweenManager.Run(_pausedLowpassAmount, _normalLowpassAmount, duration, l => _audioMixer.SetFloat("LowPass", l), Easer.InSine);
    }
}
