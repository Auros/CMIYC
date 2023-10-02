using CMIYC.Location;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CMIYC.UI
{
    public class DirectoryElement : MonoBehaviour
    {
        [SerializeField]
        private RawImage _icon = null!;

        [SerializeField]
        private TMP_Text _name = null!;

        [SerializeField]
        private TMP_Text _savedSpace = null!;

        public void Initialize(LocationNode node)
        {
            _icon.texture = node.texture2D!;
            _name.text = node.location;

            // TODO: formatting
            _savedSpace.text = node.size.HasValue
                ? $"-{node.size} bytes"
                : string.Empty;
        }
    }
}
