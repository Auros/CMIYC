using System;
using CMIYC.Metadata;
using UnityEngine;

namespace CMIYC.Enemy
{
    // get around unity serialization weirdness to have per-enemy spawn chance

    [Serializable]
    public class TxtMetadataScriptableObjectInstance
    {
        [field: SerializeField]
        public TxtMetadataScriptableObject Metadata { get; set; } = null!;

        [SerializeField]
        public float SpawnWeight = 1f;
    }

    [Serializable]
    public class FbxMetadataScriptableObjectInstance
    {
        [field: SerializeField]
        public FbxMetadataScriptableObject Metadata { get; set; } = null!;

        [SerializeField]
        public float SpawnWeight = 1f;
    }

    [Serializable]
    public class JpgMetadataScriptableObjectInstance
    {
        [field: SerializeField]
        public JpgMetadataScriptableObject Metadata { get; set; } = null!;

        [SerializeField]
        public float SpawnWeight = 1f;
    }

    [Serializable]
    public class PngMetadataScriptableObjectInstance
    {
        [field: SerializeField]
        public PngMetadataScriptableObject Metadata { get; set; } = null!;

        [SerializeField]
        public float SpawnWeight = 1f;
    }
}
