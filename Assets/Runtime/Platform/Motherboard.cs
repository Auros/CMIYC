using System;
using UnityEngine;

namespace CMIYC.Platform
{
    public class Motherboard : MonoBehaviour
    {
        private bool _entered = false;
        public event Action<Motherboard>? OnEntered;
        public event Action<Motherboard>? OnReleased;

        [SerializeField]
        private BoxCollider _collider = null!;

        [Min(0.01f)]
        [SerializeField]
        private float _height;

        public void SetDimensionsSize(Vector2Int size)
        {
            var width = PlatformGenerator.daughterboardUnit * size.x;
            var height = PlatformGenerator.daughterboardUnit * size.y;

            _collider.center = new Vector3(width / 2f, _height / 2f, height / 2f);
            _collider.size = Vector3.zero.WithX(width).WithY(_height).WithZ(height);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_entered)
            {
                _entered = true;
                OnEntered?.Invoke(this);
            }
        }

        public void ClearEvents()
        {
            _entered = false;
            OnReleased?.Invoke(this);
        }
    }
}
