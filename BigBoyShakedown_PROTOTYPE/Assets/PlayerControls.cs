// GENERATED AUTOMATICALLY FROM 'Assets/PlayerControls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @PlayerControls : IInputActionCollection, IDisposable
{
    private InputActionAsset asset;
    public @PlayerControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerControls"",
    ""maps"": [
        {
            ""name"": ""All"",
            ""id"": ""bec20ea7-212c-4322-bf6a-255cef458930"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Button"",
                    ""id"": ""9dd288e9-8ec8-492a-bdcb-0df50006aaad"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Punch"",
                    ""type"": ""Button"",
                    ""id"": ""196dff74-5512-4d71-a9e3-d53dc0af908f"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Grow"",
                    ""type"": ""Button"",
                    ""id"": ""088df195-d377-4101-bf84-9992a06df4ca"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Shrink"",
                    ""type"": ""Button"",
                    ""id"": ""12fda82b-a1fd-4f2c-a9ca-8e5e20d55ec2"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""bcad5a77-4988-45e0-a0f6-928247110b73"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": ""StickDeadzone(max=0.925)"",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2e50cc03-9f9a-41ba-9b35-55771854c515"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Punch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d3e75d33-7ec3-41be-ad17-4f8dccb1c113"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Grow"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""df523d26-a59c-4994-bcda-24de1a01a57c"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Shrink"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // All
        m_All = asset.FindActionMap("All", throwIfNotFound: true);
        m_All_Move = m_All.FindAction("Move", throwIfNotFound: true);
        m_All_Punch = m_All.FindAction("Punch", throwIfNotFound: true);
        m_All_Grow = m_All.FindAction("Grow", throwIfNotFound: true);
        m_All_Shrink = m_All.FindAction("Shrink", throwIfNotFound: true);
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

    // All
    private readonly InputActionMap m_All;
    private IAllActions m_AllActionsCallbackInterface;
    private readonly InputAction m_All_Move;
    private readonly InputAction m_All_Punch;
    private readonly InputAction m_All_Grow;
    private readonly InputAction m_All_Shrink;
    public struct AllActions
    {
        private @PlayerControls m_Wrapper;
        public AllActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_All_Move;
        public InputAction @Punch => m_Wrapper.m_All_Punch;
        public InputAction @Grow => m_Wrapper.m_All_Grow;
        public InputAction @Shrink => m_Wrapper.m_All_Shrink;
        public InputActionMap Get() { return m_Wrapper.m_All; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(AllActions set) { return set.Get(); }
        public void SetCallbacks(IAllActions instance)
        {
            if (m_Wrapper.m_AllActionsCallbackInterface != null)
            {
                @Move.started -= m_Wrapper.m_AllActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_AllActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_AllActionsCallbackInterface.OnMove;
                @Punch.started -= m_Wrapper.m_AllActionsCallbackInterface.OnPunch;
                @Punch.performed -= m_Wrapper.m_AllActionsCallbackInterface.OnPunch;
                @Punch.canceled -= m_Wrapper.m_AllActionsCallbackInterface.OnPunch;
                @Grow.started -= m_Wrapper.m_AllActionsCallbackInterface.OnGrow;
                @Grow.performed -= m_Wrapper.m_AllActionsCallbackInterface.OnGrow;
                @Grow.canceled -= m_Wrapper.m_AllActionsCallbackInterface.OnGrow;
                @Shrink.started -= m_Wrapper.m_AllActionsCallbackInterface.OnShrink;
                @Shrink.performed -= m_Wrapper.m_AllActionsCallbackInterface.OnShrink;
                @Shrink.canceled -= m_Wrapper.m_AllActionsCallbackInterface.OnShrink;
            }
            m_Wrapper.m_AllActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @Punch.started += instance.OnPunch;
                @Punch.performed += instance.OnPunch;
                @Punch.canceled += instance.OnPunch;
                @Grow.started += instance.OnGrow;
                @Grow.performed += instance.OnGrow;
                @Grow.canceled += instance.OnGrow;
                @Shrink.started += instance.OnShrink;
                @Shrink.performed += instance.OnShrink;
                @Shrink.canceled += instance.OnShrink;
            }
        }
    }
    public AllActions @All => new AllActions(this);
    public interface IAllActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnPunch(InputAction.CallbackContext context);
        void OnGrow(InputAction.CallbackContext context);
        void OnShrink(InputAction.CallbackContext context);
    }
}
