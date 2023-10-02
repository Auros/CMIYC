using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CMIYC.Projectiles
{
    public class JpgProjectile : MonoBehaviour
    {
        private static int _addToNoiseProperty = Shader.PropertyToID("_AddToNoiseUV");

        [field: SerializeField]
        public ProjectileDefinition ProjectileDefinition { get; set; } = null!;

        [field: SerializeField]
        public List<Renderer> Renderers { get; set; } = null!;

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
                renderer.material.SetVector(_addToNoiseProperty, new Vector4(Random.Range(0f, 1f), Random.Range(0f, 1f), 0, 0));
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
