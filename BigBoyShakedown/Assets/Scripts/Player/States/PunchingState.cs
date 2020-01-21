using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BigBoyShakedown.Player.Input;
using System;
using BigBoyShakedown.Player.Controller;

namespace BigBoyShakedown.Player.State
{
    /// <summary>
    /// this class represents the state responsible for all punches in a normal combo.
    /// this class handles the following input events:
    ///     playerHit: handle, then switch state
    ///     Punch: handle here, switch to self
    ///     death: not implemented
    /// </summary>
    public class PunchingState : State
    {
        /// <summary>
        /// indicates how many punches have been executed in the current combo; min 1, max 3
        /// </summary>
        int comboCount;
        /// <summary>
        /// whether or not the punch effects have already been applied
        /// </summary>
        bool punchCompleted;

        /// <summary>
        /// length of wind-up, stored at start of punch
        /// </summary>
        public float windUpTime;
        /// <summary>
        /// whether or not the punch is in wind-up phase
        /// </summary>
        bool inWindUp;
        /// <summary>
        /// length of wind-up, stored at start of punch
        /// </summary>
        public float recoveryTime;
        /// <summary>
        /// whether or not the punch is in recovery phase
        /// </summary>
        bool inRecovery;
        /// <summary>
        /// if another punch input is registered during recovery, set to true
        /// </summary>
        bool comboChained;

        void FixedUpdate()
        {
            if (!inRecovery)
            {
                if (!inWindUp)
                {
                    if (!punchCompleted)
                    {
                        CompletePunch();

                        inRecovery = true;
                        Time.StartTimer(new VariableReference<bool>(() => inRecovery, (val) => { inRecovery = val; }).SetEndValue(false), recoveryTime);
                        punchCompleted = true;
                    }
                    else
                    {
                        if (comboChained)
                        {
                            machine.SetState<PunchingState>();
                        }
                        else
                        {
                            machine.SetState<IdlingState>();
                        }
                    }
                }
            }
        }
                
        /// <summary>
        /// starts punch, called when state is activated
        /// </summary>
        private void StartPunch()
        {
            inWindUp = true;
            Time.StartTimer(new VariableReference<bool>(() => inWindUp, (val) => { inWindUp = val; }).SetEndValue(false), windUpTime);
            comboCount++;
            if (comboCount > 3) comboCount = 1;

            var objectsInRange = controller.GetAllAttackablesInRange();
            var objectToSnapTo = controller.GetClosestAttackable(objectsInRange);
            if (objectToSnapTo) controller.TurnTo(objectToSnapTo.position);
            var objectsInCone = controller.GetAllAttackablesInAttackCone(objectsInRange);
            controller.TargetAttackables(objectsInCone);

            this.InvokeRepeating("TargetAllAttackables", .1f, .1f);
        }
       
        /// <summary>
        /// call when windup is over, before recovery starts
        /// </summary>
        private void CompletePunch()
        {
            var enemiesToAttack = controller.GetAllAttackablesInAttackCone(controller.GetAllAttackablesInRange());
            controller.HitAllAttackables(enemiesToAttack,
                                        controller.metrics.PlayerDamage[controller.size - 1, comboCount],
                                        controller.metrics.PlayerPunchKnockback[controller.size - 1, comboCount],
                                        controller.metrics.PlayerPunchStunDuration[controller.size - 1, comboCount]);
        }

        /// <summary>
        /// target all enemies inside of the attack cone
        /// is periodically called as long as this state is active
        /// </summary>
        public void TargetAllAttackables()
        {
            var objectsInRange = controller.GetAllAttackablesInRange();
            var objectsInCone = controller.GetAllAttackablesInAttackCone(objectsInRange);
            controller.TargetAttackables(objectsInCone);
        }

        #region inputHandlers
        /// <summary>
        /// Handles punch event, raised by #PlayerInputHandler
        /// </summary>
        private void OnPunchInputHandler()
        {
            if (inRecovery)
            {
                comboChained = true;
            }
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
            this.inputRelay.OnPunchInput += OnPunchInputHandler;
            this.inputRelay.OnPlayerHit += OnPlayerHitHandler;

            windUpTime = controller.metrics.PlayerWindUpTime[controller.size];
            recoveryTime = controller.metrics.PlayerPunchRecoveryTime[controller.size];

            StartPunch();
        }
        protected override void OnStateExit()
        {
            this.inputRelay.OnPunchInput -= OnPunchInputHandler;
            this.inputRelay.OnPlayerHit -= OnPlayerHitHandler;

            if (!comboChained) comboCount = 0;

            CancelInvoke("TargetAllAttackables");
        }
        protected override void OnStateInitialize(StateMachine machine = null)
        {
            comboCount = 0;
        }
        #endregion
    }
}
