using BigBoyShakedown.Player.Controller;
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
    public class IdlingState : State
    {
        bool playerTargeted;

        void FixedUpdate()
        {
            
        }

        #region inputHandlers
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
            else
            {
                Debug.Log("Tried to dash!");
                carryOver.stunDuration = .3f;
                machine.SetState<StunnedState>();
            }
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
        private void OnPlayerHitHandler(PlayerController from, float damageIntended, Vector3 knockbackDistanceIntended, float stunDurationIntended, bool ignoreSize)
        {
            if (ignoreSize || from.size > controller.size)
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
            Debug.Log("Logging idling state startup");
            Debug.Log("before subscribing to events");
            if (!inputRelay) Debug.LogError("relay is null");
            this.inputRelay.OnMovementInput += OnMovementInputHandler;
            this.inputRelay.OnInteractionInput += OnInteractionInputHandler;
            this.inputRelay.OnPunchInput += OnPunchInputHandler;
            this.inputRelay.OnDashInput += OnDashInputHandler;
            this.inputRelay.OnPlayerTargeted += OnPlayerTargetedHandler;
            this.inputRelay.OnPlayerHit += OnPlayerHitHandler;
            Debug.Log("after subscribing to events");

            //PLAY ANIMATION

            machine.playerAppearance.PlayAnimation(Appearance.AnimatedAction.Idle);
            Debug.Log("after starting animation");
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
