using CMIYC.Player;
using UnityEngine;

namespace CMIYC.UI.Settings
{
    public class SensitivitySetting : MonoBehaviour
    {
        [SerializeField]
        private PlayerController _playerController = null!;

        public void OnSliderValueChanged(float newValue) => _playerController.Sensitivity = newValue;
    }
}
