using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CMIYC.Projectiles
{
    // Used in the root of a prefab.
    public class ProjectileDefinition : MonoBehaviour
    {
        [Tooltip("Amount of damage to deal to entities.")]
        [field: SerializeField] public float Damage { get; private set; }

        [Tooltip("If true, this projectile will have infinite velocity and travel instantly to its target.")]
        [SerializeField] private bool _hitScan;

        [Tooltip("If false, this sets the velocity of the projectile over one second.")]
        [SerializeField] private float _velocity;

        [Tooltip("Whether or not to use a rigidbody for physics calculations.")]
        [SerializeField] private bool _useRigidbody;

        [Tooltip("If the projectile has not hit any target over this length of time, instantly destroy. <= 0 implies infinite lifetime.")]
        [SerializeField] private float _lifetime;

        [Tooltip("Layers for the projectile to ignore or collide with.")]
        [SerializeField] private LayerMask _layerMask;

        private float _timeAlive;
        private Ray _raycast;
        private bool _initialized;
        private Rigidbody _attachedRigidbody;
        private bool _collided = false;

        /// <summary>
        /// Initializes this projectile with the given world position and direction.
        /// </summary>
        /// <param name="position">Initial world position of this projectile.</param>
        /// <param name="forward">Direction of projectile travel.</param>
        /// <exception cref="InvalidOperationException">Projectile was already initialized</exception>
        public void Initialize(Vector3 position, Vector3 forward)
        {
            if (_initialized) throw new InvalidOperationException("BRUH WE ARE ALREADY INITIALIZED!!!!");
            _initialized = true;

            transform.position = position;
            transform.forward = forward;

            if (_hitScan)
            {
                _velocity = float.MaxValue;
            }

            if (_useRigidbody)
            {
                _attachedRigidbody = GetComponent<Rigidbody>();
                _attachedRigidbody.AddForce(forward.normalized * _velocity);
                _attachedRigidbody.AddTorque(new Vector3(UnityEngine.Random.Range(-1, 1), UnityEngine.Random.Range(-1, 1), UnityEngine.Random.Range(-1, 1)).normalized);
                return;
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

                if (!_useRigidbody)
                {
                    // Move along our ray by the velocity
                    transform.position = _raycast.origin = targetPoint;
                }

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
            var hitEvent = new ProjectileHitEvent(this, hitInfo.collider);
            CallbackAndDestroy(hitEvent);
        }

        // If the physics system reports a collision, we should assume that we need to call back.
        private void OnCollisionEnter(Collision collision)
        {
            // if we already collideed then no colliding !!!
            if (_collided) return;

            // Ensure we are colliding only with layers we want
            if ((_layerMask.value & 1 << collision.gameObject.layer) <= 0) return;

            _collided = true;

            var hitEvent = new ProjectileHitEvent(this, collision.collider);

            Callback(hitEvent);

            if (!_useRigidbody)
            {
                Destroy(gameObject);
            }
        }

        private void CallbackAndDestroy(ProjectileHitEvent projectileHitEvent)
        {
            Callback(projectileHitEvent);

            Destroy(gameObject);
        }

        private void Callback(ProjectileHitEvent projectileHitEvent)
        {
            // I think using Messages would be more performant than manually iterating through every component?
            projectileHitEvent.Collider
                .BroadcastMessage(nameof(IProjectileTarget.OnProjectileHit), projectileHitEvent, SendMessageOptions.DontRequireReceiver);
        }
    }
}
