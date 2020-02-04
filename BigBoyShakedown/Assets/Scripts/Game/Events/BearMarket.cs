using BigBoyShakedown.Game.PowerUp;
using BigBoyShakedown.Player.Controller;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BigBoyShakedown.Game.Event
{
    enum HorizontalDirection { left, right}
    enum VerticalDirection { up, down}
    [RequireComponent(typeof(EventCallback), typeof(EventSpawnPositionData))]
    public class BearMarket : MonoBehaviour
    {
        [SerializeField] int PLAYER_LAYER = 9;
        [SerializeField] GameObject attackIndicator;
        [SerializeField] float damagePerHit = 250f, knockbackPerHit = 5f, stunPerHit = 1.5f, windUpDuration = 2f, recoveryDuration = 3f, lifetime = 1f;
        bool readyToHit, inWindUp, inRecovery, punchCompleted, destroyed, animationRunning;
        [SerializeField] float range = 4f;
        float attackConeAngle = 72f;
        [SerializeField] Animator animator;
        [SerializeField] SpriteRenderer bearImage;
        [SerializeField] Sprite roar1, roar2, swipe1, swipe2, swipeBack1, swipeBack2;

        EventCallback eventCallback;

        HorizontalDirection lookDirectionHorizonatal;
        VerticalDirection lookDirectionVertical;

        private void Awake()
        {
            if (!this.TryGetComponent<EventCallback>(out eventCallback))
            {
                throw new Exception("problem with event prefab; no eventCallback Component found");
            }
            readyToHit = true;
            attackIndicator.SetActive(false);

            Manager.AudioManager.instance.Play("event_bear");
        }
        private void Start()
        {
            destroyed = false;
            Time.StartTimer(new VariableReference<bool>(() => destroyed, (val) => { destroyed = val; }).SetEndValue(true), lifetime);
        }
        private void OnEnable()
        {
            animator.enabled = false;
        }
        private void OnDisable()
        {
            animator.enabled = true;
        }
        private void Update()
        {
            if (Time.IsRunning)
            {
                if (!destroyed)
                {
                    if (readyToHit)
                    {
                        StartHit();
                    }
                    else if (!inWindUp && !inRecovery && !punchCompleted)
                    {
                        WindUpComplete();
                    }
                    else if (!inWindUp && !inRecovery && punchCompleted)
                    {
                        RecoveryComplete();
                    }
                }
                else if (!animationRunning) StartDestroyAnim();
            }
        }

        private void StartHit()
        {
            readyToHit = false;
            inWindUp = true;
            inRecovery = false;
            punchCompleted = false;
            Time.StartTimer(new VariableReference<bool>(() => inWindUp, (val) => { inWindUp = val; }).SetEndValue(false), recoveryDuration);

            TurnCone();
            
            if (lookDirectionHorizonatal == HorizontalDirection.left)
            {
                bearImage.flipX = false;
            }
            else
            {
                bearImage.flipX = true;
            }
            if (lookDirectionVertical == VerticalDirection.down)
            {
                bearImage.sprite = swipe1;
            }
            else
            {
                bearImage.sprite = swipeBack1;
            }
        }
        private void TurnCone()
        {
            attackIndicator.SetActive(true);
            attackIndicator.transform.localScale = new Vector3(range, range, range);
            var angle = Random.Range(0f, 360f);
            attackIndicator.transform.SetPositionAndRotation(attackIndicator.transform.position, Quaternion.Euler(attackIndicator.transform.rotation.eulerAngles.x, angle, 0f));

            if (angle >= 0 && angle < 90)
            {
                lookDirectionHorizonatal = HorizontalDirection.left;
                lookDirectionVertical = VerticalDirection.down;
            }
            else if (angle >= 90 && angle < 180)
            {
                lookDirectionHorizonatal = HorizontalDirection.left;
                lookDirectionVertical = VerticalDirection.up;
            }
            else if (angle >= 180 && angle < 270)
            {
                lookDirectionHorizonatal = HorizontalDirection.right;
                lookDirectionVertical = VerticalDirection.up;
            }
            else
            {
                lookDirectionHorizonatal = HorizontalDirection.right;
                lookDirectionVertical = VerticalDirection.down;
            }
        }
        public void WindUpComplete()
        {
            readyToHit = false;
            inWindUp = false;
            inRecovery = true;
            punchCompleted = true;
            Time.StartTimer(new VariableReference<bool>(() => inRecovery, (val) => { inRecovery = val; }).SetEndValue(false), recoveryDuration);

            CompleteHit();
        }
        public void CompleteHit()
        {
            int roll = Random.Range(0, 4);
            Manager.AudioManager.instance.Play("swing_pitch_" + roll);
            attackIndicator.SetActive(false);
            var attackables = GetAllAttackablesInAttackCone(GetAllAttackablesInRange());
            if (attackables.Length > 0)
            {
                roll = Random.Range(0, 3);
                Manager.AudioManager.instance.Play("combo_impact_" + roll);
            }
            HitAllAttackables(attackables, damagePerHit, knockbackPerHit, stunPerHit, true);
            
            if (lookDirectionHorizonatal == HorizontalDirection.left)
            {
                bearImage.flipX = false;
            }
            else
            {
                bearImage.flipX = true;
            }
            if (lookDirectionVertical == VerticalDirection.down)
            {
                bearImage.sprite = swipe2;
            }
            else
            {
                bearImage.sprite = swipeBack2;
            }
        }
        private void RecoveryComplete()
        {
            readyToHit = true;
            inWindUp = false;
            inRecovery = false;
            punchCompleted = false;
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
                        otherController.TryReceiveHit(null, damageIntended, knockbackDistanceIntended * directionToAttackable, stunDurationIntended, ignoreSize);
                        continue;
                    }
                    else
                    {
                        //when the transform contains an attackable object
                        Attackable objectToAttack;
                        if (attackable.TryGetComponent<Attackable>(out objectToAttack))
                        {
                            //try to apply the hit
                            objectToAttack.DamageAttackable(null, damageIntended);
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
        /// calculates which colliders are in a cone in this transforms view direction, its angle being specified in playerMetrics
        /// </summary>
        /// <param name="attackablesInRange">all attackable colliders in range</param>
        /// <returns>all colliders in the cone</returns>
        public Collider[] GetAllAttackablesInAttackCone(Collider[] attackablesInRange)
        {
            var attackablesInCone = new List<Collider>();
            var maxDistance = Mathf.Sin(attackConeAngle * Mathf.Deg2Rad) / Mathf.Sin((180 - attackConeAngle) * Mathf.Deg2Rad / 2);
            foreach (var item in attackablesInRange)
            {
                if (item.transform.root != this.transform.root)
                {
                    var vectorToCollider3D = item.ClosestPoint(this.transform.root.position) - this.transform.root.position;
                    var vectorToCollider = new Vector2(vectorToCollider3D.x, vectorToCollider3D.z);
                    vectorToCollider.Normalize();

                    var facing3D = attackIndicator.transform.forward;
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
        /// get all objects on the layers classified as "attackable" in player metrics in this players range
        /// </summary>
        /// <returns>array of colliders in range</returns>
        public Collider[] GetAllAttackablesInRange()
        {
            var colliders = Physics.OverlapSphere(this.transform.position, range, LayerMask.GetMask("Player"), QueryTriggerInteraction.Collide);
            //if (colliders.Length > 0)
            //{
            //    Debug.LogWarning("found sth to hit");
            //}
            return colliders;
        }
                
        private void StartDestroyAnim()
        {
            if (inWindUp) CompleteHit();
            attackIndicator.SetActive(false);
            animator.enabled = true;
            animationRunning = true;
            animator.Play("bearMarket_end");
        }
        private void DestroyEvent()
        {
            eventCallback.RelayEventEnded();
            Destroy(this.gameObject);
        }
    }
}