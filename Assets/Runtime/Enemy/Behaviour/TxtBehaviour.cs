using AuraTween;
using CMIYC.Metadata;
using CMIYC.Projectiles;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace CMIYC.Enemy.Behaviour
{
    public class TxtBehaviour : EnemyBehaviour
    {
        public TxtMetadataScriptableObject AssignedMetadata { get; private set; }

        [SerializeField]
        private TMP_Text _documentText = null!;

        [SerializeField]
        private TextProjectile _textProjectile = null!;
        [SerializeField]
        private Transform _projectileOrigin = null!;

        // TXT: Length of metadata contents * hardcoded value to keep score comparable to png
        public override int Size => AssignedMetadata.TextContents.Length * 16;

        void Start()
        {

        }

        public void SetMetadata(TxtMetadataScriptableObject metadata, EnemyScriptableObject enemy, Camera cameraToLookAt)
        {
            AssignedMetadata = metadata;

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

        public void CreateTextProjectile(string displayText)
        {
            if (!_isWithinPlayerRange) return;
            // create projectile
            FireProjectile(_textProjectile, _cameraToLookAt.transform, displayText);
        }

        // TODO: Only fire within reasonable range of player to avoid unfair sniping
        private void FireProjectile(TextProjectile projectile, Transform target, string displayText)
        {
            if (projectile == null) return;

            // Calculate projectile direction from emission point
            var spawnPoint = _projectileOrigin.position;
            var projectileForward = target.position - spawnPoint;

            // Emit a new projectile at the weapon emission point, and let it loose.
            var newProjectile = Instantiate(projectile);
            newProjectile.ProjectileDefinition.Initialize(spawnPoint, projectileForward);
            newProjectile.Text.SetText(displayText);
            newProjectile.SetTarget(target);
        }
    }
}
