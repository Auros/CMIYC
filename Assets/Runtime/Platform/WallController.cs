using UnityEngine;

namespace CMIYC.Platform
{
    public class WallController : MonoBehaviour
    {
        [SerializeField]
        private GameObject _door = null!;

        [SerializeField]
        private GameObject _wall = null!;

        public void SetType(WallSegmentType wall)
        {
            _door.SetActive(wall is WallSegmentType.Door);
            _wall.SetActive(wall is WallSegmentType.Wall);
        }
    }
}
