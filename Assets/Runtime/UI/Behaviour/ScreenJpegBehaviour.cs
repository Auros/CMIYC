using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace CMIYC.Runtime.UI.Behaviour
{
    public class ScreenJpegBehaviour : MonoBehaviour
    {
        [SerializeField]
        private Image _image;

        private static int _addToNoiseProperty = Shader.PropertyToID("_AddToNoiseUV");
        private static int _uvMultProperty = Shader.PropertyToID("_UVMult");

        public void OnEnable()
        {
            ChangeLoop().Forget();

            var ratio = (float)Screen.width / Screen.height;
            _image.material.SetVector(_uvMultProperty, new Vector4(ratio, 1, 1, 1));
        }

        private async UniTask ChangeLoop()
        {
            while (enabled)
            {
                if (!enabled) break;
                Color randomColor = Color.white;
                if (Random.Range(0, 1f) > 0.6f)
                {
                    randomColor = new Color(Random.Range(0.6f, 1f), Random.Range(0.6f, 1f), Random.Range(0.6f, 1f), 1);
                }
                _image.material.color = randomColor;
                _image.material.SetVector(_addToNoiseProperty, new Vector4(Random.Range(0f, 5f), Random.Range(0f, 5f), 0, 0));
                await UniTask.Delay(Random.Range(1000, 1600));
            }
        }
    }
}
