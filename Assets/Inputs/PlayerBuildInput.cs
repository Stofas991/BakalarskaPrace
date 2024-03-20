//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.4.4
//     from Assets/Inputs/PlayerBuildInput.inputactions
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

public partial class @PlayerBuildInput : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerBuildInput()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerBuildInput"",
    ""maps"": [
        {
            ""name"": ""Building"",
            ""id"": ""02fe41d6-b6a2-46a6-b4b1-b4e7156c0a68"",
            ""actions"": [
                {
                    ""name"": ""MouseLeftClick"",
                    ""type"": ""Button"",
                    ""id"": ""4127d1b0-e9ee-4306-9836-2d92b0294322"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Tap,SlowTap"",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""MouseRightClick"",
                    ""type"": ""Button"",
                    ""id"": ""cead39c4-be9b-4498-9201-47d7a3c1402a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""MousePosition"",
                    ""type"": ""Value"",
                    ""id"": ""99aba44e-5c6b-4837-b558-7d07fafc3e79"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""34e56ae8-9b30-4cb2-af0e-6857f4fe7bdf"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MouseLeftClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""56c763af-93f5-4883-a282-235625b9215e"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MouseRightClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""07c2fff2-a136-4bb2-b006-aa6af3edc560"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MousePosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Building
        m_Building = asset.FindActionMap("Building", throwIfNotFound: true);
        m_Building_MouseLeftClick = m_Building.FindAction("MouseLeftClick", throwIfNotFound: true);
        m_Building_MouseRightClick = m_Building.FindAction("MouseRightClick", throwIfNotFound: true);
        m_Building_MousePosition = m_Building.FindAction("MousePosition", throwIfNotFound: true);
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

    // Building
    private readonly InputActionMap m_Building;
    private IBuildingActions m_BuildingActionsCallbackInterface;
    private readonly InputAction m_Building_MouseLeftClick;
    private readonly InputAction m_Building_MouseRightClick;
    private readonly InputAction m_Building_MousePosition;
    public struct BuildingActions
    {
        private @PlayerBuildInput m_Wrapper;
        public BuildingActions(@PlayerBuildInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @MouseLeftClick => m_Wrapper.m_Building_MouseLeftClick;
        public InputAction @MouseRightClick => m_Wrapper.m_Building_MouseRightClick;
        public InputAction @MousePosition => m_Wrapper.m_Building_MousePosition;
        public InputActionMap Get() { return m_Wrapper.m_Building; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(BuildingActions set) { return set.Get(); }
        public void SetCallbacks(IBuildingActions instance)
        {
            if (m_Wrapper.m_BuildingActionsCallbackInterface != null)
            {
                @MouseLeftClick.started -= m_Wrapper.m_BuildingActionsCallbackInterface.OnMouseLeftClick;
                @MouseLeftClick.performed -= m_Wrapper.m_BuildingActionsCallbackInterface.OnMouseLeftClick;
                @MouseLeftClick.canceled -= m_Wrapper.m_BuildingActionsCallbackInterface.OnMouseLeftClick;
                @MouseRightClick.started -= m_Wrapper.m_BuildingActionsCallbackInterface.OnMouseRightClick;
                @MouseRightClick.performed -= m_Wrapper.m_BuildingActionsCallbackInterface.OnMouseRightClick;
                @MouseRightClick.canceled -= m_Wrapper.m_BuildingActionsCallbackInterface.OnMouseRightClick;
                @MousePosition.started -= m_Wrapper.m_BuildingActionsCallbackInterface.OnMousePosition;
                @MousePosition.performed -= m_Wrapper.m_BuildingActionsCallbackInterface.OnMousePosition;
                @MousePosition.canceled -= m_Wrapper.m_BuildingActionsCallbackInterface.OnMousePosition;
            }
            m_Wrapper.m_BuildingActionsCallbackInterface = instance;
            if (instance != null)
            {
                @MouseLeftClick.started += instance.OnMouseLeftClick;
                @MouseLeftClick.performed += instance.OnMouseLeftClick;
                @MouseLeftClick.canceled += instance.OnMouseLeftClick;
                @MouseRightClick.started += instance.OnMouseRightClick;
                @MouseRightClick.performed += instance.OnMouseRightClick;
                @MouseRightClick.canceled += instance.OnMouseRightClick;
                @MousePosition.started += instance.OnMousePosition;
                @MousePosition.performed += instance.OnMousePosition;
                @MousePosition.canceled += instance.OnMousePosition;
            }
        }
    }
    public BuildingActions @Building => new BuildingActions(this);
    public interface IBuildingActions
    {
        void OnMouseLeftClick(InputAction.CallbackContext context);
        void OnMouseRightClick(InputAction.CallbackContext context);
        void OnMousePosition(InputAction.CallbackContext context);
    }
}