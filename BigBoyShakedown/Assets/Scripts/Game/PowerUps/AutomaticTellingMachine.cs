using BigBoyShakedown.Player.Controller;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BigBoyShakedown.Game.PowerUp
{
    public class AutomaticTellingMachine : MonoBehaviour, Interactable
    {
        float[] moneyList;

        [SerializeField] float timeUntilStageCompletion;
        bool beingInteractedWith, interactionStageCompleted;
        [SerializeField] float storingAmount = 100f;
        [SerializeField] float storingMultiplier = 3f;
        PlayerController interactingPlayer;
        int interactingPlayerIndex;
        int interactionStagesCompleted;

        int minPlayerSize = 2;
        //ON Destroy animation reference

        private void Awake()
        {
            moneyList = new float[4];
        }

        private void Update()
        {
            if (beingInteractedWith)
            {
                if (interactionStageCompleted)
                {
                    CompleteInteraction();
                    interactionStagesCompleted++;
                }
            }
        }

        /// <summary>
        /// start an interaction by a player with this object
        /// </summary>
        /// <param name="player">the interacting player</param>
        public void StartInteraction(PlayerController player)
        {
            interactingPlayerIndex = player.inputRelay.input.playerIndex;
            if (moneyList[interactingPlayerIndex] > 0)
            {
                RetrieveStoredMoney(interactingPlayerIndex);
            }
            else if(interactingPlayer.CheckRemainingMoney(interactingPlayer.metrics.PlayerScore[minPlayerSize - 1] + storingAmount * Mathf.Pow(storingMultiplier, interactionStagesCompleted)))
            {
                moneyList[interactingPlayerIndex] = 0;
                interactionStagesCompleted = 0;
                beingInteractedWith = true;
                interactingPlayer = player;
                interactionStageCompleted = false;
                Time.StartTimer(new VariableReference<bool>(() => interactionStageCompleted, (val) => { interactionStageCompleted = val; }).SetEndValue(true), timeUntilStageCompletion);
            }
        }
        /// <summary>
        /// give their money back to the interacting player
        /// </summary>
        /// <param name="interactingPlayerIndex_">the player to hand their money back to</param>
        private void RetrieveStoredMoney(int interactingPlayerIndex_)
        {
            interactingPlayer.CompleteInteraction(this, moneyList[interactingPlayerIndex_]);
            moneyList[interactingPlayerIndex_] = 0;
        }
        /// <summary>
        /// stop an interaction by the interacting player with this object
        /// </summary>
        public void CancelInteraction()
        {
            if (interactingPlayer) interactingPlayer.CancelInteraction();
            beingInteractedWith = false;
            interactionStageCompleted = false;
            interactingPlayer = null;
            interactingPlayerIndex = -1;
        }
        /// <summary>
        /// complete the interaction and notify the interacting playerController with the reward
        /// </summary>
        public void CompleteInteraction()
        {
            interactingPlayer.BankMoney(storingAmount * Mathf.Pow(storingAmount, interactionStagesCompleted));
            if (interactingPlayer.CheckRemainingMoney(interactingPlayer.metrics.PlayerScore[minPlayerSize-1] + storingAmount * Mathf.Pow(storingAmount, interactionStagesCompleted + 1)))
            {
                interactionStageCompleted = false;
                Time.StartTimer(new VariableReference<bool>(() => interactionStageCompleted, (val) => { interactionStageCompleted = val; }).SetEndValue(true), timeUntilStageCompletion);
            }
        }
        public void DestroyInteractable()
        {
            //trigger on destroy animation and destroy this gameObject
            Destroy(this.gameObject);
        }

        public MonoBehaviour GetObject()
        {
            return this;
        }
    }
}