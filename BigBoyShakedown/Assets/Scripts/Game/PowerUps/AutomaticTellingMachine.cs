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

        [SerializeField] float timeUntilStageCompletion = 2f;
        [SerializeField] float retrievalTimeMultiplier = 2f;
        bool beingInteractedWith, interactionStageCompleted;
        [SerializeField] float storingAmount = 100f;
        [SerializeField] float storingMultiplier = 3f;
        PlayerController interactingPlayer;
        int interactingPlayerIndex;
        int interactionStagesCompleted;

        [SerializeField] int minPlayerSize = 2;
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

            interactingPlayer = player;
            interactingPlayerIndex = interactingPlayer.inputRelay.input.playerIndex;

            var threshhold = interactingPlayer.metrics.PlayerScore[minPlayerSize - 1] + storingAmount * Mathf.Pow(storingMultiplier, interactionStagesCompleted);
            Debug.Log("threshold" + 5 + " fetched: " + threshhold);

            if (moneyList[interactingPlayerIndex] > 0)
            {
                Manager.AudioManager.instance.Play("ATM_interact");
                Time.StartTimer(new VariableReference<bool>(()=> false, (val) => { RetrieveStoredMoney(interactingPlayerIndex); }), timeUntilStageCompletion * retrievalTimeMultiplier);
            }
            else if(player.CheckRemainingMoney(threshhold))
            {
                Manager.AudioManager.instance.Play("ATM_interact");
                moneyList[interactingPlayerIndex] = 0;
                interactionStagesCompleted = 0;
                beingInteractedWith = true;
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
            if (interactingPlayerIndex_ < 0 || interactingPlayerIndex_ >= moneyList.Length)
            {
                Debug.LogError("atm mistake, index not in array");
            }
            interactingPlayer.CompleteInteraction(this, moneyList[interactingPlayerIndex_]);
            moneyList[interactingPlayerIndex_] = 0;
            CancelInteraction();
        }
        /// <summary>
        /// stop an interaction by the interacting player with this object
        /// </summary>
        public void CancelInteraction()
        {
            Manager.AudioManager.instance.StopPlaying("ATM_interact");
            if (interactingPlayer) interactingPlayer.CancelInteraction();
            beingInteractedWith = false;
            interactionStageCompleted = false;
            interactionStagesCompleted = 0;
            interactingPlayer = null;
            interactingPlayerIndex = -1;
        }
        /// <summary>
        /// complete the interaction and notify the interacting playerController with the reward
        /// </summary>
        public void CompleteInteraction()
        {
            Manager.AudioManager.instance.StopPlaying("ATM_interact");
            float moneyToBank = storingAmount * Mathf.Pow(storingMultiplier, interactionStagesCompleted);
            moneyList[interactingPlayerIndex] += moneyToBank;
            interactingPlayer.BankMoney(moneyToBank);
            var threshhold = interactingPlayer.metrics.PlayerScore[minPlayerSize - 1] + storingAmount * Mathf.Pow(storingMultiplier, interactionStagesCompleted);
            Debug.Log("threshold" + 5 + " fetched: " + threshhold);
            if (interactingPlayer.CheckRemainingMoney(threshhold))
            {
                interactionStageCompleted = false;
                Time.StartTimer(new VariableReference<bool>(() => interactionStageCompleted, (val) => { interactionStageCompleted = val; }).SetEndValue(true), timeUntilStageCompletion);
            }
            else
            {
                CancelInteraction();
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