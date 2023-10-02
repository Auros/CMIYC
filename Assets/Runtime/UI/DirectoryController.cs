using System.Collections;
using System.Collections.Generic;
using CMIYC.Location;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CMIYC.UI
{
    public class DirectoryController : MonoBehaviour
    {
        [SerializeField]
        private DirectoryElement _elementPrefab = null!;

        [SerializeField]
        private LocationTrackerController _locationTracker = null!;

        [SerializeField]
        private Transform _container = null!;

        private void Start()
        {
        }

        public void OnPlayerDeath()
        {
            SetDirectories();
        }

        private void SetDirectories()
        {
            // TODO: add this at like, next motherboard traversal or something...
            foreach (Transform child in _container.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (var child in _locationTracker.RootNode.children)
            {
                // drives
                CreateFromNode(child, _container);
            }
        }

        private DirectoryElement CreateFromNode(LocationNode node, Transform parent)
        {
            var element = Instantiate(_elementPrefab, parent);
            element.Initialize(node, CreateFromNode);

            return element;
        }
    }
}
