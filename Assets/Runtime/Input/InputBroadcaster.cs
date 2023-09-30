using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace CMIYC.Input
{
    public class InputBroadcaster : MonoBehaviour
    {
        private List<IInputReceiver> _inputReceivers = new();

        public void Register(IInputReceiver inputReceiver)
        {
            _inputReceivers.Add(inputReceiver);
        }

        void Update()
        {
            // won't work w/ controller
            if (Keyboard.current == null) return;

            if (Keyboard.current.anyKey.IsPressed())
            {
                foreach (var key in Keyboard.current.allKeys)
                {
                    if (key.wasPressedThisFrame)
                    {
                        BroadcastKey(key);
                        // Debug.Log(key.displayName);
                    }
                }
            }
        }

        private void BroadcastKey(KeyControl key)
        {
            foreach (var inputReceiver in _inputReceivers)
            {
                if (inputReceiver != null)
                {
                    inputReceiver.OnKeyPressed(key);
                }
            }
        }
    }
}
