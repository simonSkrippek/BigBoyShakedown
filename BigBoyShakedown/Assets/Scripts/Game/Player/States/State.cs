using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BigBoyShakedown.Player.Input;
using BigBoyShakedown.Player.Controller;

namespace BigBoyShakedown.Player.State
{
    /// <summary>
    /// the template for all states
    /// </summary>
    [RequireComponent(typeof(StateMachine), typeof(PlayerInputRelay), typeof(PlayerController)), RequireComponent(typeof(StateCarryOver))]
    public class State : MonoBehaviour
    {
        bool initialized = false;

        public StateMachine machine;
        public PlayerInputRelay inputRelay;
        public PlayerController controller;
        public StateCarryOver carryOver;

        #region stateInitialize
        private void Awake()
        {
            if (!initialized) this.enabled = false;
        }
        public void Initialize(StateMachine machine)
        {
            this.machine = machine;
            initialized = true;
            this.machine = GetComponent<StateMachine>();
            this.inputRelay = GetComponent<PlayerInputRelay>();
            this.controller = GetComponent<PlayerController>();
            this.carryOver = GetComponent<StateCarryOver>();
            OnStateInitialize(machine);
        }
        protected virtual void OnStateInitialize(StateMachine machine = null)
        {
        }
        #endregion

        #region stateEnter
        public void StateEnter()
        {
            //Debug.Log("State entered: " + this.GetType());

            enabled = true;
            OnStateEnter();
            carryOver.Reset();
        }
        protected virtual void OnStateEnter()
        {
        }
        #endregion

        #region stateExit
        public void StateExit()
        {
            OnStateExit();

            //Debug.Log("State exited: " + this.GetType());

            enabled = false;
        }
        protected virtual void OnStateExit()
        {
        }
        #endregion

        #region unityEvents
        public void OnEnable()
        {
            if (initialized)
            {
                if (this != machine.GetCurrentState)
                {
                    enabled = false;
                    Debug.LogWarning("Do not enable States directly. Use StateMachine.SetState");
                }
            }
        }
        public void OnDisable()
        {
            if (initialized)
            {
                if (this == machine.GetCurrentState && this != machine.GetOldState)
                {
                    enabled = true;
                    Debug.LogWarning("Do not disable States directly. Use StateMachine.SetState");
                }
            }
        }
        #endregion


        public static implicit operator bool(State state)
        {
            return state != null;
        }
    }
}