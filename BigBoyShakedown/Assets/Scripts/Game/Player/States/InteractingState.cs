using BigBoyShakedown.Game.Interactable;
using BigBoyShakedown.Player.Controller;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BigBoyShakedown.Player.State
{
    /// <summary>
    /// this class represents the state responsible for idling
    /// this class handles the following input events:
    ///     playerHit: handle, then switch state
    ///     playerTargetted: handle here
    ///     Dash: switch state
    ///     Interaction: switch state
    ///     Movement: save movement, switch state
    ///     Punch: switch state
    ///     death: not implemented
    /// </summary>
    public class InteractingState : State
    {
        bool playerTargeted;
        Interactable interactable;

        void FixedUpdate()
        {
            if (!interactable || !controller.InteractableInRange(interactable))
            {
                CancelInteraction();
                machine.SetState<IdlingState>();
            }
        }
        private void CancelInteraction()
        {
            interactable.CancelInteraction();
            interactable = null;
            playerTargeted = false;
        }
        private void StartInteraction()
        {
            interactable = controller.GetClosestInteractable();
            if (!interactable)
            {
                machine.SetState<IdlingState>();
                return;
            }
            interactable.StartInteraction(this.controller);
            this.machine.playerAppearance.PlayAnimation(Appearance.AnimatedAction.Interact);
        }

        #region inputHandlers
        private void OnInteractionCompleteHandler()
        {
            machine.SetState<IdlingState>();
        }

        /// <summary>
        /// Handles move event, raised by #PlayerInputHandler
        /// </summary>
        private void OnMovementInputHandler(Vector3 movement_)
        {
            if (movement_ != Vector3.zero)
            {
                carryOver.previousMovement = movement_;
                machine.SetState<MovingState>();
            }
        }

        /// <summary>
        /// Handles interaction event, raised by #PlayerInputHandler
        /// </summary>
        private void OnInteractionInputHandler()
        {
            CancelInteraction();
            machine.SetState<IdlingState>();
        }

        /// <summary>
        /// Handles punch event, raised by #PlayerInputHandler
        /// </summary>
        private void OnPunchInputHandler()
        {
            CancelInteraction();
            machine.SetState<PunchingState>();
        }

        /// <summary>
        /// Handles dash event, raised by #PlayerInputHandler
        /// </summary>
        private void OnDashInputHandler()
        {
            //switch to dash state when player is targeted
            if (playerTargeted)
            {
                CancelInteraction();
                machine.SetState<DashingState>();
            }
        }

        /// <summary>
        /// Handles targetted event, raised by #PlayerInputHandler
        /// </summary>
        private void OnPlayerTargetedHandler()
        {
            playerTargeted = true;
            Time.StartTimer(new VariableReference<bool>(() => playerTargeted, (val) => { playerTargeted = val; }).SetEndValue(false), .1f);
        }

        /// <summary>
        /// Handles hit event, raised by #PlayerInputHandler
        /// </summary>
        private void OnPlayerHitHandler(PlayerController from, float damageIntended, Vector3 knockbackDistanceIntended, float stunDurationIntended, bool ignoreSize)
        {
            if (ignoreSize || from.size > controller.size)
            {
                controller.ReceiveHit(from, damageIntended, knockbackDistanceIntended, stunDurationIntended);
                carryOver.stunDuration = stunDurationIntended;
                carryOver.knockbackDistance = knockbackDistanceIntended;
                CancelInteraction();
                machine.SetState<StunnedState>();
            }
            else
            {
                controller.ReceiveHit(from, damageIntended, Vector3.zero, 0f);
            }
        }
        #endregion

        #region eventOverrides
        protected override void OnStateEnter()
        {
            this.inputRelay.OnMovementInput += OnMovementInputHandler;
            this.inputRelay.OnInteractionInput += OnInteractionInputHandler;
            this.inputRelay.OnPunchInput += OnPunchInputHandler;
            this.inputRelay.OnDashInput += OnDashInputHandler;
            this.inputRelay.OnPlayerTargeted += OnPlayerTargetedHandler;
            this.inputRelay.OnPlayerHit += OnPlayerHitHandler;

            StartInteraction();
        }
        protected override void OnStateExit()
        {
            this.inputRelay.OnMovementInput -= OnMovementInputHandler;
            this.inputRelay.OnInteractionInput -= OnInteractionInputHandler;
            this.inputRelay.OnPunchInput -= OnPunchInputHandler;
            this.inputRelay.OnDashInput -= OnDashInputHandler;
            this.inputRelay.OnPlayerTargeted -= OnPlayerTargetedHandler;
            this.inputRelay.OnPlayerHit -= OnPlayerHitHandler;
        }
        protected override void OnStateInitialize(StateMachine machine = null)
        {

        }
        #endregion
    }
}
