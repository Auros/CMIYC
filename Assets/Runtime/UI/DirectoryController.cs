using System.Collections;
using System.Collections.Generic;
using CMIYC.Location;
using UnityEngine;

namespace CMIYC.UI
{
    public class DirectoryController : MonoBehaviour
    {
        [SerializeField]
        private DirectoryElement _elementPrefab = null!;

        [SerializeField]
        private LocationTrackerController _locationTracker = null!;

        private void Start()
        {
            // TODO: populate vertical list with elements, ideally tabbed in similar to an actual hierarchy
        }
    }
}
