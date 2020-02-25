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
        [SerializeField] float storingAmount = 100f;
        [SerializeField] float storingMultiplier = 3f;
        PlayerController interactingPlayer;
        int interactingPlayerIndex;
        int interactionStagesCompleted;

        [SerializeField] int minPlayerSize = 2;

        [Header("Colors"), SerializeField, ColorUsage(true)]
        Color inUseColor;
        [SerializeField] MeshRenderer atmRenderer;

        List<VariableReference<bool>> currentRunningTimers;
        //ON Destroy animation reference

        private void Awake()
        {
            moneyList = new float[4];
            currentRunningTimers = new List<VariableReference<bool>>();
        }

        /// <summary>
        /// start an interaction by a player with this object
        /// </summary>
        /// <param name="player">the interacting player</param>
        public void StartInteraction(PlayerController player)
        {
            if (!player) return;

            interactingPlayer = player;
            interactingPlayerIndex = interactingPlayer.inputRelay.input.playerIndex;

            if (moneyList[interactingPlayerIndex] > 0 && interactionStagesCompleted == 0)
            {
                Manager.AudioManager.instance.Play("ATM_interact");
                var timer = new VariableReference<bool>(() => false, (val) => { RetrieveStoredMoney(interactingPlayerIndex); });
                Time.StartTimer(timer, timeUntilStageCompletion * retrievalTimeMultiplier);
                currentRunningTimers.Add(timer);
            }
            else if(player.CheckRemainingMoney(interactingPlayer.metrics.PlayerScore[minPlayerSize - 1]))
            {
                if (moneyList[interactingPlayerIndex] < 0) moneyList[interactingPlayerIndex] = 0;

                atmRenderer.material.color = inUseColor;

                Manager.AudioManager.instance.Play("ATM_interact");

                var timer = new VariableReference<bool>(() => false, (val) => { CompleteInteraction(); });
                Time.StartTimer(timer, timeUntilStageCompletion);
                currentRunningTimers.Add(timer);
            }
            else
            {
                CancelInteraction();
            }
        }
        /// <summary>
        /// give their money back to the interacting player
        /// </summary>
        /// <param name="interactingPlayerIndex_">the player to hand their money back to</param>
        private void RetrieveStoredMoney(int interactingPlayerIndex_)
        {
            if (interactingPlayer)
            {
                if (interactingPlayerIndex_ < 0 || interactingPlayerIndex_ >= moneyList.Length)
                {
                    Debug.LogError("atm mistake, index not in array");
                }
                interactingPlayer.CompleteInteraction(this, moneyList[interactingPlayerIndex_]);
                moneyList[interactingPlayerIndex_] = 0;
                CancelInteraction();
            }
        }
        /// <summary>
        /// stop an interaction by the interacting player with this object
        /// </summary>
        public void CancelInteraction()
        {
            atmRenderer.material.color = new Color(1,1,1,0);
            Manager.AudioManager.instance.StopPlaying("ATM_interact");

            if (interactingPlayer) interactingPlayer.CancelInteraction();

            interactingPlayer = null;
            interactionStagesCompleted = 0;
            interactingPlayerIndex = -1;

            foreach (var timer in currentRunningTimers)
            {
                Time.StopTimer(timer);
            }
            currentRunningTimers = new List<VariableReference<bool>>();
        }
        /// <summary>
        /// complete the interaction and notify the interacting playerController with the reward
        /// </summary>
        public void CompleteInteraction()
        {
            if (interactingPlayer)
            {
                atmRenderer.material.color = new Color(1, 1, 1, 0);
                Manager.AudioManager.instance.StopPlaying("ATM_interact");

                float moneyToBank = storingAmount * Mathf.Pow(storingMultiplier, interactionStagesCompleted);
                if (moneyToBank > (interactingPlayer.score - interactingPlayer.metrics.PlayerScore[minPlayerSize - 1]))
                {
                    moneyToBank = interactingPlayer.score - interactingPlayer.metrics.PlayerScore[minPlayerSize - 1];
                }
                interactingPlayer.BankMoney(moneyToBank);
                moneyList[interactingPlayerIndex] += moneyToBank;

                interactionStagesCompleted++;

                StartInteraction(interactingPlayer);
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