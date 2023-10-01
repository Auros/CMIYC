using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CMIYC.Projectiles
{
    public class PngProjectile : MonoBehaviour
    {
        [field: SerializeField]
        public ProjectileDefinition ProjectileDefinition { get; set; } = null!;

        [field: SerializeField]
        public List<Renderer> Renderers { get; set; } = null!;
        [field: SerializeField]
        public List<TrailRenderer> TrailRenderers { get; set; } = null!;

        private Transform? _target;

        public void SetTarget(Transform target)
        {
            _target = target;
        }

        public void SetColor(Color color)
        {
            color.a = Mathf.Max(color.a, 0.4f);
            foreach (var renderer in Renderers)
            {
                renderer.material.color = color;
            }
        }

        /*void Update()
        {
            if (_target == null) return;
            Text.transform.LookAt(_target);
            Text.transform.Rotate(0, 180, 0);
        }*/
    }
}
