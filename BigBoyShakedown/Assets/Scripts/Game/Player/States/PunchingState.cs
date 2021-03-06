﻿using UnityEngine;
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
        /// whether or not the punch is in recovery phase
        /// </summary>
        bool inRecovery;
        /// <summary>
        /// if another punch input is registered during recovery, set to true
        /// </summary>
        bool comboChained;

        Vector3 moveDirection;
        float moveSpeed;
        bool moving;


        private void FixedUpdate()
        {
            if (moving) controller.TryApplyMovement(moveDirection * moveSpeed * .01f);
        }


        /// <summary>
        /// starts punch, called when state is activated
        /// </summary>
        private void StartPunch()
        {
            comboCount++;
            if (comboCount > 3) comboCount = 1;

            //target all attackables 1st time, initiate retargetting
            var objectsInRange = controller.GetAllAttackablesInRange();
            var objectToSnapTo = controller.GetClosestAttackable(objectsInRange);
            if (objectToSnapTo) controller.TurnTo(objectToSnapTo.position);
            var objectsInCone = controller.GetAllAttackablesInAttackCone(objectsInRange);
            controller.TargetAttackables(objectsInCone);

            this.InvokeRepeating("TargetAllAttackables", .1f, .1f);

            //calculate movement vars
            float animationDuration = controller.metrics.PlayerPunchAnimationDurationIntended[controller.size - 1, comboCount - 1];
            float animationSpeedMultiplier = controller.metrics.PlayerPunchAnimationFixedDuration[controller.size - 1] / animationDuration;
            float movementStartPointPercent = controller.metrics.PlayerPunchMovementStartPoint[controller.size - 1, comboCount - 1];
            float movementStartPointSeconds = movementStartPointPercent * animationDuration;
            moveDirection = this.transform.forward.normalized;
            float movementDistance = controller.metrics.PlayerPunchForwardMovementDistance[controller.size - 1, comboCount - 1];
            moveSpeed = movementDistance / (animationDuration - movementStartPointSeconds);

            moving = false;
            Time.StartTimer(new VariableReference<bool>(() => moving, (val) => { moving = val; }).SetEndValue(true), movementStartPointSeconds);

            //Debug.Log("Punch Started! \n Combo Count: " + comboCount);
            switch (comboCount)
            {
                case 1:
                    machine.playerAppearance.PlayAnimation(Appearance.AnimatedAction.Punch1, animationSpeedMultiplier);
                    break;
                case 2:
                    machine.playerAppearance.PlayAnimation(Appearance.AnimatedAction.Punch2, animationSpeedMultiplier);
                    break;
                case 3:
                    machine.playerAppearance.PlayAnimation(Appearance.AnimatedAction.Punch3, animationSpeedMultiplier);
                    break;
            }
            machine.playerAppearance.ScaleAttackIndicator(controller.metrics.PlayerPunchRange[controller.size - 1]);
            machine.playerAppearance.ShowAttackIndicator();
        }       
        /// <summary>
        /// call when windup is over, before recovery starts
        /// </summary>
        private void LandPunch()
        {
            machine.playerAppearance.HideAttackIndicator();
            machine.playerAppearance.PlaySound("swing_pitch_1");
            this.CancelInvoke("TargetAllAttackables");
            var enemiesToAttack = controller.GetAllAttackablesInAttackCone(controller.GetAllAttackablesInRange());
            if (enemiesToAttack.Length > 0) machine.playerAppearance.PlaySound("combo_impact_" + comboCount);
            controller.HitAllAttackables(enemiesToAttack,
                                        controller.metrics.PlayerDamage[controller.size - 1, comboCount-1],
                                        controller.metrics.PlayerPunchKnockback[controller.size - 1, comboCount-1],
                                        controller.metrics.PlayerPunchStunDuration[controller.size - 1, comboCount-1],
                                        false);

            //Debug.Log("Punch Landed!" + "\n chained: " + comboChained.ToString() + "\n count: " + comboCount);
            inRecovery = true;
        }
        /// <summary>
        /// call when recovery is over
        /// </summary>
        private void FinishPunch()
        {
            //Debug.Log("Punch Finished");
            if (comboChained)
            {
                if (!machine.SetState<PunchingState>()) throw new Exception("Switching states unsuccessful!");
            }
            else
            {
                if (!machine.SetState<IdlingState>()) throw new Exception("Switching states unsuccessful!");
            }
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
        /// <summary>
        /// call when punch is interrupted, revert ro normal state
        /// </summary>
        private void CancelPunch()
        {
            machine.playerAppearance.HideAttackIndicator();
        }

        #region inputHandlers
        /// <summary>
        /// Handles death event, raised by #PlayerInputHandler
        /// </summary>
        private void OnPlayerDeathHandler(PlayerController player)
        {
            CancelPunch();
            machine.SetState<IdlingState>();
        }
        /// <summary>
        /// Handles punch event, raised by #PlayerInputHandler
        /// </summary>
        private void OnPunchInputHandler()
        {
            if (inRecovery && comboCount < 3)
            {
                comboChained = true;
            }
        }
        /// <summary>
        /// Handles hit event, raised by #PlayerInputHandler
        /// </summary>
        private void OnPlayerHitHandler(PlayerController from, float damageIntended, Vector3 knockbackDistanceIntended, float stunDurationIntended, bool ignoreSize)
        {
            if (ignoreSize || from.size >= controller.size)
            {
                controller.ReceiveHit(from, damageIntended, knockbackDistanceIntended, stunDurationIntended);
                carryOver.stunDuration = stunDurationIntended;
                carryOver.knockbackDistance = knockbackDistanceIntended;
                CancelPunch();
                machine.SetState<StunnedState>();
            }
            else
            {
                controller.ReceiveHit(from, damageIntended, Vector3.zero, 0f);
            }
        }
        /// <summary>
        /// Handles windUpComplete event, raised by #PlayerInputHandler
        /// </summary>
        private void OnWindUpCompleteHandler()
        {
            //Debug.Log("windup done!");
            LandPunch();
        }
        /// <summary>
        /// Handles recoveryComplete event, raised by #PlayerInputHandler
        /// </summary>
        private void OnRecoveryCompleteHandler()
        {
            //Debug.Log("recovery done!");
            FinishPunch();
        }
        #endregion

        #region eventOverrides
        protected override void OnStateEnter()
        {
            this.inputRelay.OnPunchInput += OnPunchInputHandler;
            this.inputRelay.OnPlayerHit += OnPlayerHitHandler;

            this.inputRelay.OnRecoveryComplete += OnRecoveryCompleteHandler;
            this.inputRelay.OnWindUpComplete += OnWindUpCompleteHandler;

            this.inputRelay.OnPlayerDeath += OnPlayerDeathHandler;

            StartPunch();
        }
        protected override void OnStateExit()
        {
            this.inputRelay.OnPunchInput -= OnPunchInputHandler;
            this.inputRelay.OnPlayerHit -= OnPlayerHitHandler;

            this.inputRelay.OnRecoveryComplete -= OnRecoveryCompleteHandler;
            this.inputRelay.OnWindUpComplete -= OnWindUpCompleteHandler;

            this.inputRelay.OnPlayerDeath -= OnPlayerDeathHandler;

            if (!comboChained) comboCount = 0;
            comboChained = false;
        }
        protected override void OnStateInitialize(StateMachine machine = null)
        {
            comboCount = 0;
        }
        #endregion
    }
}
