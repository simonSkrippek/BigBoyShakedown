using System.Collections.Generic;
using UnityEngine;

namespace BigBoyShakedown.Player.State
{
    [RequireComponent(typeof(Appearance.PlayerAppearance))]
    public class StateMachine : MonoBehaviour
    {
        public List<State> statesList;
        protected State oldState;
        public State GetOldState { get { return oldState; } }
        protected State currentState;
        public State GetCurrentState { get { return currentState; } }

        public Appearance.PlayerAppearance playerAppearance;

        private void Awake()
        {
            statesList = new List<State>();
            currentState = null;

            playerAppearance = GetComponent<Appearance.PlayerAppearance>();
        }
        public void Start()
        {
            SetState<IdlingState>();
        }


        public void setState(State state)
        {
            SetState(state);
        }

        /// <summary>
        /// Switch the currentState to a specific State object
        /// </summary>
        /// <param name="state">
        /// The state object to set as the currentState</param>
        /// <returns>Whether the state was changed</returns>
        public virtual bool SetState(State state)
        {
            bool success = false;
            if (state)
            {
                oldState = currentState;
                currentState = state;
                if (oldState)
                    oldState.StateExit();
                currentState.StateEnter();
                success = true;
            }
            return success;
        }

        /// <summary>
        /// Switch the currentState to a State of a the given type.
        /// </summary>
        /// <typeparam name="StateType">
        /// The type of state to use for the currentState</typeparam>
        /// <returns>Whether the state was changed</returns>
        public virtual bool SetState<StateType>() where StateType : State
        {
            bool success = false;
            //if the state can be found in the list of states 
            //already created, switch to the existing version
            foreach (State state in statesList)
            {
                if (state is StateType)
                {
                    success = SetState(state);
                    return success;
                }
            }
            //if the state is not found in the list,
            //see if it is on the gameobject.
            State stateComponent = GetComponent<StateType>();
            if (stateComponent)
            {
                stateComponent.Initialize(this);
                statesList.Add(stateComponent);
                success = SetState(stateComponent);
                return success;
            }
            //if it is not on the gameobject,
            //make a new instance
            State newState = gameObject.AddComponent<StateType>();
            newState.Initialize(this);
            statesList.Add(newState);
            success = SetState(newState);

            return success;
        }
    }
}