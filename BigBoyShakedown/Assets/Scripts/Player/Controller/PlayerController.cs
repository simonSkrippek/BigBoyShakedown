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
            Vector3 possibleMovement = CheckCollisionInPath(movement);
            this.transform.Translate(possibleMovement);
        }
        /// <summary>
        /// cast a capsule in the movement direction, originating from this players transform. values for capsule size are taken form playerMetrics object.
        /// </summary>
        /// <param name="movement">the movement direction / length</param>
        /// <returns>distance / direction until the collision, ie distance that can safely be moved</returns>
        private Vector3 CheckCollisionInPath(Vector3 movement)
        {
            var point1 = this.transform.position;
            point1.y = metrics.PlayerScale[size].x * 1 / 3;
            var point2 = this.transform.position;
            point1.y = metrics.PlayerScale[size].x * 2 / 3;
            var radius = metrics.PlayerScale[size].y / 2;
            var mask = LayerMask.GetMask(metrics.Mask_collidables);
            RaycastHit hit;
            if (Physics.CapsuleCast(point1, point2, radius, movement.normalized, out hit, movement.magnitude, mask, QueryTriggerInteraction.Collide) && hit.transform.root != this.transform)
                return hit.distance * movement.normalized;
            else
                return Vector3.zero;
        }

        /// <summary>
        /// relay a hit from another character to this one. if current state allows the hit, ApplyHit() will be called,
        /// </summary>
        /// <param name="from">the characterController of the character attacking</param> 
        /// <param name="damageIntended">the [raw] damage the other character is trying to deal</param>
        /// <param name="knockbackDistanceIntended">the [raw] distance the other character is trying to knock this back</param>
        /// <param name="stunDurationIntended">the [raw] duration the other character is trying to stun this for</param>
        /// [raw] => _not_ modified by current state
        public void TryApplyHit(PlayerController from, float damageIntended, float knockbackDistanceIntended, float stunDurationIntended)
        {
            this.inputRelay.RelayPlayerHit(from, damageIntended, knockbackDistanceIntended, stunDurationIntended);
        }

        /// <summary>
        /// apply a hit from another character to this one. modify this chracter's score and notify other characterController.
        /// </summary>
        /// <param name="from">the characterController of the character attacking</param> 
        /// <param name="damage">the [scaled] damage the other character is trying to deal</param>
        /// <param name="knockbackDistance">the [scaled] distance the other character is trying to knock this back</param>
        /// <param name="stunDuration">the [scaled] duration the other character is trying to stun this for</param>
        /// [scaled] => modified by current state
        public void ApplyHit(PlayerController from, float damage, float knockbackDistance, float stunDuration)
        {
            //apply damage
            score -= (int) damage;
            //TODO: rest
            throw new NotImplementedException();
        }

        /// <summary>
        /// apply the effects of a successful hit on another player.
        /// </summary>
        /// <param name="to">the character that was hit</param>
        /// <param name="damageDealt">the damage that was accepted by the other player.</param>
        public void HitCallback(PlayerController to, float damageDealt)
        {
            score += (int)damageDealt;
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
