using System;
using System.Collections.Generic;
using UnityEngine;

namespace CMIYC.Platform
{
    public class Motherboard : MonoBehaviour
    {
        public Transform Root => transform;

        public Daughterboard Entrance => throw new NotImplementedException();

        public IReadOnlyList<Daughterboard> Daughterboards => throw new NotImplementedException();

        public void SetData(Daughterboard entrance, Daughterboard daughterboards)
        {

        }
    }
}
