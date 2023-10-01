using UnityEngine;
using UnityEngine.InputSystem;

namespace CMIYC.Input
{
    /// <summary>
    /// Container object that holds a shared <see cref="CacheInput"/> instance.
    /// </summary>
    public class InputController : MonoBehaviour
    {
        /// <summary>
        /// Shared <see cref="CacheInput"/> for consumers to use.
        /// </summary>
        public CacheInput Input { get; private set; }

        /// <summary>
        /// Enabled state
        /// </summary>
        public bool Enabled { get; private set; }

        private void Awake() => Input = new();

        private void Start() => Enable();

        public void Enable()
        {
            foreach (var inputAction in Input)
            {
                inputAction.Enable();
            }

            Enabled = true;
        }

        public void Disable(params InputAction[] ignore)
        {
            foreach (var inputAction in Input)
            {
                // Iterate through our ignore list to see if we should ignore this action
                var willIgnore = false;
                for (var i = 0; i < ignore.Length; i++)
                {
                    willIgnore |= inputAction == ignore[i];
                }

                if (willIgnore) continue;

                inputAction.Disable();
            }

            Enabled = false;
        }
    }
}
