﻿using System.Collections;
using System.Collections.Generic;
using CMIYC.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CMIYC
{
    public class PlayerController : MonoBehaviour, CacheInput.IPlayerActions
    {
        private CapsuleCollider _capsuleCollider;
        private Rigidbody _rigidbody;
        private Camera _camera;

        private CacheInput _cacheInput;

        [SerializeField]
        private LayerMask _collisionMask;

        [SerializeField]
        private float _sensitivity = 1f;

        [SerializeField]
        private float _maxSpeed = 0f;

        [SerializeField]
        private float _groundAccel = 0f;

        [SerializeField]
        private float _groundDecel = 0f;

        [SerializeField]
        private float _airAccel = 0f;

        [SerializeField]
        private float _jumpForce = 0f;

        private bool _grounded = false;
        private bool _inputJumping = false;
        private Vector2 _inputMovement = Vector2.zero;

        void Start()
        {
            _capsuleCollider = GetComponent<CapsuleCollider>();
            _rigidbody = GetComponent<Rigidbody>();
            _camera = GetComponentInChildren<Camera>();

            _cacheInput = new CacheInput();
            _cacheInput.Player.AddCallbacks(this);
            _cacheInput.Player.Enable();

            Cursor.lockState = CursorLockMode.Locked;
        }

        void Update()
        {
            Vector2 lookValue = _cacheInput.Player.Look.ReadValue<Vector2>();
            lookValue *= _sensitivity * 0.1f;
            Vector3 angles = _camera.transform.localEulerAngles;
            angles.x -= lookValue.y;
            angles.y += lookValue.x;
            _camera.transform.localEulerAngles = angles;
        }

        void FixedUpdate()
        {
            CheckGrounded();

            Vector3 velocity = _rigidbody.velocity;
            float yVelocity = velocity.y;
            velocity.y = 0.0f;

            if (_inputJumping && _grounded)
            {
                _grounded = false;
                yVelocity = _jumpForce;
            }

            float yaw = _camera.transform.localEulerAngles.y * Mathf.Deg2Rad;
            float cosYaw = Mathf.Cos(yaw);
            float sinYaw = Mathf.Sin(yaw);
            Vector3 inputDir = new Vector3(_inputMovement.x, 0f, _inputMovement.y).normalized;
            Vector3 direction = new Vector3(cosYaw * inputDir.x + sinYaw * inputDir.z, 0f, -sinYaw * inputDir.x + cosYaw * inputDir.z);

            // Attempting to move
            if (direction != Vector3.zero)
            {
                float dot = Vector3.Dot(velocity, direction);

                // Counter-strafing
                if (dot < 0.0f)
                {
                    velocity -= direction * dot;
                    dot = 0.0f;
                }

                // Grounded accelerate
                if (_grounded)
                {
                    velocity += direction * _groundAccel * Time.fixedDeltaTime;
                    float speed = velocity.magnitude;
                    if (speed > _maxSpeed)
                        velocity = velocity / speed * _maxSpeed;
                }
                // Air accelerate
                else if (dot < _airAccel * Time.fixedDeltaTime)
                    velocity += direction * _airAccel * Time.fixedDeltaTime;
            }
            // Giving no input & grounded
            else if (velocity != Vector3.zero && _grounded)
            {
                // Decelerate
                float speed = velocity.magnitude;
                direction = velocity / speed;
                speed = Mathf.Max(0.0f, speed - _groundDecel * Time.fixedDeltaTime);
                velocity = direction * speed;
            }

            velocity.y = yVelocity;
            _rigidbody.velocity = velocity;
        }

        private void CheckGrounded()
        {
            Ray ray = new Ray(transform.position + new Vector3(0f, _capsuleCollider.height * -0.5f + _capsuleCollider.radius, 0f), Vector3.down);
            _grounded = Physics.SphereCast(ray, _capsuleCollider.radius - 0.05f, 0.051f, _collisionMask);
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.performed)
                _inputJumping = true;
            else if (context.canceled)
                _inputJumping = false;
        }

        public void OnLook(InputAction.CallbackContext context)
        {
        }

        public void OnMovement(InputAction.CallbackContext context)
        {
            if (context.performed)
                _inputMovement = context.ReadValue<Vector2>();
            else
                _inputMovement = Vector2.zero;
        }
    }
}
