using System;
using AuraTween;
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

        [Tooltip("Size curve ypipeee")]
        [SerializeField] private AnimationCurve _sizeCurve;

        [Tooltip("Layers for the projectile to ignore or collide with.")]
        [SerializeField] private LayerMask _layerMask;

        private float _timeAlive;
        private Ray _raycast;
        private bool _initialized;
        private Rigidbody _attachedRigidbody;
        private bool _collided = false;
        private bool _updateSkip = false;
        private Vector3 _originalScale = Vector3.zero;

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

            gameObject.SetActive(true);
            transform.position = position;
            transform.forward = forward;

            if (_hitScan)
            {
                //_velocity = float.MaxValue; dont want to use this anymore, bc it fucks it if rigidbody is enabled
            }

            if (_useRigidbody && !_hitScan)
            {
                _attachedRigidbody = GetComponent<Rigidbody>();
                _attachedRigidbody.AddForce(forward.normalized * _velocity);
                _attachedRigidbody.AddTorque(new Vector3(UnityEngine.Random.Range(-1, 1), UnityEngine.Random.Range(-1, 1), UnityEngine.Random.Range(-1, 1)).normalized);
                return;
            }

            _originalScale = transform.localScale;

            _raycast = new Ray(position, forward);
        }

        private void Update()
        {
            if (!_updateSkip && _hitScan && _useRigidbody)
            {
                _updateSkip = true;
                return;
            }

            var rayVelocity = _velocity * Time.deltaTime;
            if (_hitScan)
            {
                rayVelocity = float.MaxValue;
            }

            var targetPoint = _raycast.GetPoint(rayVelocity);

            // Raycast, add a little bit wiggle room so the raycast can actually hit objects
            var projectileHit = Physics.Raycast(_raycast, out var hitInfo, rayVelocity + 0.1f, _layerMask);

            if (!projectileHit)
            {
                // Instantly destroy the projectile if we are hitscan; we wont hit anything ever.
                if (_hitScan)
                {
                    // If use rigidbody, dont delete the gameobject, so that the rigidbody can continue to simulate
                    if (!_useRigidbody)
                    {
                        Destroy(gameObject);
                    }
                    else if (!_collided)
                    {
                        _attachedRigidbody = GetComponent<Rigidbody>();
                        _attachedRigidbody.AddForce(transform.forward.normalized * _velocity * 20);
                        _attachedRigidbody.AddTorque(new Vector3(UnityEngine.Random.Range(-1, 1), UnityEngine.Random.Range(-1, 1), UnityEngine.Random.Range(-1, 1)).normalized);
                        _collided = true;
                    }

                    return;
                }

                if (!_useRigidbody)
                {
                    // Move along our ray by the velocity
                    transform.position = _raycast.origin = targetPoint;
                }

                // Early return, since we didnt actually hit anything
                return;
            }

            // Early return if our projectile has infinite life
            if (_lifetime <= 0) return;

            // Update our lifetime
            _timeAlive += Time.deltaTime;

            // Exceeded lifetime, exterminate! Exterminate! Exterminate!
            if (_timeAlive > _lifetime)
            {
                Destroy(gameObject);
                return;
            }

            // Update scale
            var scale = _sizeCurve.Evaluate(_timeAlive / _lifetime);
            transform.localScale = _originalScale * scale;

            // Our projectile has hit something; destroy and call back
            if (!_collided)
            {
                var hitEvent = new ProjectileHitEvent(this, hitInfo.collider, hitInfo.point, transform.forward);
                Callback(hitEvent);

                if (!_useRigidbody)
                {
                    Destroy(gameObject);
                }
                else
                {
                    transform.position = hitInfo.point - transform.forward.normalized / 5;
                    _attachedRigidbody = GetComponent<Rigidbody>();
                    _attachedRigidbody.AddForce(transform.forward.normalized * _velocity);
                    _attachedRigidbody.AddTorque(new Vector3(UnityEngine.Random.Range(-1, 1), UnityEngine.Random.Range(-1, 1), UnityEngine.Random.Range(-1, 1)).normalized);
                }

                _collided = true;
            }
        }

        // If the physics system reports a collision, we should assume that we need to call back.
        private void OnCollisionEnter(Collision collision)
        {
            // if we already collideed then no colliding !!!
            if (_collided) return;

            // Ensure we are colliding only with layers we want
            if ((_layerMask.value & 1 << collision.gameObject.layer) <= 0) return;

            _collided = true;

            var contactPoint = collision.GetContact(0);
            var hitEvent = new ProjectileHitEvent(this, collision.collider, contactPoint.point, _attachedRigidbody.velocity.normalized);

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
