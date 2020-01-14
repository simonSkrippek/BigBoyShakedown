using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BigBoyShakedown.Player.Input;
using BigBoyShakedown.Player.Controller;

namespace BigBoyShakedown.Player.State
{
    [RequireComponent(typeof(StateMachine), typeof(PlayerInputRelay), typeof(PlayerController))]
    public class State : MonoBehaviour
    {
        public StateMachine machine;
        public PlayerInputRelay inputRelay;
        public PlayerController controller;

        #region stateInitialize
        public void Initialize(StateMachine machine)
        {
            this.machine = machine;
            OnStateInitialize(machine);
        }
        protected virtual void OnStateInitialize(StateMachine machine = null)
        {
        }
        #endregion

        #region stateEnter
        public void StateEnter()
        {
            Debug.Log("State entered: " + this.GetType());

            enabled = true;
            OnStateEnter();
        }
        protected virtual void OnStateEnter()
        {
        }
        #endregion

        #region stateExit
        public void StateExit()
        {
            Debug.Log("State exited: " + this.GetType());

            OnStateExit();
            enabled = false;
        }
        protected virtual void OnStateExit()
        {
        }
        #endregion

        #region enable/disable
        public void OnEnable()
        {
            if (this != machine.GetCurrentState)
            {
                enabled = false;
                Debug.LogWarning("Do not enable States directly. Use StateMachine.SetState");
            }
        }

        public void OnDisable()
        {
            if (this == machine.GetCurrentState)
            {
                enabled = true;
                Debug.LogWarning("Do not disable States directly. Use StateMachine.SetState");
            }
        }
        #endregion

        void OnValidate()
        {
            this.machine = GetComponent<StateMachine>();
            this.inputRelay = GetComponent<PlayerInputRelay>();
            this.controller = GetComponent<PlayerController>();
        }

        public static implicit operator bool(State state)
        {
            return state != null;
        }
    }
}