using System;
using TMPro;
using UnityEngine;

namespace CMIYC.UI
{
    public class GameplayTipController : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _text = null!;

        [SerializeField]
        private string[] _tips = Array.Empty<string>();

        private void Start()
            => _text.text = _tips[UnityEngine.Random.Range(0, _tips.Length)];
    }
}
