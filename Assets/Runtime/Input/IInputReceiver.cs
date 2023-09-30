using UnityEngine.InputSystem.Controls;

namespace CMIYC.Input
{
    public interface IInputReceiver
    {
        void OnKeyPressed(KeyControl key);
    }
}
