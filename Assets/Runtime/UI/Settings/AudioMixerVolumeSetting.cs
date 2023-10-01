using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace CMIYC.UI.Settings
{
    public class AudioMixerVolumeSetting : MonoBehaviour
    {
        [SerializeField]
        private AudioMixer _audioMixer;

        [SerializeField]
        private string _audioMixerFloatID = string.Empty;

        [SerializeField]
        private float _maxVol;

        [SerializeField]
        private Slider _slider;

        public void OnSliderValueChanged(float newValue)
            => _audioMixer.SetFloat(_audioMixerFloatID, Mathf.Lerp(-80, _maxVol, newValue));
    }
}
