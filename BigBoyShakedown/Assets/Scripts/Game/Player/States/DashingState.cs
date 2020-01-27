using BigBoyShakedown.Player.Controller;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BigBoyShakedown.Player.State
{
    /// <summary>
    /// this class represents the state responsible for dashing.
    /// this class handles the following input events:
    ///     death: not implemented
    /// </summary>
    public class DashingState : State
    {
        Vector3 movement;
        float speed;

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
            controller.TryApplyMovement(movement * speed * .3f);
        }

        private void StartDash()
        {
            machine.playerAppearance.PlayAnimation(Appearance.AnimatedAction.Dash);

            speed = controller.metrics.PlayerDashSpeed[controller.size];

            movement = carryOver.previousMovement;
            movement.y = 0;
            if (movement == Vector3.zero)
            {
                movement = this.transform.forward * -1;
            }
            movement.Normalize();
        }
        
        #region inputHandlers
        private void OnPlayerDeathHandler()
        {
            throw new NotImplementedException();
        }
        private void OnRecoveryCompleteHandler()
        {
            machine.SetState<IdlingState>();
        }
        #endregion

        #region eventOverrides
        protected override void OnStateEnter()
        {
            inputRelay.OnPlayerDeath += OnPlayerDeathHandler;
            inputRelay.OnRecoveryComplete += OnRecoveryCompleteHandler;

            StartDash();
        }
        protected override void OnStateExit()
        {
            inputRelay.OnPlayerDeath -= OnPlayerDeathHandler;
            inputRelay.OnRecoveryComplete -= OnRecoveryCompleteHandler;
        }
        protected override void OnStateInitialize(StateMachine machine = null)
        {

        }
        #endregion
    }
}
