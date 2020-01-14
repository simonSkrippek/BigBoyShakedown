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
                        //apply punch

                        inRecovery = true;
                        Time.StartTimer(new VariableReference<bool>(() => inRecovery, (val) => { inRecovery = val; }).SetEndValue(false), windUpTime);
                        punchCompleted = true;
                    }
                    else
                    {
                        if (comboChained )
                            //also check if in combo time window
                        machine.SetState<IdlingState>();
                    }
                }
                else
                {
                    //get all players in cone, target them
                }
            }
        }

        private void OnPunchInputHandler()
        {
            
        }

        private void OnPlayerHitHandler(PlayerController from, float damageIntended, float knockbackDistanceIntended, float stunDurationIntended)
        {
            if (from.size > controller.size)
            {
                controller.ApplyHit(from, damageIntended, knockbackDistanceIntended, stunDurationIntended);
            }
            else
            {
                controller.ApplyHit(from, damageIntended, 0f, 0f);
            }
        }

        #region eventOverrides
        protected override void OnStateEnter()
        {
            this.inputRelay.OnPunchInput += OnPunchInputHandler;
            this.inputRelay.OnPlayerHit += OnPlayerHitHandler;

            windUpTime = controller.metrics.PlayerWindUpTime[controller.size];
            inWindUp = true;
            Time.StartTimer(new VariableReference<bool>(() => inWindUp, (val) => { inWindUp = val; }).SetEndValue(false), windUpTime);
            recoveryTime = controller.metrics.PlayerPunchRecoveryTime[controller.size];
            comboCount++;
            if (comboCount > 3) comboCount = 1;
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
