using BigBoyShakedown.Player.Controller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BigBoyShakedown.Game.Interactable
{
    public class Interactable : MonoBehaviour
    {
        [SerializeField] float timeUntilCompletion;
        bool beingInteractedWith, interactionCompleted;
        [SerializeField] float monetaryReward;
        public float MonetaryReward { get => monetaryReward; }
        PlayerController interactingPlayer;

        //ON Destroy animation reference

        private void Update()
        {
            if (beingInteractedWith)
            {
                if (interactionCompleted)
                {
                    CompleteInteraction();
                }
            }
        }
        /// <summary>
        /// start an interaction by a player with this object
        /// </summary>
        /// <param name="player">the interacting player</param>
        public void StartInteraction(PlayerController player)
        {
            beingInteractedWith = true;
            interactingPlayer = player;
            interactionCompleted = false;
            Time.StartTimer(new VariableReference<bool>(() => interactionCompleted, (val) => { interactionCompleted = val; }).SetEndValue(true), timeUntilCompletion);
        }
        /// <summary>
        /// stop an interaction by the interacting player with this object
        /// </summary>
        public void CancelInteraction()
        {
            beingInteractedWith = false;
            interactionCompleted = false;
            interactingPlayer = null;
        }
        /// <summary>
        /// complete the interaction and notify the interacting playerController with the reward
        /// </summary>
        public void CompleteInteraction()
        {
            interactingPlayer.CompleteInteraction(this, MonetaryReward);
            DestroyInteractable();
        }
        public void DestroyInteractable()
        {
            //trigger on destroy animation and destroy this gameObject

            Destroy(this.gameObject);
        }
    }
}
