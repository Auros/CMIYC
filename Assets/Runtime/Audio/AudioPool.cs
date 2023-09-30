using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

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

                foreach (var source in _activeSources)
                {
                    source.volume = _volume;
                }

                foreach (var source in _pooledSources)
                {
                    source.volume = _volume;
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
        private Stack<AudioSource> _pooledSources = new();

        public void Play(AudioClip clip)
        {
            var source = ReuseOrCreateSource();

            source.pitch = _pitchBase + Random.Range(-_pitchRandomness, _pitchRandomness);
            source.clip = clip;

            _activeSources.Add(source);
            source.Play();
        }

        private void Start()
        {
            for (var i = 0; i < _initialPoolSize; i++)
            {
                _pooledSources.Push(CreateNewAudioSource());
            }
        }

        private void Update()
        {
            for (var i = 0; i < _activeSources.Count; i++)
            {
                var source = _activeSources[i];

                if (!source.isPlaying)
                {
                    _activeSources.RemoveAt(i);
                    _pooledSources.Push(source);
                    i--;
                }
            }
        }

        private AudioSource ReuseOrCreateSource()
        {
            if (!_pooledSources.TryPop(out var res))
            {
                res = CreateNewAudioSource();
            }

            return res;
        }

        private AudioSource CreateNewAudioSource()
        {
            var source = gameObject.AddComponent<AudioSource>();
            source.volume = Volume;
            source.outputAudioMixerGroup = _sfxMixerGroup;
            return source;
        }
    }
}
