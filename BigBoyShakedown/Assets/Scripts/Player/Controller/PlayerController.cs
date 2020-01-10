using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BigBoyShakedown.Player.Input;
using BigBoyShakedown.Player.Metrics;
using System;

namespace BigBoyShakedown.Player.Controller
{
    [RequireComponent(typeof(PlayerInputRelay))]
    public class PlayerController : MonoBehaviour
    {
        PlayerInputRelay inputRelay;

        public PlayerMetrics metrics;
        public int size;
        public int score;

        private void Awake()
        {
            inputRelay = GetComponent<PlayerInputRelay>();
        }

        /// <summary>
        /// check if an interactable is in range of the charater.
        /// </summary>
        /// <returns>if an interactable in range was found</returns>
        public bool InteractableInRange()
        {
            throw new NotImplementedException();
        }

        ///<summary> 
        ///check for collision in the movement direction. If there is none, move the character.
        ///</summary>
        ///<param name="movement">The movement vector to apply, not adapted to camera perspective</param>
        public void TryApplyMovement(Vector3 movement)
        {
            
        }

        /// <summary>
        /// apply a hit from another character to this one. modify score and notify inputRelay.
        /// </summary>
        /// <param name="From">the characterController of the character dealing the damage</param>
        public void TakeDamage(PlayerController From)
        {

        }

        /// <summary>
        /// apply a hit from this character to another one. modify score and notify other character.
        /// </summary>
        /// <param name="To">the character being hit</param>
        public void DealDamage(PlayerController To)
        {

        }

        /// <summary>
        /// checks wether the character is standing on the floor
        /// </summary>
        /// <returns>false if character is in the air</returns>
        public bool IsGrounded()
        {
            return this.transform.position.y <= 0;
        }
    }
}
