using BigBoyShakedown.Player.Controller;
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
        public event Action<Vector3> OnMovementInput;
        public event Action OnMovementStoppedInput;
        public event Action OnPunchInput;
        public event Action OnInteractionInput;
        public event Action OnDashInput;
        public event Action OnPlayerTargeted;
        public event Action<PlayerController, float, Vector3, float> OnPlayerHit;
        public event Action OnPlayerDeath;
        #endregion

        public void RelayPlayerHit(PlayerController from, float damageIntended, Vector3 knockbackDistanceIntended, float stunDurationIntended)
        {
            OnPlayerHit.Invoke(from, damageIntended, knockbackDistanceIntended, stunDurationIntended);
        }
        public void RelayPlayerTargeted()
        {
            OnPlayerTargeted.Invoke();
        }
        public void RelayPlayerDeath()
        {
            OnPlayerDeath.Invoke();
        }

        private void Awake()
        {
            input = GetComponent<PlayerInput>();

            OnMovementInput += (val) => { };
            OnMovementStoppedInput += () => { };
            OnPunchInput += () => { };
            OnInteractionInput += () => { };
            OnDashInput += () => { };
            OnPlayerTargeted += () => { };
            OnPlayerDeath += () => { };
            OnPlayerHit += (val1, val2, val3, val4) => { };
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
                        var move2D = callbackContext.ReadValue<Vector2>();
                        var move3D = new Vector3(move2D.x, 0f, move2D.y);
                        OnMovementInput.Invoke(move3D);
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
