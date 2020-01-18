using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BigBoyShakedown.Player.Controller;

namespace BigBoyShakedown.Player.State
{
    public class StunnedState : State
    {
        float stunTimer;
        Vector3 knockBack;
        bool stunned = true;

        // Update is called once per frame
        void Update()
        {
            
        }

        private void StartStun()
        {
            stunTimer = carryOver.stunDuration;
            knockBack = carryOver.knockbackDistance;
            stunned = true;
            Time.StartTimer(new VariableReference<bool>(() => stunned, (val) => stunned = val), stunTimer);
            carryOver.Reset();
        }

        private void OnPlayerDeathHandler()
        {
            throw new System.NotImplementedException();
        }

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
    }
}
