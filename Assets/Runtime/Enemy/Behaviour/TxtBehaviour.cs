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

        public void SetMetadata(TxtMetadataScriptableObject metadata)
        {
            _documentText.SetText(metadata.TextContents);
        }
    }
}
