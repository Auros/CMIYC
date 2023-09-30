using CMIYC.Metadata;
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
    }
}
