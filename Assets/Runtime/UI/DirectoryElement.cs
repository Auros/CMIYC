using System;
using System.Collections.Generic;
using CMIYC.Location;
using CMIYC.Runtime.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CMIYC.UI
{
    public class DirectoryElement : MonoBehaviour
    {
        private const int _height = 60;

        [SerializeField]
        private RectTransform _rectTransform = null!;

        [SerializeField]
        private RawImage _icon = null!;

        [SerializeField]
        private TMP_Text _name = null!;

        [SerializeField]
        private TMP_Text _savedSpace = null!;

        [SerializeField]
        private Transform _childContainer = null!;

        private List<DirectoryElement> _childElements = new();

        private int RecursiveChildCount(DirectoryElement node)
        {
            int count = 0;
            foreach (var child in node._childElements)
            {
                count += 1 + RecursiveChildCount(child);
            }
            return count;
        }

        public void Initialize(LocationNode node, Func<LocationNode, Transform, DirectoryElement>? createNode)
        {
            _icon.texture = node.texture2D!;
            _name.text = node.location;

            if (!node.size.HasValue)
            {
                _name.text += "\\";
            }
            // TODO: formatting
            _savedSpace.text = node.size.HasValue
                ? $"-{FileSizeUtilities.GetFileSizeText((long)node.size)}"
                : string.Empty;

            foreach (var child in node.children)
            {
                if (child == null) continue;
                var childNode = createNode?.Invoke(child, _childContainer);
                if (childNode != null)
                {
                    _childElements.Add(childNode);
                }
            }

            _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, _height + _height * RecursiveChildCount(this));
        }
    }
}
