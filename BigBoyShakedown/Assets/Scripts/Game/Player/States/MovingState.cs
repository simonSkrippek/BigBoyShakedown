using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BigBoyShakedown.Player.Input;
using System;
using BigBoyShakedown.Player.Controller;

namespace BigBoyShakedown.Player.State
{
    /// <summary>
    /// this class represents the state responsible for normal movement.
    /// this class handles the following input events:
    ///     playerHit: handle, then switch state
    ///     playerTargetted: handle here
    ///     Dash: switch state
    ///     Interaction: switch state
    ///     Movement: handle here
    ///     Punch: switch state
    ///     death: not implemented
    /// </summary>
    public class MovingState : State
    {
        bool playerTargeted;

        Vector3 movement;

        void FixedUpdate()
        {
            //calculate y movement
            if (!controller.IsGrounded())
            {
                movement.y = controller.metrics.PlayerYMoveSpeed[controller.size];
            }
            else
            {
                movement.y = 0f;
            }
            //1. check for collision
            //2. move
            controller.TurnIn(-movement);
            controller.TryApplyMovement(-movement * controller.metrics.PlayerMoveSpeed[controller.size-1] * .1f);
        }

        #region inputHandlers
        /// <summary>
        /// Handles move event, raised by #PlayerInputHandler
        /// </summary>
        private void OnMovementInputHandler(Vector3 movement_)
        {
            if (movement_ == Vector3.zero)
            {
                machine.SetState<IdlingState>();
            }
            else
            {
                movement = movement_;
            }
        }
        /// <summary>
        /// Handles move event, raised by #PlayerInputHandler
        /// </summary>
        private void OnMovementStoppedInputHandler()
        {
            machine.SetState<IdlingState>();
        }

        /// <summary>
        /// Handles interaction event, raised by #PlayerInputHandler
        /// </summary>
        private void OnInteractionInputHandler()
        {
            //switch to interaction state
            if (controller.InteractableInRange())
            {
                machine.SetState<InteractingState>();
            }
            else
            {

            }
        }

        /// <summary>
        /// Handles punch event, raised by #PlayerInputHandler
        /// </summary>
        private void OnPunchInputHandler()
        {
            machine.SetState<PunchingState>();
        }

        /// <summary>
        /// Handles dash event, raised by #PlayerInputHandler
        /// </summary>
        private void OnDashInputHandler()
        {
            //switch to dash state when player is targeted
            if (playerTargeted) machine.SetState<DashingState>();
            else Debug.Log("Tried to dash!");
        }

        /// <summary>
        /// Handles targetted event, raised by #PlayerInputHandler
        /// </summary>
        private void OnPlayerTargetedHandler()
        {
            playerTargeted = true;
            //Debug.Log("playerTargeted: " + this.gameObject.name);
            Time.StartTimer(new VariableReference<bool>(() => playerTargeted, (val) => { playerTargeted = val; }).SetEndValue(false), .1f);
        }

        /// <summary>
        /// Handles hit event, raised by #PlayerInputHandler
        /// </summary>
        private void OnPlayerHitHandler(PlayerController from, float damageIntended, Vector3 knockbackDistanceIntended, float stunDurationIntended) 
        {
            if (from.size > controller.size)
            {
                controller.ReceiveHit(from, damageIntended, knockbackDistanceIntended, stunDurationIntended);
                carryOver.stunDuration = stunDurationIntended;
                carryOver.knockbackDistance = knockbackDistanceIntended;
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
            this.inputRelay.OnMovementStoppedInput += OnMovementStoppedInputHandler;
            this.inputRelay.OnInteractionInput += OnInteractionInputHandler;
            this.inputRelay.OnPunchInput += OnPunchInputHandler;
            this.inputRelay.OnDashInput += OnDashInputHandler;
            this.inputRelay.OnPlayerTargeted += OnPlayerTargetedHandler;
            this.inputRelay.OnPlayerHit += OnPlayerHitHandler;

            movement = carryOver.previousMovement;
            //PLAY ANIMATION

            machine.playerAppearance.PlayAnimation(Appearance.AnimatedAction.Run);
        }
        protected override void OnStateExit()
        {
            this.inputRelay.OnMovementInput -= OnMovementInputHandler;
            this.inputRelay.OnMovementStoppedInput -= OnMovementStoppedInputHandler;
            this.inputRelay.OnInteractionInput -= OnInteractionInputHandler;
            this.inputRelay.OnPunchInput -= OnPunchInputHandler;
            this.inputRelay.OnDashInput -= OnDashInputHandler;
            this.inputRelay.OnPlayerTargeted -= OnPlayerTargetedHandler;
            this.inputRelay.OnPlayerHit -= OnPlayerHitHandler;

            carryOver.previousMovement = movement;
        }
        protected override void OnStateInitialize(StateMachine machine = null)
        {

        }
        #endregion
    }
}
