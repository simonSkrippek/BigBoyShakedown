using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BigBoyShakedown.Player.Input;
using System;
using BigBoyShakedown.Player.Controller;

namespace BigBoyShakedown.Player.State
{
    public class PunchingState : State
    {
        int comboCount;

        bool punchCompleted;

        public float windUpTime;
        bool inWindUp;
        public float recoveryTime;
        bool inRecovery;

        bool comboChained;


        void Update()
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

        private void CompletePunch()
        {
            var enemiesToAttack = controller.GetAllAttackablesInAttackCone(controller.GetAllAttackablesInRange());
            foreach(var enemy in enemiesToAttack)
            {
                
            }
        }

        public void TargetAllAttackables()
        {
            var objectsInRange = controller.GetAllAttackablesInRange();
            var objectsInCone = controller.GetAllAttackablesInAttackCone(objectsInRange);
            controller.TargetAttackables(objectsInCone);
        }

        private void OnPunchInputHandler()
        {
            if (inRecovery)
            {
                comboChained = true;
            }
        }

        private void OnPlayerHitHandler(PlayerController from, float damageIntended, float knockbackDistanceIntended, float stunDurationIntended)
        {
            if (from.size > controller.size)
            {
                controller.ReceiveHit(from, damageIntended, knockbackDistanceIntended, stunDurationIntended);
            }
            else
            {
                controller.ReceiveHit(from, damageIntended, 0f, 0f);
            }
        }

        private void StartPunch()
        {
            inWindUp = true;
            Time.StartTimer(new VariableReference<bool>(() => inWindUp, (val) => { inWindUp = val; }).SetEndValue(false), windUpTime);
            comboCount++;
            if (comboCount > 3) comboCount = 1;

            var objectsInRange = controller.GetAllAttackablesInRange();
            var objectToSnapTo = controller.GetClosestAttackable(objectsInRange);
            controller.TurnTo(objectToSnapTo.position);
            var objectsInCone = controller.GetAllAttackablesInAttackCone(objectsInRange);
            controller.TargetAttackables(objectsInCone);

            this.InvokeRepeating("TargetAllAttackables", .101f, .101f);
        }

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
        }
        protected override void OnStateInitialize(StateMachine machine = null)
        {
            comboCount = 0;
        }
        #endregion
    }
}
