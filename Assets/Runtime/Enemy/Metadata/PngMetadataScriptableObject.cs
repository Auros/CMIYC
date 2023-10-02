using System;
using UnityEngine;
using UnityEngine.UI;

namespace CMIYC.Metadata
{
    [CreateAssetMenu(fileName = "Metadata", menuName = "ScriptableObjects/Enemy Metadata/PNG", order = 1)]
    public class PngMetadataScriptableObject : ScriptableObject
    {
        public string NameTag = String.Empty;
        public Texture2D Texture;
    }
}
