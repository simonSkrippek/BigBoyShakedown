using BigBoyShakedown.Player.Controller;
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
        float dashDuration;
        bool dashing;
        Vector3 movement;
        float speed;

        private void Update()
        {
            if (!dashing)
            {
                machine.SetState<IdlingState>();
            }
        }
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
            controller.TryApplyMovement(-movement * speed * .1f);
        }

        private void StartDash()
        {
            machine.playerAppearance.PlayAnimation(Appearance.AnimatedAction.Dash);
            machine.playerAppearance.PlaySound("dodge");

            dashDuration = controller.metrics.PlayerDashDuration[controller.size - 1];
            speed = controller.metrics.PlayerDashSpeed[controller.size - 1];
            dashing = true;
            Time.StartTimer(new VariableReference<bool>(() => dashing, (val) => { dashing = val; }).SetEndValue(false), dashDuration);

            movement = carryOver.previousMovement;
            movement.y = 0;
            if (movement == Vector3.zero)
            {
                movement = this.transform.forward * -1;
            }
            else
            {
                Debug.Log("got cool moves");
            }

            movement.Normalize();
            controller.TurnIn(-movement);
            //Debug.Log(movement);
        }

        #region inputHandlers
        /// <summary>
        /// Handles death event, raised by #PlayerInputHandler
        /// </summary>
        private void OnPlayerDeathHandler(PlayerController player)
        {
            machine.SetState<IdlingState>();
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
