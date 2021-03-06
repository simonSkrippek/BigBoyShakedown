﻿using BigBoyShakedown.Player.Controller;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BigBoyShakedown.Player.Input
{
    public class PlayerInputRelay : MonoBehaviour
    {
        [SerializeField]
        public PlayerInput input;
        [SerializeField]
        AnimationCallbackRelay[] animationCallbackRelays;

        #region events
        #region playerInputEvents
        /// <summary>
        /// raised upon movement input from playerInput
        /// </summary>
        public event Action<Vector3> OnMovementInput;
        /// <summary>
        /// raised when movement input is no longer continued from playerInput
        /// </summary>
        public event Action OnMovementStoppedInput;
        /// <summary>
        /// raised upon punch input from playerInput
        /// </summary>
        public event Action OnPunchInput;
        /// <summary>
        /// raised upon interaction input from playerInput
        /// </summary>
        public event Action OnInteractionInput;
        /// <summary>
        /// raised upon dash input from playerInput
        /// </summary>
        public event Action OnDashInput;
        #endregion

        #region playerControllerEvents
        /// <summary>
        /// raised upon completing an interaction
        /// </summary>
        public event Action OnInteractionComplete;
        /// <summary>
        /// raised upon cancelling an interaction
        /// </summary>
        public event Action OnInteractionCancelled;
        /// <summary>
        /// raised upon being targetted from playerController
        /// </summary>
        public event Action OnPlayerTargeted;
        /// <summary>
        /// raised upon being hit from playerController
        /// </summary>
        public event Action<PlayerController, float, Vector3, float, bool> OnPlayerHit;
        /// <summary>
        /// raised upon dying from playerController
        /// </summary>
        public event Action<PlayerController> OnPlayerDeath;
        /// <summary>
        /// raised upon size change from playerController
        /// </summary>
        public event Action<int> OnPlayerSizeChanged;
        /// <summary>
        /// raised upon score change from playerController
        /// </summary>
        public event Action<float> OnPlayerScoreChanged;
        #endregion

        #region animationEvents
        /// <summary>
        /// raised upon windUp-animation completion from animationCallbackRelay
        /// </summary>
        public event Action OnWindUpComplete;
        /// <summary>
        /// raised upon recovery-animation completion from animationCallbackRelay
        /// </summary>
        public event Action OnRecoveryComplete;
        #endregion
        #endregion


        #region PlayerControllerRelays
        /// <summary>
        /// relay that an interaction was completed
        /// </summary>
        public void RelayInteractionComplete()
        {
            OnInteractionComplete?.Invoke();
        }
        /// <summary>
        /// relay that an interaction was canceled
        /// </summary>
        public void RelayInteractionCanceled()
        {
            OnInteractionCancelled?.Invoke();
        }        
        /// <summary>
        /// relay a hit the playercontroller noticed
        /// </summary>
        /// <param name="from">the attacking playercontroller</param>
        /// <param name="damageIntended">the damage the other player intends to inflict</param>
        /// <param name="knockbackDistanceIntended">the knockback the other player intends to inflict</param>
        /// <param name="stunDurationIntended">the stun the other player intends to inflict</param>
        public void RelayPlayerHit(PlayerController from, float damageIntended, Vector3 knockbackDistanceIntended, float stunDurationIntended, bool ignoreSize)
        {
            OnPlayerHit.Invoke(from, damageIntended, knockbackDistanceIntended, stunDurationIntended, ignoreSize);
        }
        /// <summary>
        /// relay that this playerController has been targetted for an attack
        /// </summary>
        public void RelayPlayerTargeted()
        {
            OnPlayerTargeted.Invoke();
        }
        /// <summary>
        /// relay that this playerController has died
        /// </summary>
        public void RelayPlayerDeath()
        {
            OnPlayerDeath?.Invoke(this.GetComponent<PlayerController>());
        }
        /// <summary>
        /// relay that this playerController has changed it's size
        /// </summary>
        /// <param name="toSize">player's new size</param>
        public void RelayPlayerSizeChange(int toSize)
        {
            OnPlayerSizeChanged.Invoke(toSize);
        }
        /// <summary>
        /// relay that this playerController has changed it's score
        /// </summary>
        /// <param name="toSize">player's new size</param>
        public void RelayPlayerScoreChange (float scoreChange)
        {
            OnPlayerScoreChanged.Invoke(scoreChange);
        }
        #endregion

        #region unityEvents
        private void Awake()
        {
            //animationCallbackRelays = GetComponentsInChildren<AnimationCallbackRelay>();
            if (animationCallbackRelays.Length < 5) throw new MissingMemberException("not all animationCallbackRelay found in children");

            OnMovementInput += (val) => { };
            OnMovementStoppedInput += () => { };
            OnPunchInput += () => { };
            OnInteractionInput += () => { };
            OnDashInput += () => { };
            OnPlayerTargeted += () => { };
            OnPlayerDeath += (val) => { };
            OnPlayerHit += (val1, val2, val3, val4, val5) => { };
            OnPlayerScoreChanged += (val) => { };
            OnPlayerSizeChanged += (val) => { };

        }
        private void OnEnable()
        {
            if (!input) throw new Exception("no input module set");
            //subscribe to playerInputEvents
            input.onActionTriggered += OnActionTriggeredHandler;
            //subscribe to animationCallbackEvents
            foreach (var animationCallbackRelay in animationCallbackRelays)
            {
                animationCallbackRelay.OnWindUpComplete += OnWindUpCompleteHandler;
                animationCallbackRelay.OnRecoveryComplete += OnRecoveryCompleteHandler;
            }
        }
        private void OnDisable()
        {
            input.onActionTriggered -= OnActionTriggeredHandler;
            foreach (var animationCallbackRelay in animationCallbackRelays)
            {
                animationCallbackRelay.OnWindUpComplete -= OnWindUpCompleteHandler;
                animationCallbackRelay.OnRecoveryComplete -= OnRecoveryCompleteHandler;
            }
        }
        public void ActivateInput(PlayerInput input_)
        {
            if (input) throw new Exception("Input already set");
            if (!input_) throw new Exception("Input cannot be null");
            input = input_;
        }
        #endregion

        #region eventHandlers
        private void OnWindUpCompleteHandler(AnimationCallbackRelay source)
        {
            if (source.isActiveAndEnabled) OnWindUpComplete?.Invoke();
        }
        private void OnRecoveryCompleteHandler(AnimationCallbackRelay source)
        {
            if (source.isActiveAndEnabled) OnRecoveryComplete?.Invoke();
        }
        private void OnActionTriggeredHandler(InputAction.CallbackContext callbackContext)
        {
            //Debug.Log("action received");
            var actionName = callbackContext.action.name;
            if (callbackContext.performed)
            {
                switch (actionName)
                {
                    case "Movement":
                        var move2D = callbackContext.ReadValue<Vector2>();
                        var move3D = new Vector3(move2D.x, 0f, move2D.y);
                        OnMovementInput?.Invoke(move3D);
                        return;
                    case "Punch":
                        OnPunchInput?.Invoke();
                        return;
                    case "Interaction":
                        OnInteractionInput?.Invoke();
                        return;
                    case "Dash":
                        OnDashInput?.Invoke();
                        return;
                }
            }
            else
            {
                switch (actionName)
                {
                    case "Movement":
                        OnMovementStoppedInput?.Invoke();
                        return;
                }
            }
        }
        #endregion
    }
}
