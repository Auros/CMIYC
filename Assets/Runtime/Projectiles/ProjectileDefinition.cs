using System;
using UnityEngine;

namespace CMIYC.Projectiles
{
#nullable enable
    // Used in the root of a prefab.
    public class ProjectileDefinition : MonoBehaviour
    {
        [Tooltip("If true, this projectile will have infinite velocity and travel instantly to its target.")]
        [SerializeField] private bool _hitScan;

        [Tooltip("If false, this sets the velocity of the projectile over one second.")]
        [SerializeField] private float _velocity;

        [Tooltip("If the projectile has not hit any target over this length of time, instantly destroy. <= 0 implies infinite lifetime.")]
        [SerializeField] private float _lifetime;

        [Tooltip("Layers for the projectile to ignore or collide with.")]
        [SerializeField] private LayerMask _layerMask;

        private float _timeAlive;
        private Action<ProjectileHitEvent>? _onCollision;
        private Ray _raycast;
        private bool _initialized;

        public void Initialize(Vector3 position, Vector3 forward, Action<ProjectileHitEvent>? onCollision)
        {
            if (_initialized) throw new InvalidOperationException("BRUH WE ARE ALREADY INITIALIZED!!!!");
            _initialized = true;

            transform.position = position;
            transform.forward = forward;

            _onCollision = onCollision;

            if (_hitScan)
            {
                _velocity = float.MaxValue;
            }

            _raycast = new Ray(position, forward);
        }

        private void Update()
        {
            var rayVelocity = _velocity * Time.deltaTime;
            var targetPoint = _raycast.GetPoint(rayVelocity);

            // Raycast, add a little bit wiggle room so the raycast can actually hit objects
            var projectileHit = Physics.Raycast(_raycast, out var hitInfo, rayVelocity + 0.1f, _layerMask);

            if (!projectileHit)
            {
                // Instantly destroy the projectile if we are hitscan; we wont hit anything ever.
                if (_hitScan)
                {
                    Destroy(gameObject);
                    return;
                }

                // Move along our ray by the velocity
                transform.position = _raycast.origin = targetPoint;

                // Early return if our projectile has infinite life
                if (_lifetime <= 0) return;

                // Update our lifetime
                _timeAlive += Time.deltaTime;

                // Exceeded lifetime, exterminate! Exterminate! Exterminate!
                if (_timeAlive > _lifetime) Destroy(gameObject);

                // Early return, since we didnt actually hit anything
                return;
            }

            // Our projectile has hit something; destroy and call back
            var hitEvent = new ProjectileHitEvent(this, hitInfo);
            _onCollision?.Invoke(hitEvent);

            Destroy(gameObject);
        }
    }
#nullable restore
}
