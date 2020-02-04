using BigBoyShakedown.Player.Controller;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BigBoyShakedown.Game.PowerUp
{
    public class CommonInteractable : MonoBehaviour, Interactable
    {
        [SerializeField] float timeUntilCompletion;
        [SerializeField] float monetaryReward;
        public float MonetaryReward { get => monetaryReward; }
        PlayerController interactingPlayer;

        PowerUpCallback callback;

        VariableReference<bool> activeTimer;

        //ON Destroy animation reference

        private void Awake()
        {
            if (!TryGetComponent<PowerUpCallback>(out callback))
            {
                throw new Exception("no PowerUpCallback component found");
            }
        }

        private void Update()
        {
        }
        /// <summary>
        /// start an interaction by a player with this object
        /// </summary>
        /// <param name="player">the interacting player</param>
        public void StartInteraction(PlayerController player)
        {
            Manager.AudioManager.instance.Play("ATM_interact");
            interactingPlayer = player;
            activeTimer = new VariableReference<bool>(() => false, (val) => { CompleteInteraction(); });
            Time.StartTimer(activeTimer, timeUntilCompletion);
        }
        /// <summary>
        /// stop an interaction by the interacting player with this object
        /// </summary>
        public void CancelInteraction()
        {
            Manager.AudioManager.instance.StopPlaying("ATM_interact");
            Time.StopTimer(activeTimer);
            activeTimer = null;
            interactingPlayer = null;
        }
        /// <summary>
        /// complete the interaction and notify the interacting playerController with the reward
        /// </summary>
        public void CompleteInteraction()
        {
            Manager.AudioManager.instance.StopPlaying("ATM_interact");
            if (interactingPlayer)
            {
                activeTimer = null;
                interactingPlayer.CompleteInteraction(this, MonetaryReward);
                DestroyInteractable();
            }
        }
        public void DestroyInteractable()
        {
            //trigger on destroy animation and destroy this gameObject
            callback.RelayPowerUpEnded();
            Destroy(this.gameObject);
        }
               
               
        public MonoBehaviour GetObject()
        {
            return this;
        }
    }
}
