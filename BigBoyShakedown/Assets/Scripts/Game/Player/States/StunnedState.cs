using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BigBoyShakedown.Player.Controller;

namespace BigBoyShakedown.Player.State
{
    /// <summary>
    /// this class represents the state responsible for stuns & knockback.
    /// this class handles the following input events:
    ///     playerHit: handle here
    ///     death: not implemented
    /// </summary>
    public class StunnedState : State
    {
        float stunTimer;
        bool stunned = true;
        Vector3 knockBack;
        float knockbackTimer;
        Vector3 knockbackPerUpdate;

        // Update is called once per frame
        void FixedUpdate()
        {
            controller.TryApplyMovement(knockbackPerUpdate);
        }

        private void StartStun()
        {
            stunTimer = carryOver.stunDuration;
            knockBack = carryOver.knockbackDistance;
            knockbackPerUpdate = knockBack / stunTimer * UnityEngine.Time.fixedDeltaTime;
            stunned = true;
            Time.StartTimer(new VariableReference<bool>(() => stunned, (val) => stunned = val), stunTimer);

            machine.playerAppearance.PlayAnimation(Appearance.AnimatedAction.GetHit);
        }

        #region inputHandlers
        /// <summary>
        /// Handles death event, raised by #PlayerInputHandler
        /// </summary>
        private void OnPlayerDeathHandler()
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// Handles playerHit event, raised by #PlayerInputHandler
        /// </summary>
        private void OnPlayerHitHandler(PlayerController from, float damageIntended, Vector3 knockbackDistanceIntended, float stunDurationIntended)
        {
            if (from.size > controller.size)
            {
                controller.ReceiveHit(from, damageIntended, knockbackDistanceIntended, stunDurationIntended);
                carryOver.stunDuration = stunDurationIntended;
                carryOver.knockbackDistance = knockbackDistanceIntended;
                StartStun();
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
            inputRelay.OnPlayerHit += OnPlayerHitHandler;
            inputRelay.OnPlayerDeath += OnPlayerDeathHandler;

            StartStun();
        }
        protected override void OnStateExit()
        {
            inputRelay.OnPlayerHit -= OnPlayerHitHandler;
            inputRelay.OnPlayerDeath -= OnPlayerDeathHandler;
        }
        protected override void OnStateInitialize(StateMachine machine = null)
        {
            base.OnStateInitialize(machine);
        }
        #endregion
    }
}
