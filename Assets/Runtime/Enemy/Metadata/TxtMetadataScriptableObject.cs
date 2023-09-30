using System;
using UnityEngine;

namespace CMIYC.Metadata
{
    [CreateAssetMenu(fileName = "Metadata", menuName = "ScriptableObjects/Enemy Metadata/TXT", order = 1)]
    public class TxtMetadataScriptableObject : ScriptableObject
    {
        public string NameTag = String.Empty;
        [TextArea(5, 3)]
        public string TextContents = String.Empty;
    }
}
