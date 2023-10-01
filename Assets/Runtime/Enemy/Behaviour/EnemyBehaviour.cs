using System;
using System.Collections.Generic;
using System.Linq;
using AuraTween;
using CMIYC.Projectiles;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace CMIYC.Enemy.Behaviour
{
    public abstract class EnemyBehaviour : MonoBehaviour, IProjectileTarget
    {
        private bool _isAlive = false;

        private float _health;
        private float _maxHealth;
        protected Camera _cameraToLookAt;
        protected TweenManager _tweenManager;
        private Action<EnemyBehaviour>? _onDeath;

        private float _initialYPos = 0f;
        private float _previousDissolve = _maxDissolve;
        private List<float> _queuedDissolves = new();

        [SerializeField]
        private Transform _nameTag = null!;
        [SerializeField]
        protected TMP_Text _nameText = null!;
        [SerializeField]
        protected TMP_Text _fileTypeText = null!;
        [SerializeField]
        protected ParticleSystem _damageParticles = null!;

        [SerializeField]
        private List<Renderer> _dissolvingRenderers = new();

        private static readonly int _dissolveProperty = Shader.PropertyToID("_DissolveY");
        private static readonly float _maxDissolve = 1.7f;
        private static readonly float _minDissolve = -1.04f;
        private static readonly float _aliveMinDissolve = 0.5f;

        public void SetNameTagMetadata(string fileName, Camera cameraToLookAt)
        {
            _nameText.SetText(fileName);
            _cameraToLookAt = cameraToLookAt;
        }

        public void Setup(float health, TweenManager tweenManager, Action<EnemyBehaviour> onDeath)
        {
            _tweenManager = tweenManager;
            _health = health;
            _maxHealth = health;
            _onDeath = onDeath;
            _isAlive = true;
        }

        void Update()
        {
            if (_cameraToLookAt == null) return;

            _nameTag.LookAt(_cameraToLookAt.transform);
            _nameTag.localRotation = Quaternion.Euler(0, _nameTag.localRotation.eulerAngles.y + 180, 0);
        }

        public void OnProjectileHit(ProjectileHitEvent hitEvent)
        {
            if (!_isAlive) return;
            Debug.Log($"Hit enemy for {hitEvent.Instance.Damage} damage!");
            _damageParticles.transform.position = hitEvent.Point;
            _damageParticles.transform.rotation = Quaternion.LookRotation(-hitEvent.Normal, transform.up);
            _damageParticles.Play();
            HandleHealthChange(hitEvent.Instance.Damage);
        }

        private void HandleHealthChange(float damage)
        {
            _health -= damage;
            if (_health < 0) _health = 0;

            if (_health == 0)
            {
                _isAlive = false;
            }

            SetDissolvePercentage(_health / _maxHealth, _health == 0);
        }

        private void SetDissolvePercentage(float percent, bool isDead)
        {
            var dissolve = _aliveMinDissolve + (_maxDissolve - _aliveMinDissolve) * percent;

            if (isDead)
            {
                DeathTween().Forget();
            }
            else
            {
                TweenDissolve(dissolve).Forget();
            }
        }

        private async UniTask TweenDissolve(float dissolve)
        {
            // TODO: Need a proper queue here lol
            await UniTask.WaitUntil(() => !_queuedDissolves.Any(x => x < dissolve));

            _queuedDissolves.Add(dissolve);

            await _tweenManager.Run(_previousDissolve, dissolve, 0.1f,
                (t) =>
                {
                    foreach (var dissolvingRenderer in _dissolvingRenderers)
                    {
                        dissolvingRenderer.material.SetFloat(_dissolveProperty, t);
                    }
                }, Easer.Linear);
            _previousDissolve = dissolve;
            _queuedDissolves.Remove(dissolve);
        }

        protected virtual async UniTask DeathTween()
        {
            var dissolve = _minDissolve;
            // TODO: Need a proper queue here lol
            await UniTask.WaitUntil(() => !_queuedDissolves.Any(x => x < dissolve));

            _queuedDissolves.Add(dissolve);

            _initialYPos = this.transform.localPosition.y;

            var duration = 0.5f;
            _tweenManager.Run(1f, 0f, duration / 4,
                (t) =>
                {
                    _fileTypeText.alpha = t;
                }, Easer.Linear);
            _tweenManager.Run(_previousDissolve, dissolve, duration,
                (t) =>
                {
                    foreach (var dissolvingRenderer in _dissolvingRenderers)
                    {
                        dissolvingRenderer.material.SetFloat(_dissolveProperty, t);
                    }
                }, Easer.Linear);
            _tweenManager.Run(1f, 0f, duration,
                (t) =>
                {
                    _nameText.alpha = t;
                }, Easer.Linear);

            _queuedDissolves.Remove(dissolve);

            await UniTask.Delay(500);
            /*await _tweenManager.Run(1f, 0f, duration, (t) =>
            {
                this.transform.localScale = new Vector3(1f, t, 1f);
                var yPos = _initialYPos - ((1 - t * 0.8f));
                this.transform.localPosition = new Vector3(this.transform.localPosition.x, yPos, this.transform.localPosition.z);
            }, Easer.Linear);*/

            // you have died, cleanup whatever
            _onDeath?.Invoke(this);
        }
    }
}
