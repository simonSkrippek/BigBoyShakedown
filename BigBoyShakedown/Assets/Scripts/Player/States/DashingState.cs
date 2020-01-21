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
        bool playerTargeted;

        Vector3 movement;
        float duration;
        float speed;
        bool dashing;

        void FixedUpdate()
        {
            if (dashing)
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
                controller.TryApplyMovement(movement * speed);
            }
            else
            {
                machine.SetState<IdlingState>();
            }
        }

        private void StartDash()
        {
            speed = controller.metrics.PlayerDashSpeed[controller.size];
            duration = controller.metrics.PlayerDashDuration[controller.size - 1];
            Time.StartTimer(new VariableReference<bool>(() => dashing, (val) => dashing = val), duration);
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
        #endregion

        #region eventOverrides
        protected override void OnStateEnter()
        {
            inputRelay.OnPlayerDeath += OnPlayerDeathHandler;

            StartDash();
        }
        protected override void OnStateExit()
        {
            inputRelay.OnPlayerDeath -= OnPlayerDeathHandler;
        }
        protected override void OnStateInitialize(StateMachine machine = null)
        {

        }
        #endregion
    }
}
