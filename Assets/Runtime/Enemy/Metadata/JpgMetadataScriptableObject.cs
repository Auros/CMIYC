using System;
using UnityEngine;
using UnityEngine.UI;

namespace CMIYC.Metadata
{
    [CreateAssetMenu(fileName = "Metadata", menuName = "ScriptableObjects/Enemy Metadata/JPG", order = 1)]
    public class JpgMetadataScriptableObject : ScriptableObject
    {
        public string NameTag = String.Empty;
        public Texture2D Texture;
    }
}
