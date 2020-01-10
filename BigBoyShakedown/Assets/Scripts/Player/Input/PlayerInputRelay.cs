﻿using BigBoyShakedown.Player.Controller;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BigBoyShakedown.Player.Input
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerInputRelay : MonoBehaviour
    {
        PlayerInput input;

        #region events
        public event Action<Vector2> OnMovementInput;
        public event Action OnMovementStoppedInput;
        public event Action OnPunchInput;
        public event Action OnInteractionInput;
        public event Action OnDashInput;
        public event Action OnPlayerTargeted;
        public event Action<PlayerController> OnPlayerHit;
        #endregion

        public void RelayPlayerHit(PlayerController other)
        {
            OnPlayerHit(other);
        }

        private void Awake()
        {
            input = GetComponent<PlayerInput>();
        }

        private void OnEnable()
        {
            input.onActionTriggered += OnActionTriggeredHandler;
        }
        private void OnDisable()
        {
            input.onActionTriggered -= OnActionTriggeredHandler;
        }

        private void OnActionTriggeredHandler(InputAction.CallbackContext callbackContext)
        {
            var actionName = callbackContext.action.name;
            if (callbackContext.performed)
            {
                switch (actionName)
                {
                    case "Movement":
                        OnMovementInput.Invoke(callbackContext.ReadValue<Vector2>());
                        return;
                    case "Punch":
                        OnPunchInput.Invoke();
                        return;
                    case "Interaction":
                        OnInteractionInput.Invoke();
                        return;
                    case "Dash":
                        OnDashInput.Invoke();
                        return;
                }
            }
            else
            {
                switch (actionName)
                {
                    case "Movement":
                        OnMovementStoppedInput.Invoke();
                        return;
                }
            }
        }


    }
}