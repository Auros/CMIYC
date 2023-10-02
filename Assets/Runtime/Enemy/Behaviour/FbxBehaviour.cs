using System;
using CMIYC.Metadata;
using CMIYC.Projectiles;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CMIYC.Enemy.Behaviour
{
    public class FbxBehaviour : EnemyBehaviour
    {
        [SerializeField]
        private Renderer _imageRenderer;

        [SerializeField]
        private Transform _projectileEmitPoint;

        private static int _mainTexProperty = Shader.PropertyToID("_MainTex");

        private float _fireRate;
        private ProjectileDefinition _projectile;

        public void SetMetadata(FbxMetadataScriptableObject metadata, EnemyScriptableObject enemy, Camera cameraToLookAt)
        {
            _fireRate = metadata.FireRate;
            _projectile = metadata.Projectile;

            // should prob be cached or something
            var fileExtension = "." + enemy.EnemyTypeName.ToLower();

            _imageRenderer.material.SetTexture(_mainTexProperty, metadata.Texture);
            SetNameTagMetadata(metadata.NameTag + fileExtension, cameraToLookAt);

            // super simple attack loop
            AttackLoop().AttachExternalCancellation(this.GetCancellationTokenOnDestroy()).Forget();
        }

        private async UniTask AttackLoop()
        {
            while (_isAlive)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(_fireRate));
                if (_isWithinPlayerRange && _isAlive)
                {
                    // GOOBIE: create projectiles here
                    // Check PngBehaviour/JpgBehaviour for example of how i'm spawning projectiles using enemies
                    // Those both use multiple bullets at a time but it should be fairly easy to just do one

                    if (_projectile == null) return;

                    // Calculate projectile direction from emission point
                    var spawnPoint = _projectileEmitPoint.position;
                    var projectileForward = _cameraToLookAt.transform.position - spawnPoint;

                    // Emit a new projectile at the weapon emission point, and let it loose.
                    var newProjectile = Instantiate(_projectile);
                    newProjectile.Initialize(spawnPoint, projectileForward);
                }
            }
        }
    }
}
