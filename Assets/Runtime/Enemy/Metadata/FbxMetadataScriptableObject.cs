using System;
using CMIYC.Projectiles;
using UnityEngine;

namespace CMIYC.Metadata
{
    [CreateAssetMenu(fileName = "Metadata", menuName = "ScriptableObjects/Enemy Metadata/FBX", order = 1)]
    public class FbxMetadataScriptableObject : ScriptableObject
    {
        public string NameTag = String.Empty;
        public Texture2D Texture;
        public ProjectileDefinition Projectile;
        public GameObject DroppedItem;
        public float FireRate;
    }
}
