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

        private async void Start()
        {
            Debug.Log("guh");
            // TODO: populate vertical list with elements, ideally tabbed in similar to an actual hierarchy

            foreach (Transform child in _container.transform)
            {
                Destroy(child.gameObject);
            }

            await UniTask.Delay(25000);
            Debug.Log("guh2");
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
