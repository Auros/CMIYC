using CMIYC.Metadata;
using UnityEngine;
using UnityEngine.UI;

namespace CMIYC.Enemy.Behaviour
{
    public class PngBehaviour : EnemyBehaviour
    {
        [SerializeField]
        private Renderer _imageRenderer;

        public void SetMetadata(PngMetadataScriptableObject metadata, EnemyScriptableObject enemy, Camera cameraToLookAt)
        {
            // should prob be cached or something
            var fileExtension = "." + enemy.EnemyTypeName.ToLower();

            _imageRenderer.material.SetTexture("_MainTex", metadata.Texture);
            SetNameTagMetadata(metadata.NameTag + fileExtension, cameraToLookAt);
        }
    }
}
