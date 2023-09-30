//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.7.0
//     from Assets/Runtime/Input/CacheInput.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace CMIYC.Input
{
    public partial class @CacheInput: IInputActionCollection2, IDisposable
    {
        public InputActionAsset asset { get; }
        public @CacheInput()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""CacheInput"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""f151dff4-ad0e-46f7-934a-73b6fe5b2e2d"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""Value"",
                    ""id"": ""7395be71-694e-4578-92e0-d732b128dff3"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Look"",
                    ""type"": ""Value"",
                    ""id"": ""f5de94e0-420d-46a4-8b27-12d273a46d59"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""e977e35d-1494-45c0-b7a6-f41111882369"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""f498b3cf-02be-469c-87cc-ae730fd755d1"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Keyboard"",
                    ""id"": ""1b292f8e-d894-4b40-bc2e-89c107fff074"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""d42413fd-7c54-4085-bb2d-dfde75e33839"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""d37407dd-8ebe-4ff0-a1ec-0cc3b36d1e8d"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""d7394c8f-b49c-47c8-88af-ba7c143e8c53"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""276fe158-ae93-42ea-9921-c88079c2ef25"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""a6ad918a-08d0-4967-82d3-781d497c6a46"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f3ca1ed1-89bb-4150-823f-da785e9fac49"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""69fa5577-9627-4e55-a831-3f87a9070981"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""18b31a56-9def-4577-ac5d-441d8d9b04ad"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Shooting"",
            ""id"": ""ec086781-29d3-4677-9aa2-f7097cf6d193"",
            ""actions"": [
                {
                    ""name"": ""Shoot"",
                    ""type"": ""Button"",
                    ""id"": ""fa603b8b-afa5-4892-87c2-3d348f5e2225"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""35824307-a7f6-4c3e-b511-e63b6913ea56"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Shoot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b91f3126-058e-45bb-9ca9-f5049253e711"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Shoot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Pause"",
            ""id"": ""af12c81c-da8f-49d5-9bff-7af48cbc7501"",
            ""actions"": [
                {
                    ""name"": ""Pause"",
                    ""type"": ""Button"",
                    ""id"": ""da5659e8-6181-40ee-b08b-3f0be62fa2a9"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""e11ace19-b623-4d58-8b42-55af8cebbd45"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8c120a74-f910-4f8c-8e86-a0b3488a37e5"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
            // Player
            m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
            m_Player_Movement = m_Player.FindAction("Movement", throwIfNotFound: true);
            m_Player_Look = m_Player.FindAction("Look", throwIfNotFound: true);
            m_Player_Jump = m_Player.FindAction("Jump", throwIfNotFound: true);
            // Shooting
            m_Shooting = asset.FindActionMap("Shooting", throwIfNotFound: true);
            m_Shooting_Shoot = m_Shooting.FindAction("Shoot", throwIfNotFound: true);
            // Pause
            m_Pause = asset.FindActionMap("Pause", throwIfNotFound: true);
            m_Pause_Pause = m_Pause.FindAction("Pause", throwIfNotFound: true);
        }

        public void Dispose()
        {
            UnityEngine.Object.Destroy(asset);
        }

        public InputBinding? bindingMask
        {
            get => asset.bindingMask;
            set => asset.bindingMask = value;
        }

        public ReadOnlyArray<InputDevice>? devices
        {
            get => asset.devices;
            set => asset.devices = value;
        }

        public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

        public bool Contains(InputAction action)
        {
            return asset.Contains(action);
        }

        public IEnumerator<InputAction> GetEnumerator()
        {
            return asset.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Enable()
        {
            asset.Enable();
        }

        public void Disable()
        {
            asset.Disable();
        }

        public IEnumerable<InputBinding> bindings => asset.bindings;

        public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
        {
            return asset.FindAction(actionNameOrId, throwIfNotFound);
        }

        public int FindBinding(InputBinding bindingMask, out InputAction action)
        {
            return asset.FindBinding(bindingMask, out action);
        }

        // Player
        private readonly InputActionMap m_Player;
        private List<IPlayerActions> m_PlayerActionsCallbackInterfaces = new List<IPlayerActions>();
        private readonly InputAction m_Player_Movement;
        private readonly InputAction m_Player_Look;
        private readonly InputAction m_Player_Jump;
        public struct PlayerActions
        {
            private @CacheInput m_Wrapper;
            public PlayerActions(@CacheInput wrapper) { m_Wrapper = wrapper; }
            public InputAction @Movement => m_Wrapper.m_Player_Movement;
            public InputAction @Look => m_Wrapper.m_Player_Look;
            public InputAction @Jump => m_Wrapper.m_Player_Jump;
            public InputActionMap Get() { return m_Wrapper.m_Player; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
            public void AddCallbacks(IPlayerActions instance)
            {
                if (instance == null || m_Wrapper.m_PlayerActionsCallbackInterfaces.Contains(instance)) return;
                m_Wrapper.m_PlayerActionsCallbackInterfaces.Add(instance);
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
                @Look.started += instance.OnLook;
                @Look.performed += instance.OnLook;
                @Look.canceled += instance.OnLook;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
            }

            private void UnregisterCallbacks(IPlayerActions instance)
            {
                @Movement.started -= instance.OnMovement;
                @Movement.performed -= instance.OnMovement;
                @Movement.canceled -= instance.OnMovement;
                @Look.started -= instance.OnLook;
                @Look.performed -= instance.OnLook;
                @Look.canceled -= instance.OnLook;
                @Jump.started -= instance.OnJump;
                @Jump.performed -= instance.OnJump;
                @Jump.canceled -= instance.OnJump;
            }

            public void RemoveCallbacks(IPlayerActions instance)
            {
                if (m_Wrapper.m_PlayerActionsCallbackInterfaces.Remove(instance))
                    UnregisterCallbacks(instance);
            }

            public void SetCallbacks(IPlayerActions instance)
            {
                foreach (var item in m_Wrapper.m_PlayerActionsCallbackInterfaces)
                    UnregisterCallbacks(item);
                m_Wrapper.m_PlayerActionsCallbackInterfaces.Clear();
                AddCallbacks(instance);
            }
        }
        public PlayerActions @Player => new PlayerActions(this);

        // Shooting
        private readonly InputActionMap m_Shooting;
        private List<IShootingActions> m_ShootingActionsCallbackInterfaces = new List<IShootingActions>();
        private readonly InputAction m_Shooting_Shoot;
        public struct ShootingActions
        {
            private @CacheInput m_Wrapper;
            public ShootingActions(@CacheInput wrapper) { m_Wrapper = wrapper; }
            public InputAction @Shoot => m_Wrapper.m_Shooting_Shoot;
            public InputActionMap Get() { return m_Wrapper.m_Shooting; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(ShootingActions set) { return set.Get(); }
            public void AddCallbacks(IShootingActions instance)
            {
                if (instance == null || m_Wrapper.m_ShootingActionsCallbackInterfaces.Contains(instance)) return;
                m_Wrapper.m_ShootingActionsCallbackInterfaces.Add(instance);
                @Shoot.started += instance.OnShoot;
                @Shoot.performed += instance.OnShoot;
                @Shoot.canceled += instance.OnShoot;
            }

            private void UnregisterCallbacks(IShootingActions instance)
            {
                @Shoot.started -= instance.OnShoot;
                @Shoot.performed -= instance.OnShoot;
                @Shoot.canceled -= instance.OnShoot;
            }

            public void RemoveCallbacks(IShootingActions instance)
            {
                if (m_Wrapper.m_ShootingActionsCallbackInterfaces.Remove(instance))
                    UnregisterCallbacks(instance);
            }

            public void SetCallbacks(IShootingActions instance)
            {
                foreach (var item in m_Wrapper.m_ShootingActionsCallbackInterfaces)
                    UnregisterCallbacks(item);
                m_Wrapper.m_ShootingActionsCallbackInterfaces.Clear();
                AddCallbacks(instance);
            }
        }
        public ShootingActions @Shooting => new ShootingActions(this);

        // Pause
        private readonly InputActionMap m_Pause;
        private List<IPauseActions> m_PauseActionsCallbackInterfaces = new List<IPauseActions>();
        private readonly InputAction m_Pause_Pause;
        public struct PauseActions
        {
            private @CacheInput m_Wrapper;
            public PauseActions(@CacheInput wrapper) { m_Wrapper = wrapper; }
            public InputAction @Pause => m_Wrapper.m_Pause_Pause;
            public InputActionMap Get() { return m_Wrapper.m_Pause; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(PauseActions set) { return set.Get(); }
            public void AddCallbacks(IPauseActions instance)
            {
                if (instance == null || m_Wrapper.m_PauseActionsCallbackInterfaces.Contains(instance)) return;
                m_Wrapper.m_PauseActionsCallbackInterfaces.Add(instance);
                @Pause.started += instance.OnPause;
                @Pause.performed += instance.OnPause;
                @Pause.canceled += instance.OnPause;
            }

            private void UnregisterCallbacks(IPauseActions instance)
            {
                @Pause.started -= instance.OnPause;
                @Pause.performed -= instance.OnPause;
                @Pause.canceled -= instance.OnPause;
            }

            public void RemoveCallbacks(IPauseActions instance)
            {
                if (m_Wrapper.m_PauseActionsCallbackInterfaces.Remove(instance))
                    UnregisterCallbacks(instance);
            }

            public void SetCallbacks(IPauseActions instance)
            {
                foreach (var item in m_Wrapper.m_PauseActionsCallbackInterfaces)
                    UnregisterCallbacks(item);
                m_Wrapper.m_PauseActionsCallbackInterfaces.Clear();
                AddCallbacks(instance);
            }
        }
        public PauseActions @Pause => new PauseActions(this);
        public interface IPlayerActions
        {
            void OnMovement(InputAction.CallbackContext context);
            void OnLook(InputAction.CallbackContext context);
            void OnJump(InputAction.CallbackContext context);
        }
        public interface IShootingActions
        {
            void OnShoot(InputAction.CallbackContext context);
        }
        public interface IPauseActions
        {
            void OnPause(InputAction.CallbackContext context);
        }
    }
}
