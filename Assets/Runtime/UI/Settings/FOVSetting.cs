using UnityEngine;

namespace CMIYC.UI.Settings
{
    public class FOVSetting : MonoBehaviour
    {
        [SerializeField]
        private Camera _camera;

        public void OnSliderValueChanged(float value) => _camera.fieldOfView = value;
    }
}
