using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CMIYC.Location
{
    public class LocationController : MonoBehaviour
    {
        public event Action<string> OnLocationEnter;
        public event Action<string> OnLocationExit;

        private Stack<string> _locations = new();

        private void Start()
        {
            var randomDrive = (char)UnityEngine.Random.Range('A', 'Z');
            _locations.Push($"{randomDrive}:");
        }

        public string GetFullLocation()
        {
            StringBuilder stringBuilder = new();
            // wahhhh allocations
            foreach (var loc in _locations.ToArray().Reverse())
            {
                stringBuilder.Append(loc);
                stringBuilder.Append('\\');
            }
            return stringBuilder.ToString();
        }

        public void EnterLocation(string location)
        {
            _locations.Push(location);
            OnLocationEnter?.Invoke(location);
        }

        public void ExitLocation()
        {
            var location = _locations.Pop();
            OnLocationExit?.Invoke(location);
        }
    }
}
