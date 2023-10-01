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

        [Tooltip("The rigidbody to use for physics calculations (if it is desired).")]
        [SerializeField] private Rigidbody? _attachedRigidbody;

        [Tooltip("If the projectile has not hit any target over this length of time, instantly destroy. <= 0 implies infinite lifetime.")]
        [SerializeField] private float _lifetime;

        [Tooltip("Size curve ypipeee")]
        [SerializeField] private AnimationCurve _sizeCurve;

        [Tooltip("Layers for the projectile to ignore or collide with.")]
        [SerializeField] private LayerMask _layerMask;

        private float _timeAlive;
        private Ray _raycast;
        private bool _initialized;
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

            _originalScale = transform.localScale;

            _raycast = new Ray(position, forward);
        }

        private void Update()
        {
            // we skip one update so that the trail renders from the spawn position
            if (!_updateSkip)
            {
                _updateSkip = true;
                return;
            }

            ApplyLifetime();

            if (!_collided)
            {
                var rayVelocity = _velocity * Time.deltaTime;
                if (_hitScan)
                {
                    rayVelocity = float.MaxValue * Time.deltaTime;
                }

                var targetPoint = _raycast.GetPoint(rayVelocity);

                // Raycast, add a little bit wiggle room so the raycast can actually hit objects
                var projectileHit = Physics.Raycast(_raycast, out var hitInfo, rayVelocity + 0.1f, _layerMask);

                if (!projectileHit)
                {
                    // Instantly destroy the projectile if we are hitscan; we wont hit anything ever.
                    if (_hitScan)
                    {
                        if (_attachedRigidbody == null)
                        {
                            // Only destroy if we dont want to continue as a rigidbody
                            Destroy(gameObject);
                        }
                        else
                        {
                            // Add forces and forget
                            _attachedRigidbody.AddForce(transform.forward.normalized * _velocity * 20); // mult velocity by 20 to increase speed (since it is supposed to be hitscan)
                            _attachedRigidbody.AddTorque(new Vector3(UnityEngine.Random.Range(-1, 1), UnityEngine.Random.Range(-1, 1), UnityEngine.Random.Range(-1, 1)).normalized);
                            _collided = true;
                        }

                        return;
                    }

                    // Move along our ray by the velocity
                    transform.position = _raycast.origin = targetPoint;

                    // Early return, since we didnt actually hit anything
                    return;
                }

                // We hit something, send the callback
                var hitEvent = new ProjectileHitEvent(this, hitInfo.collider, hitInfo.point, transform.forward);
                Callback(hitEvent);

                if (_attachedRigidbody == null)
                {
                    // Only destroy if we dont want to continue as a rigidbody
                    Destroy(gameObject);
                }
                else
                {
                    // Set position to the place where bullet hit, then apply the forces and forget.
                    transform.position = hitInfo.point - transform.forward.normalized / 5;
                    _attachedRigidbody = GetComponent<Rigidbody>();
                    _attachedRigidbody.AddForce(transform.forward.normalized * _velocity);
                    _attachedRigidbody.AddTorque(new Vector3(UnityEngine.Random.Range(-1, 1), UnityEngine.Random.Range(-1, 1), UnityEngine.Random.Range(-1, 1)).normalized);
                }

                // Set collided to true to ensure we dont repeat all of this code again
                _collided = true;
            }
        }

        private void ApplyLifetime()
        {
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

            if (_sizeCurve == null)
            {
                return;
            }

            // Update scale
            var scale = _sizeCurve.Evaluate(_timeAlive / _lifetime);
            transform.localScale = _originalScale * scale;
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

            var velocityNormal = transform.forward;
            if (_attachedRigidbody != null)
            {
                velocityNormal = _attachedRigidbody.velocity.normalized;
            }

            var hitEvent = new ProjectileHitEvent(this, collision.collider, contactPoint.point, velocityNormal);

            Callback(hitEvent);

            if (_attachedRigidbody == null)
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
