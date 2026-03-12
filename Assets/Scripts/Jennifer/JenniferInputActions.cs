// Auto-generated Input Actions class
// Generated from JenniferInputActions.inputactions

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @JenniferInputActions : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }

    public @JenniferInputActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""JenniferInputActions"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""a1b2c3d4-e5f6-7890-abcd-ef1234567890"",
            ""actions"": [
                { ""name"": ""Move"", ""type"": ""Value"", ""id"": ""b2c3d4e5-f6a7-8901-bcde-f12345678901"", ""expectedControlType"": ""Vector2"", ""processors"": """", ""interactions"": """", ""initialStateCheck"": true },
                { ""name"": ""Look"", ""type"": ""Value"", ""id"": ""c3d4e5f6-a7b8-9012-cdef-123456789012"", ""expectedControlType"": ""Vector2"", ""processors"": """", ""interactions"": """", ""initialStateCheck"": false },
                { ""name"": ""Interact"", ""type"": ""Button"", ""id"": ""d4e5f6a7-b8c9-0123-defa-234567890123"", ""expectedControlType"": ""Button"", ""processors"": """", ""interactions"": """", ""initialStateCheck"": false },
                { ""name"": ""Intimidate"", ""type"": ""Button"", ""id"": ""e5f6a7b8-c9d0-1234-efab-345678901234"", ""expectedControlType"": ""Button"", ""processors"": """", ""interactions"": """", ""initialStateCheck"": false },
                { ""name"": ""Pause"", ""type"": ""Button"", ""id"": ""f6a7b8c9-d0e1-2345-fabc-456789012345"", ""expectedControlType"": ""Button"", ""processors"": """", ""interactions"": """", ""initialStateCheck"": false }
            ],
            ""bindings"": [
                { ""name"": ""WASD"", ""id"": ""a7b8c9d0-e1f2-3456-abcd-567890123456"", ""path"": ""2DVector"", ""interactions"": """", ""processors"": """", ""groups"": """", ""action"": ""Move"", ""isComposite"": true, ""isPartOfComposite"": false },
                { ""name"": ""up"", ""id"": ""b8c9d0e1-f2a3-4567-bcde-678901234567"", ""path"": ""<Keyboard>/w"", ""interactions"": """", ""processors"": """", ""groups"": """", ""action"": ""Move"", ""isComposite"": false, ""isPartOfComposite"": true },
                { ""name"": ""down"", ""id"": ""c9d0e1f2-a3b4-5678-cdef-789012345678"", ""path"": ""<Keyboard>/s"", ""interactions"": """", ""processors"": """", ""groups"": """", ""action"": ""Move"", ""isComposite"": false, ""isPartOfComposite"": true },
                { ""name"": ""left"", ""id"": ""d0e1f2a3-b4c5-6789-defa-890123456789"", ""path"": ""<Keyboard>/a"", ""interactions"": """", ""processors"": """", ""groups"": """", ""action"": ""Move"", ""isComposite"": false, ""isPartOfComposite"": true },
                { ""name"": ""right"", ""id"": ""e1f2a3b4-c5d6-7890-efab-901234567890"", ""path"": ""<Keyboard>/d"", ""interactions"": """", ""processors"": """", ""groups"": """", ""action"": ""Move"", ""isComposite"": false, ""isPartOfComposite"": true },
                { ""name"": """", ""id"": ""f2a3b4c5-d6e7-8901-fabc-012345678901"", ""path"": ""<Mouse>/delta"", ""interactions"": """", ""processors"": """", ""groups"": """", ""action"": ""Look"", ""isComposite"": false, ""isPartOfComposite"": false },
                { ""name"": """", ""id"": ""a3b4c5d6-e7f8-9012-abcd-123456789012"", ""path"": ""<Keyboard>/e"", ""interactions"": """", ""processors"": """", ""groups"": """", ""action"": ""Interact"", ""isComposite"": false, ""isPartOfComposite"": false },
                { ""name"": """", ""id"": ""b4c5d6e7-f8a9-0123-bcde-234567890123"", ""path"": ""<Keyboard>/f"", ""interactions"": """", ""processors"": """", ""groups"": """", ""action"": ""Intimidate"", ""isComposite"": false, ""isPartOfComposite"": false },
                { ""name"": """", ""id"": ""c5d6e7f8-a9b0-1234-cdef-345678901234"", ""path"": ""<Keyboard>/escape"", ""interactions"": """", ""processors"": """", ""groups"": """", ""action"": ""Pause"", ""isComposite"": false, ""isPartOfComposite"": false }
            ]
        }
    ],
    ""controlSchemes"": []
}");

        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_Move = m_Player.FindAction("Move", throwIfNotFound: true);
        m_Player_Look = m_Player.FindAction("Look", throwIfNotFound: true);
        m_Player_Interact = m_Player.FindAction("Interact", throwIfNotFound: true);
        m_Player_Intimidate = m_Player.FindAction("Intimidate", throwIfNotFound: true);
        m_Player_Pause = m_Player.FindAction("Pause", throwIfNotFound: true);
    }

    ~@JenniferInputActions() { UnityEngine.Debug.Assert(!asset.enabled); }

    public void Dispose() { UnityEngine.Object.Destroy(asset); }

    public InputBinding? bindingMask { get => asset.bindingMask; set => asset.bindingMask = value; }
    public ReadOnlyArray<InputDevice>? devices { get => asset.devices; set => asset.devices = value; }
    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;
    public bool Contains(InputAction action) => asset.Contains(action);
    public IEnumerator<InputAction> GetEnumerator() => asset.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public void Enable() => asset.Enable();
    public void Disable() => asset.Disable();
    public IEnumerable<InputBinding> bindings => asset.bindings;
    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false) => asset.FindAction(actionNameOrId, throwIfNotFound);
    public int FindBinding(InputBinding bindingMask, out InputAction action) => asset.FindBinding(bindingMask, out action);

    // Player Action Map
    private readonly InputActionMap m_Player;
    private readonly InputAction m_Player_Move;
    private readonly InputAction m_Player_Look;
    private readonly InputAction m_Player_Interact;
    private readonly InputAction m_Player_Intimidate;
    private readonly InputAction m_Player_Pause;

    public struct PlayerActions
    {
        private @JenniferInputActions m_Wrapper;
        public PlayerActions(@JenniferInputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction Move => m_Wrapper.m_Player_Move;
        public InputAction Look => m_Wrapper.m_Player_Look;
        public InputAction Interact => m_Wrapper.m_Player_Interact;
        public InputAction Intimidate => m_Wrapper.m_Player_Intimidate;
        public InputAction Pause => m_Wrapper.m_Player_Pause;
        public InputActionMap Get() => m_Wrapper.m_Player;
        public void Enable() => Get().Enable();
        public void Disable() => Get().Disable();
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) => set.Get();
    }

    public PlayerActions Player => new PlayerActions(this);
}
