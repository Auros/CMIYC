using System.Collections.Generic;
using CMIYC.Audio;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace CMIYC.Runtime.UI.Behaviour
{
    public class ScreenJpegBehaviour : MonoBehaviour
    {
        [SerializeField]
        private Image _image;

        [SerializeField]
        private AudioPool _audioPool;

        [SerializeField]
        private List<AudioClip> _jpegClips = new();

        //  dont have enough time to decouple UI
        [SerializeField]
        private PlayerHealthController _playerHealthController = null!;

        private static int _addToNoiseProperty = Shader.PropertyToID("_AddToNoiseUV");
        private static int _uvMultProperty = Shader.PropertyToID("_UVMult");

        private float _timeRemaining = 0f;
        private bool _doingEffect = false;

        public void Awake()
        {
            _playerHealthController.PlayerTookJpegDamage += OnTakeJpegDamage;
        }

        private void OnTakeJpegDamage()
        {
            AddEffectTime(0.3f);
        }

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
                await UniTask.Delay(Random.Range(250, 400));
                if (!enabled) break;
                if (!_doingEffect) continue;
                ChangeJpeg();
                //await UniTask.Delay(Random.Range(1000, 1600));
            }
        }

        private void ChangeJpeg()
        {
            Color randomColor = Color.white;
            if (Random.Range(0, 1f) > 0.6f)
            {
                randomColor = new Color(Random.Range(0.6f, 1f), Random.Range(0.6f, 1f), Random.Range(0.6f, 1f), 1);
            }
            _image.material.color = randomColor;
            _image.material.SetVector(_addToNoiseProperty, new Vector4(Random.Range(0f, 5f), Random.Range(0f, 5f), 0, 0));

            // play sound
            var clip = _jpegClips[Random.Range(0, _jpegClips.Count)];
            try
            {
                _audioPool.Play(clip);
            }
            catch
            {
                // can't play right on Start(). dont wanna fix
            }
        }

        public void Update()
        {
            if (!_doingEffect) return;

            _timeRemaining -= Time.deltaTime;

            if (_timeRemaining <= 0)
            {
                _timeRemaining = 0;
                _doingEffect = false;
                _image.enabled = false;
            }
        }

        public void AddEffectTime(float time)
        {
            if (_doingEffect == false)
            {
                _doingEffect = true;
                _image.enabled = true;
                ChangeJpeg();
            }

            _timeRemaining += time;
            Debug.Log("Added JPEG effect. total effect time:");
            Debug.Log(_timeRemaining);
        }
    }
}
