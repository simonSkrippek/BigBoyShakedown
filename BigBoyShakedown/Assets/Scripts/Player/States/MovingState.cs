using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BigBoyShakedown.Player.Input;
using System;
using BigBoyShakedown.Player.Controller;

namespace BigBoyShakedown.Player.State
{
    public class MovingState : State
    {
        bool playerTargeted;

        Vector3 movement;

        void Update()
        {
            //calculate y movement
            if (!controller.IsGrounded())
            {
                movement.y = controller.metrics.PlayerYMoveSpeed[controller.size];
            }
            //1. check for collision
            //2. move
            controller.TryApplyMovement(movement);
        }
        protected override void OnStateEnter()
        {
            this.inputRelay.OnMovementInput += OnMovementInputHandler;
            this.inputRelay.OnInteractionInput += OnInteractionInputHandler;
            this.inputRelay.OnPunchInput += OnPunchInputHandler;
            this.inputRelay.OnDashInput += OnDashInputHandler;
            this.inputRelay.OnPlayerTargeted += OnPlayerTargetedHandler;
            this.inputRelay.OnPlayerHit += OnPlayerHitHandler;
        }

        private void OnMovementInputHandler(Vector2 movement_)
        {
            if (movement_ == Vector2.zero)
            {
                machine.SetState<IdlingState>();
            }
            else
            {
                movement.x = movement_.x;
                movement.z = movement_.y;
            }
        }

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

        private void OnPunchInputHandler()
        {
            machine.SetState<PunchingState>();
        }

        private void OnDashInputHandler()
        {
            //switch to dash state when player is targeted
            if (playerTargeted) machine.SetState<DashingState>();
        }

        private void OnPlayerTargetedHandler()
        {
            playerTargeted = true;
            Time.StartTimer(new VariableReference<bool>(() => playerTargeted, (val) => { playerTargeted = val; }).SetEndValue(false), .2f);
        }

        private void OnPlayerHitHandler(PlayerController otherPlayer)
        {
            if (otherPlayer.size > controller.size)
            {

            }
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
    }
}
