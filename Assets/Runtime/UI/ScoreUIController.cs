using CMIYC.Player;
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
            _scoreController.OnScoreIncrease += OnScoreIncrease;
        }

        private void OnScoreIncrease(int scoreIncrease)
        {
            var score = _scoreController.Score;
            var portion = 0ul;
            var unitIdx = 0;

            while (score / 1024 > 0)
            {
                if (unitIdx == _unitsArray.Length - 1) break;

                portion = score % 1024;
                score /= 1024;
                unitIdx++;
            }

            _text.text = $"SCORE: {score}.{portion / 1024f * 100:N0} {_unitsArray[unitIdx]}";
        }

        private void OnDestroy()
        {
            _scoreController.OnScoreIncrease -= OnScoreIncrease;
        }
    }
}
