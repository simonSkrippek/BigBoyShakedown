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
        /// whether or not the punch is in recovery phase
        /// </summary>
        bool inRecovery;
        /// <summary>
        /// if another punch input is registered during recovery, set to true
        /// </summary>
        bool comboChained;

        Vector3 moveDirection;

        private void FixedUpdate()
        {
            //controller.TryApplyMovement(moveDirection * controller.metrics.PlayerMoveSpeed[controller.size - 1] * .01f);
        }


        /// <summary>
        /// starts punch, called when state is activated
        /// </summary>
        private void StartPunch()
        {
            moveDirection = this.transform.forward.normalized;

            comboCount++;
            if (comboCount > 3) comboCount = 1;

            var objectsInRange = controller.GetAllAttackablesInRange();
            var objectToSnapTo = controller.GetClosestAttackable(objectsInRange);
            if (objectToSnapTo) controller.TurnTo(objectToSnapTo.position);
            var objectsInCone = controller.GetAllAttackablesInAttackCone(objectsInRange);
            controller.TargetAttackables(objectsInCone);

            this.InvokeRepeating("TargetAllAttackables", .1f, .1f);

            machine.animator.Play("Player_Small_Punch" + comboCount + "_WindUp");
            //Debug.Log("Punch Started! \n Combo Count: " + comboCount);
        }       
        /// <summary>
        /// call when windup is over, before recovery starts
        /// </summary>
        private void LandPunch()
        {
            CancelInvoke("TargetAllAttackables");
            var enemiesToAttack = controller.GetAllAttackablesInAttackCone(controller.GetAllAttackablesInRange());
            controller.HitAllAttackables(enemiesToAttack,
                                        controller.metrics.PlayerDamage[controller.size - 1, comboCount-1],
                                        controller.metrics.PlayerPunchKnockback[controller.size - 1, comboCount-1],
                                        controller.metrics.PlayerPunchStunDuration[controller.size - 1, comboCount-1]);
            machine.animator.Play("Player_Small_Punch" + comboCount + "_Recovery");


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

            StartPunch();
        }
        protected override void OnStateExit()
        {
            this.inputRelay.OnPunchInput -= OnPunchInputHandler;
            this.inputRelay.OnPlayerHit -= OnPlayerHitHandler;

            this.inputRelay.OnRecoveryComplete -= OnRecoveryCompleteHandler;
            this.inputRelay.OnWindUpComplete -= OnWindUpCompleteHandler;


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
