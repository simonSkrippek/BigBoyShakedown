using BigBoyShakedown.Game.PowerUp;
using BigBoyShakedown.Player.Input;
using BigBoyShakedown.Player.Metrics;
using BigBoyShakedown.Player.State;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BigBoyShakedown.Player.Controller
{
    [RequireComponent(typeof(PlayerInputRelay), typeof(StateMachine), typeof(StateCarryOver))]
    public class PlayerController : MonoBehaviour
    {
        PlayerInputRelay inputRelay;

        public PlayerMetrics metrics;
        public int size = 0;
        public float score;

        private void Awake()
        {
            inputRelay = GetComponent<PlayerInputRelay>();
        }
        private void Start()
        {
            this.score = metrics.PlayerStartScore;
            size = GetSizeFromScore();
            if (size < 0) throw new Exception("mistake calculating size");
            inputRelay.RelayPlayerSizeChange(size);
        }
        private void Update()
        {
            int currentSize = GetSizeFromScore();
            if (currentSize != size && currentSize > 0 && currentSize <= 5)
            {
                size = currentSize;
                inputRelay.RelayPlayerSizeChange(size);
            }
            else if (currentSize < 1)
            {
                throw new Exception("invalid size calculated: " + currentSize);
            }
            else if (currentSize > 5)
            {
                Debug.Log("player has won, end game here");
            }
        }
        /// <summary>
        /// calculate the players size based on its score (base on values in #playerMetrics)
        /// </summary>
        /// <returns>the players current size based on score</returns>
        private int GetSizeFromScore()
        {
            for (int i = 0; i < metrics.PlayerScore.Length; i++)
            {
                if (i + 1 >= metrics.PlayerScore.Length)
                {
                    Debug.LogWarning("Player has won: " + this.gameObject.name + "\n size: " + (i+1));
                    return i + 1;
                }

                if (score >= metrics.PlayerScore[i] && score < metrics.PlayerScore[i + 1]) return i+1;
            }
            return -1;
        }

        #region interactionMethods
        /// <summary>
        /// check if an interactable is in range of the charater.
        /// </summary>
        /// <returns>if an interactable in range was found</returns>
        public bool InteractableInRange()
        {
            var result = GetInteractablesInRange();
            if (result.Length > 0) return true;
            else return false;
        }
        /// <summary>
        /// check if a specific interactable is in range
        /// </summary>
        /// <param name="interactable">the interactable to check</param>
        /// <returns>true if in range</returns>
        public bool InteractableInRange(Interactable interactable)
        {
            return ((interactable.transform.root.position - this.transform.position).magnitude < metrics.PlayerInteractionRange[size - 1]);
        }
        /// <summary>
        /// get the closest interactable to this player
        /// </summary>
        /// <returns>closest interactable found</returns>
        public Interactable GetClosestInteractable()
        {
            var interactables = GetInteractablesInRange();
            Interactable closestInteractable = null;
            float distanceToClosestInteractable = float.MaxValue;
            foreach (var interactable in interactables)
            {
                Interactable interactableComponent;
                if (interactable.transform.root.TryGetComponent<Interactable>(out interactableComponent))
                {
                    var distanceToInteractable = (interactable.transform.root.position - this.transform.position).magnitude;
                    if (distanceToInteractable < distanceToClosestInteractable)
                    {
                        distanceToClosestInteractable = distanceToInteractable;
                        closestInteractable = interactableComponent;
                    }
                }
            }
            return closestInteractable;
        }
        /// <summary>
        /// return all interactables in range, ie all objects on the layer specified as interactable layers
        /// </summary>
        /// <returns>an array of colliders in range</returns>
        public Collider[] GetInteractablesInRange()
        {
            var result = Physics.OverlapSphere(this.transform.position + new Vector3(0, metrics.PlayerScale[size - 1].x / 2f, 0), metrics.PlayerInteractionRange[size - 1], LayerMask.GetMask(metrics.Mask_interactables), QueryTriggerInteraction.Collide);
            return result;
        }
        /// <summary>
        /// complete an interaction with the specified interactable.
        /// notify playerInput on this gameobject, as well as the interactable
        /// </summary>
        /// <param name="interactable">the interactable to finish interacting with</param>
        /// <returns>whether completing the interaction was successful</returns>
        public void CompleteInteraction(Interactable interactable, float reward)
        {
            score += reward;
            inputRelay.RelayPlayerScoreChange(reward);
        }
        #endregion

        #region movementMethods
        ///<summary> 
        ///check for collision in the movement direction. If there is none, move the character.
        ///</summary>
        ///<param name="movement">The movement vector to apply, not adapted to camera perspective</param>
        public void TryApplyMovement(Vector3 movement)
        {
            Vector3 possibleMovement = CheckCollisionInPath(movement, true);
            this.transform.position += possibleMovement;
        }
        /// <summary>
        /// cast a capsule in the movement direction, originating from this players transform. values for capsule size are taken form playerMetrics object.
        /// </summary>
        /// <param name="movement">the movement direction / length</param>
        /// <returns>distance / direction until the collision, ie distance that can safely be moved</returns>
        private Vector3 CheckCollisionInPath(Vector3 movement, bool sliding)
        {
            var mask = LayerMask.GetMask(metrics.Mask_collidables);
            var point1 = this.transform.position + new Vector3(0, metrics.PlayerScale[size - 1].x * 1 / 3, 0);
            var point2 = this.transform.position + new Vector3(0, metrics.PlayerScale[size - 1].x * 2 / 3, 0);
            var radius = metrics.PlayerScale[size - 1].y / 2;

            RaycastHit hit;
            if (Physics.CapsuleCast(point1: point1, point2: point2, radius: radius, direction: movement.normalized, hitInfo: out hit, maxDistance: movement.magnitude, layerMask: mask, QueryTriggerInteraction.Collide) && hit.transform.root != this.transform)
            {
                if (sliding)
                {
                    var movementOnColliderPlane = Vector3.ProjectOnPlane(movement, hit.normal);
                    movementOnColliderPlane.y = 0f;
                    return CheckCollisionInPath(movementOnColliderPlane, false);
                }
                else
                {
                    var distanceToObject = (hit.distance - .01f);
                    if (distanceToObject >= 0)
                    {
                        var distanceToGo = distanceToObject * movement.normalized;
                        return distanceToGo;
                    }
                    else return Vector3.zero;
                }
            }
            else return movement;
        }

        /// <summary>
        /// rotate the transform attached to this gameobject towards the real world position passed.
        /// </summary>
        /// <param name="position">the position to turn to</param>
        public void TurnTo(Vector3 position)
        {
            var direction = position - this.transform.position;
            TurnIn(direction);
        }

        /// <summary>
        /// rotate the transform attached to this gameobject by setting its forward vector to the one passed
        /// </summary>
        /// <param name="direction">the direction to set the forward vector to</param>
        public void TurnIn(Vector3 direction)
        {
            direction.y = 0;
            this.transform.forward = direction;
        }

        /// <summary>
        /// checks wether the character is standing on the floor
        /// </summary>
        /// <returns>false if character is in the air</returns>
        public bool IsGrounded()
        {
            return this.transform.position.y <= 0;
        }
        #endregion

        #region combatMethods
        /// <summary>
        /// relay the targetting to the attackables responsible input relay. currently only for players.
        /// </summary>
        /// <param name="attackables">all attackables to which targetting should be relayed</param>
        public void TargetAttackables(Collider[] attackables)
        {
            foreach(var attackableCollider in attackables)
            {
                PlayerInputRelay relay;
                if (attackableCollider.transform.root.TryGetComponent<PlayerInputRelay>(out relay))
                {
                    Debug.Log("playerTargeted: " + relay.gameObject.name);
                    relay.RelayPlayerTargeted();
                }
            }
        }

        /// <summary>
        /// get all objects on the layers classified as "attackable" in player metrics in this players range
        /// </summary>
        /// <returns>array of colliders in range</returns>
        public Collider[] GetAllAttackablesInRange()
        {
            return Physics.OverlapSphere(this.transform.position + new Vector3(0, metrics.PlayerScale[size - 1].x, 0), metrics.PlayerPunchRange[size - 1], LayerMask.GetMask(metrics.Mask_attackables));
        }

        /// <summary>
        /// get all objects on the layers classified as "attackable" in player metrics in this players range * rangeMod
        /// </summary>
        /// <returns>array of colliders in range</returns>
        public Collider[] GetAllAttackablesInRange(float rangeMod)
        {
            return Physics.OverlapSphere(this.transform.position + new Vector3(0, metrics.PlayerScale[size - 1].x, 0), metrics.PlayerPunchRange[size - 1] * rangeMod, LayerMask.GetMask(metrics.Mask_attackables));
        }

        /// <summary>
        /// calculates the collider with "attackPriority", specified in playerMetrics, that is most in line with this transforms forward direction
        /// </summary>
        /// <param name="attackablesInRange">all attackable colliders in range</param>
        /// <returns>the closest collider</returns>
        public Transform GetClosestAttackable(Collider[] attackablesInRange)
        {
            Collider closestCollider = null;
            float distanceToClosestCollider = float.MaxValue;

            foreach (var currentCollider in attackablesInRange)
            {
                //if not this object
                if (currentCollider.transform.root != this.transform.root)
                {
                    //creating vector to enemy in range
                    var vectorToCollider3D = currentCollider.ClosestPoint(this.transform.root.position) - this.transform.root.position;
                    var vectorToCollider = new Vector2(vectorToCollider3D.x, vectorToCollider3D.z);
                    vectorToCollider.Normalize();
                    //tracking view direction
                    var facing3D = this.transform.forward;
                    var facing = new Vector2(facing3D.x, facing3D.z);
                    facing.Normalize();

                    var distanceToCollider = (facing - vectorToCollider).magnitude;

                    //in case of higher importance or being closer to view direction, note as new closest
                    if (closestCollider == null)
                    {
                        distanceToClosestCollider = distanceToCollider;
                        closestCollider = currentCollider;
                    }
                    else if (HasAttackPriority(currentCollider) == HasAttackPriority(closestCollider))
                    {
                        if (distanceToCollider < distanceToClosestCollider)
                        {
                            distanceToClosestCollider = distanceToCollider;
                            closestCollider = currentCollider;
                        }
                    }
                    else if (HasAttackPriority(currentCollider))
                    {
                        distanceToClosestCollider = distanceToCollider;
                        closestCollider = currentCollider;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            if (closestCollider) return closestCollider.transform.root;
            else return null;
        }

        /// <summary>
        /// calculates which colliders are in a cone in this transforms view direction, its angle being specified in playerMetrics
        /// </summary>
        /// <param name="attackablesInRange">all attackable colliders in range</param>
        /// <returns>all colliders in the cone</returns>
        public Collider[] GetAllAttackablesInAttackCone(Collider[] attackablesInRange)
        {
            var attackablesInCone = new List<Collider>();
            var maxDistance = Mathf.Sin(metrics.PlayerPunchAngle[size-1] * Mathf.Deg2Rad) / Mathf.Sin((180 - metrics.PlayerPunchAngle[size-1]) * Mathf.Deg2Rad / 2);
            foreach (var item in attackablesInRange)
            {
                if (item.transform.root != this.transform.root)
                {
                    var vectorToCollider3D = item.ClosestPoint(this.transform.root.position) - this.transform.root.position;
                    var vectorToCollider = new Vector2(vectorToCollider3D.x, vectorToCollider3D.z);
                    vectorToCollider.Normalize();

                    var facing3D = this.transform.forward;
                    var facing = new Vector2(facing3D.x, facing3D.z);
                    facing.Normalize();

                    var distanceToCollider = (facing - vectorToCollider).magnitude;

                    if (distanceToCollider < maxDistance)
                    {
                        attackablesInCone.Add(item);
                    }
                }
            }
            return attackablesInCone.ToArray();
        }

        /// <summary>
        /// determines whether or not an object the given collider is attached to has priority to be attacked
        /// </summary>
        /// <param name="collider">the given collider</param>
        /// <returns>true if it has priority </returns>
        bool HasAttackPriority(Collider collider)
        {
            foreach(var layer in metrics.PriorityAttackables)
            {
                if (collider.transform.root.gameObject.layer.Equals(layer)) return true;
            }
            return false;
        }

        /// <summary>
        /// relay a hit from another character to this one. if current state allows the hit, ApplyHit() will be called,
        /// </summary>
        /// <param name="from">the characterController of the character attacking</param> 
        /// <param name="damageIntended">the [raw] damage the other character is trying to deal</param>
        /// <param name="knockbackDistanceIntended">the [raw] distance the other character is trying to knock this back</param>
        /// <param name="stunDurationIntended">the [raw] duration the other character is trying to stun this for</param>
        /// [raw] => _not_ modified by current state
        public void TryReceiveHit(PlayerController from, float damageIntended, Vector3 knockbackDistanceIntended, float stunDurationIntended, bool ignoreSize)
        {
            this.inputRelay.RelayPlayerHit(from, damageIntended, knockbackDistanceIntended, stunDurationIntended, ignoreSize);
        }

        /// <summary>
        /// apply a hit from another character to this one. modify this chracter's score and notify other characterController.
        /// </summary>
        /// <param name="from">the characterController of the character attacking</param> 
        /// <param name="damage">the [scaled] damage the other character is trying to deal</param>
        /// <param name="knockbackDistance">the [scaled] distance the other character is trying to knock this back</param>
        /// <param name="stunDuration">the [scaled] duration the other character is trying to stun this for</param>
        /// [scaled] => modified by current state
        public void ReceiveHit(PlayerController from, float damage, Vector3 knockbackDistance, float stunDuration)
        {
            //apply damage
            score -= (int) damage;
            inputRelay.RelayPlayerScoreChange(-damage);

            from.HitCallback(this, damage);
        }

        /// <summary>
        /// notify all attackables that they have been hit.
        /// </summary>
        /// <param name="attackables">all attackables to be hit</param>
        public void HitAllAttackables(Transform[] attackables, float damageIntended, float knockbackDistanceIntended, float stunDurationIntended, bool ignoreSize)
        {
            foreach (var attackable in attackables)
            {
                if (attackable.transform.root != this.transform.root)
                {
                    //when the transform contains an playerController object
                    PlayerController otherController;
                    if (attackable.TryGetComponent<PlayerController>(out otherController))
                    {
                        //calculate knockback direction
                        var directionToAttackable = attackable.transform.position - this.transform.position;
                        directionToAttackable.y = 0;
                        directionToAttackable.Normalize();
                        //try to apply the hit
                        otherController.TryReceiveHit(this, damageIntended, knockbackDistanceIntended * directionToAttackable, stunDurationIntended, ignoreSize);
                        continue;
                    }
                    else
                    {
                        //when the transform contains an attackable object
                        Attackable objectToAttack;
                        if (attackable.TryGetComponent<Attackable>(out objectToAttack))
                        {
                            //try to apply the hit
                            objectToAttack.DamageAttackable(this, damageIntended);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// notify all attackables that they have been hit. finds root transform and proceeds like other overload.
        /// </summary>
        /// <param name="attackables"></param>
        public void HitAllAttackables(Collider[] attackables, float damageIntended, float knockbackDistanceIntended, float stunDurationIntended, bool ignoreSize)
        {
            List<Transform> attackableTransforms = new List<Transform>();
            foreach (var collider in attackables)
            {
                attackableTransforms.Add(collider.transform.root);
            }
            HitAllAttackables(attackableTransforms.ToArray(), damageIntended, knockbackDistanceIntended, stunDurationIntended, ignoreSize);
        }
        
        /// <summary>
        /// apply the effects of a successful hit on another player.
        /// </summary>
        /// <param name="to">the character that was hit</param>
        /// <param name="damageDealt">the damage that was accepted by the other player.</param>
        public void HitCallback(PlayerController to, float damageDealt)
        {
            score += (int)damageDealt;
            inputRelay.RelayPlayerScoreChange(damageDealt);
        }

        /// <summary>
        /// apply the effects of a successful hit on an object.
        /// </summary>
        /// <param name="to">the object that was hit</param>
        /// <param name="damageDealt">the damage that was accepted by the other object</param>
        public void HitCallback(Attackable to, float damageDealt)
        {
            score += (int)damageDealt;
            inputRelay.RelayPlayerScoreChange(damageDealt);
        }

        /// <summary>
        /// verify and relay the death of this character
        /// </summary>
        public void Die()
        {
            if (score < 0)
            {
                inputRelay.RelayPlayerDeath();
            }
        }
        #endregion
    }
}
