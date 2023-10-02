using System;
using System.Collections.Generic;
using AuraTween;
using CMIYC.Metadata;
using CMIYC.Projectiles;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace CMIYC.Enemy.Behaviour
{
    public class PngBehaviour : EnemyBehaviour
    {
        public PngMetadataScriptableObject AssignedMetadata { get; private set; }

        [SerializeField]
        private Renderer _imageRenderer;

        [SerializeField]
        private PngProjectile _pngProjectile = null!;

        [SerializeField]
        private List<Transform> _projectileSpawnPoints = new();

        private List<Color> _projectileColors = new();

        private static int _mainTexProperty = Shader.PropertyToID("_MainTex");
        public void SetMetadata(PngMetadataScriptableObject metadata, EnemyScriptableObject enemy, Camera cameraToLookAt)
        {
            AssignedMetadata = metadata;

            // should prob be cached or something
            var fileExtension = "." + enemy.EnemyTypeName.ToLower();

            _imageRenderer.material.SetTexture(_mainTexProperty, metadata.Texture);
            SetNameTagMetadata(metadata.NameTag + fileExtension, cameraToLookAt);

            GetPixels();

            // super simple attack loop
            AttackLoop().AttachExternalCancellation(this.GetCancellationTokenOnDestroy()).Forget();
        }

        private async UniTask AttackLoop()
        {
            while (_isAlive)
            {
                await UniTask.Delay(2500);
                if (_isWithinPlayerRange && _isAlive)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        CreatePngProjectile(_projectileSpawnPoints[i], _projectileColors[i], i);
                    }
                }
            }
        }
        private void GetPixels()
        {
            var texture = _imageRenderer.material.mainTexture as Texture2D;
            float yHeight = 0.9f;
            // 2, 3, 4, 5, 6 , 7, 8
            for (int i = 0; i < 8; i++)
            {
                var pixelHeight = (int)(texture.height * yHeight);
                //var pixelWidth = (int)((i / 10f) * (texture.width));
                // hardcoded numbers to match w/ points
                var pixelWidth = (int)((0.15f + (0.10 * i)) * texture.width);
                var pixel = texture.GetPixel(pixelWidth, pixelHeight);

                _projectileColors.Add(pixel);
                // var spawnPoint = _projectileSpawnPoints[i];

                // spawn
                // CreatePngProjectile(spawnPoint, pixel);
            }
        }

        public void CreatePngProjectile(Transform spawn, Color color, int i)
        {
            if (!_isWithinPlayerRange) return;
            // create projectile
            FireProjectile(_pngProjectile, _cameraToLookAt.transform, spawn, color, i);
        }

        // TODO: Only fire within reasonable range of player to avoid unfair sniping
        private void FireProjectile(PngProjectile projectile, Transform target, Transform spawn, Color color, int i)
        {
            if (projectile == null) return;

            // Calculate projectile direction from emission point
            var spawnPoint = spawn.position;
            var projectileForward = target.position - spawnPoint;
            var rotationAmount = (i - 4) * 1.7f;
            projectileForward = Quaternion.Euler(0, rotationAmount, 0) * projectileForward;

            // Emit a new projectile at the weapon emission point, and let it loose.
            var newProjectile = Instantiate(projectile);
            newProjectile.ProjectileDefinition.Initialize(spawnPoint, projectileForward);
            newProjectile.SetColor(color);
            newProjectile.SetTarget(target);
        }

    }
}
