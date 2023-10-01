using AuraTween;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;

namespace CMIYC.Enemy
{
    public class EnemyTextPool : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _template = null!;

        [SerializeField]
        private TweenManager _tweenManager = null!;

        IObjectPool<TMP_Text> m_Pool;

        private float _startAlpha = 0.6f;
        private float _fadeDuration = 1f;
        private float _posMaxRandom = 0.6f;

        public IObjectPool<TMP_Text> Pool
        {
            get
            {
                if (m_Pool == null)
                {
                    m_Pool = new ObjectPool<TMP_Text>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, false, 10);
                }
                return m_Pool;
            }
        }

        TMP_Text CreatePooledItem()
        {
            var text = Instantiate(_template);
            text.gameObject.SetActive(true);
            text.alpha = _startAlpha;
            return text;
        }

        // Called when an item is returned to the pool using Release
        void OnReturnedToPool(TMP_Text text)
        {
            text.gameObject.SetActive(false);
        }

        // Called when an item is taken from the pool using Get
        void OnTakeFromPool(TMP_Text text)
        {
            text.transform.localPosition = Vector3.zero;
            text.transform.localRotation = Quaternion.identity;
            text.alpha = _startAlpha;
            text.gameObject.SetActive(true);
        }

        // If the pool capacity is reached then any items returned will be destroyed.
        // We can control what the destroy behavior does, here we destroy the GameObject.
        void OnDestroyPoolObject(TMP_Text text)
        {
            Destroy(text.gameObject);
        }

        public async UniTask SpawnText(Transform parent, string inputText)
        {
            var text = Pool.Get();
            text.transform.SetParent(parent);
            // set random position.. hardcoded, whatever
            text.transform.localPosition = new Vector3(Random.Range(-_posMaxRandom, _posMaxRandom), Random.Range(-_posMaxRandom, _posMaxRandom), Random.Range(-_posMaxRandom, _posMaxRandom));
            text.SetText(inputText);
            await _tweenManager.Run(_startAlpha, 0f, _fadeDuration,
                (t) => text.alpha = t, Easer.Linear);

            if (text != null) Pool.Release(text);
        }
    }
}
