using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Pool;

namespace CMIYC.Audio
{
    // Majority of the code was re-used from our previous Ludum Dare entry, Liver Die
    public class AudioPool : MonoBehaviour
    {
        public float Volume
        {
            get => _volume;
            set
            {
                _volume = value;

                for (var i = 0; i < _activeSources.Count; i++)
                {
                    _activeSources[i].volume = _volume;
                }
            }
        }

        [SerializeField]
        private AudioMixerGroup _sfxMixerGroup = null!;

        [SerializeField]
        private float _volume = 1f;

        [Space, SerializeField]
        private float _pitchBase = 1;

        [SerializeField]
        private float _pitchRandomness = 0;

        [Space, SerializeField]
        private int _initialPoolSize = 0;

        private List<AudioSource> _activeSources = new();
        private ObjectPool<AudioSource> _objectPool = null!;

        public void Play(AudioClip clip)
        {
            var source = _objectPool.Get();
            source.clip = clip;
            source.Play();

            _activeSources.Add(source);
        }

        private void Awake()
            => _objectPool = new ObjectPool<AudioSource>(
                CreateNewAudioSource,
                PrepareAudioSource,
                null, // Nothing special needed on release
                null, // Nothing special needed on destroy
                false,
                _initialPoolSize);

        private void Update()
        {
            for (var i = 0; i < _activeSources.Count; i++)
            {
                var source = _activeSources[i];

                if (source.isPlaying) continue;

                _activeSources.RemoveAt(i);
                _objectPool.Release(source);
                i--;
            }
        }

        private AudioSource CreateNewAudioSource()
        {
            var source = gameObject.AddComponent<AudioSource>();
            source.volume = Volume;
            source.outputAudioMixerGroup = _sfxMixerGroup;
            return source;
        }

        private void PrepareAudioSource(AudioSource source)
        {
            source.pitch = _pitchBase + Random.Range(-_pitchRandomness, _pitchRandomness);
            source.volume = _volume;
        }
    }
}
