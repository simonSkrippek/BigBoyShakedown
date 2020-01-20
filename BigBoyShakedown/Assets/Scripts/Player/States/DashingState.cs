using BigBoyShakedown.Player.Controller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BigBoyShakedown.Player.State
{
    /// <summary>
    /// this class represents the state responsible for dashing.
    /// this class handles the following input events:
    ///     n/a
    /// </summary>
    public class DashingState : State
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
            controller.TryApplyMovement(movement);
        }

        #region eventOverrides
        protected override void OnStateEnter()
        {
            movement = carryOver.previousMovement;
            movement.y = 0;
            if (movement == Vector3.zero)
            {
                movement = this.transform.forward * -1;
            }
            movement.Normalize();
        }
        protected override void OnStateExit()
        {
        }
        protected override void OnStateInitialize(StateMachine machine = null)
        {

        }
        #endregion
    }
}
