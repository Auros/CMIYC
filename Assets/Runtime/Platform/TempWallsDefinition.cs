using System.Collections.Generic;
using UnityEngine;

namespace CMIYC.Platform
{
    public class TempWallsDefinition : MonoBehaviour
    {
        [SerializeField]
        public TempWallDirection NorthWalls = new();
        [SerializeField]
        public TempWallDirection SouthWalls = new();
        [SerializeField]
        public TempWallDirection EastWalls = new();
        [SerializeField]
        public TempWallDirection WestWalls = new();
    }
}
