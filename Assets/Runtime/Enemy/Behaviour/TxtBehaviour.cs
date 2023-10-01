using AuraTween;
using CMIYC.Metadata;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace CMIYC.Enemy.Behaviour
{
    public class TxtBehaviour : EnemyBehaviour
    {
        [SerializeField]
        private TMP_Text _documentText = null!;

        void Start()
        {

        }

        public void SetMetadata(TxtMetadataScriptableObject metadata, EnemyScriptableObject enemy, Camera cameraToLookAt)
        {
            // should prob be cached or something
            var fileExtension = "." + enemy.EnemyTypeName.ToLower();

            _documentText.SetText(metadata.TextContents);
            SetNameTagMetadata(metadata.NameTag + fileExtension, cameraToLookAt);
        }

        protected override async UniTask DeathTween()
        {
            base.DeathTween();

            _tweenManager.Run(1f, 0f, 0.5f,
                (t) =>
                {
                    _documentText.alpha = t;
                }, Easer.Linear);

            await UniTask.Delay(500);
        }
    }
}
