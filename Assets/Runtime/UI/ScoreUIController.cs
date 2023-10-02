using CMIYC.Player;
using CMIYC.Runtime.Utilities;
using TMPro;
using UnityEngine;

namespace CMIYC
{
    public class ScoreUIController : MonoBehaviour
    {
        private static readonly string[] _unitsArray = new[]
        {
            "bytes",
            "KB",
            "MB",
            "GB",
            "TB"
        };

        [SerializeField]
        private PlayerScoreController _scoreController = null!;

        [SerializeField]
        private TMP_Text _text = null!;

        private void Start()
        {
            if (_scoreController == null)
            {
                Debug.LogWarning("yo we need a Score Controller... eventually. not now tho");
                return;
            }

            _scoreController.OnScoreIncrease += OnScoreIncrease;
        }

        private void OnScoreIncrease(int scoreIncrease)
        {
            _text.text = $"SCORE: {FileSizeUtilities.GetFileSizeText((long)_scoreController.Score)}";
        }

        private void OnDestroy()
        {
            if (_scoreController == null)
            {
                Debug.LogWarning("yo we need a Score Controller... eventually. not now tho");
                return;
            }

            _scoreController.OnScoreIncrease -= OnScoreIncrease;
        }
    }
}
