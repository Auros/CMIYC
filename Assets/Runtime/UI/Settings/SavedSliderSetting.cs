using UnityEngine;
using UnityEngine.UI;

namespace CMIYC.UI.Settings
{
    public class SavedSliderSetting : MonoBehaviour
    {
        [SerializeField]
        private string _prefsID = string.Empty;

        [SerializeField]
        private Slider _slider = null!;

        [SerializeField]
        private float _defaultValue;

        private void Start()
            => _slider.value = PlayerPrefs.HasKey(_prefsID)
                ? PlayerPrefs.GetFloat(_prefsID)
                : _defaultValue;

        private void OnDestroy()
        {
            PlayerPrefs.SetFloat(_prefsID, _slider.value);
            PlayerPrefs.Save();
        }
    }
}
