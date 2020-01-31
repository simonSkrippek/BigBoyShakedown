using BigBoyShakedown.Player.Controller;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BigBoyShakedown.Game.PowerUp
{
    [RequireComponent(typeof(PowerUpCallback))]
    public class SubPoena : MonoBehaviour
    {
        #region components
        [SerializeField] Collider collider;
        [SerializeField] Rigidbody rigidbody;
        #endregion
        #region player
        [SerializeField] int PLAYER_LAYER = 9;
        [SerializeField] PlayerController playerToFollow;
        #endregion
        #region lifetime
        bool following, destroyed;
        [SerializeField] float timeActive;
        #endregion
        #region knockback
        [SerializeField] float knockbackDistance = 1f;
        [SerializeField] float stunDuration = .5f;
        [SerializeField] float timeBetweenWaves = 1.5f;
        bool readyForNextWave;

        PowerUpCallback callback;
        #endregion
        #region follow
        [SerializeField] Vector3 offsetFromPlayer = new Vector3(0, 1, -2);
        [SerializeField] float smoothTime = 0.3f;
        Vector3 velocity = Vector3.zero;
        #endregion

        private void Awake()
        {
            if (!TryGetComponent<PowerUpCallback>(out callback))
            {
                throw new Exception("no PowerUpCallback component found");
            }
        }

        private void FixedUpdate()
        {
            if (Time.IsRunning)
            {
                if (following)
                {
                    if (destroyed)
                    {
                        DestroyPowerUp();
                    }
                    else
                    {
                        AdjustPosition();
                        if (readyForNextWave)
                        {
                            CastWave();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Destroy this powerUp 
        /// </summary>
        private void DestroyPowerUp()
        {
            //play end effect?
            callback.RelayPowerUpEnded();
            Destroy(this.gameObject);
        }
        /// <summary>
        /// attack all unity around in a wave
        /// </summary>
        private void CastWave()
        {
            readyForNextWave = false;
            Time.StartTimer(new VariableReference<bool>(() => readyForNextWave, (val) => { readyForNextWave = val; }).SetEndValue(true), timeBetweenWaves);

            //push back all surrounding units
            playerToFollow.HitAllAttackables(playerToFollow.GetAllAttackablesInRange(.75f), 0f, knockbackDistance, stunDuration, true);
        }
        /// <summary>
        /// smoothly move this transform to the desired position
        /// </summary>
        private void AdjustPosition()
        {
            Vector3 targetPosition = playerToFollow.transform.position + offsetFromPlayer;
            // Smoothly move the camera towards that target position
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime, float.MaxValue, UnityEngine.Time.fixedDeltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!following && other.transform.root.gameObject.layer == PLAYER_LAYER)
            {
                other.transform.root.TryGetComponent<PlayerController>(out playerToFollow);
                if (playerToFollow)
                {
                    StartFollow();
                }
            }
        }

        private void StartFollow()
        {
            following = true;
            readyForNextWave = true;
            collider.enabled = false;

            this.transform.localScale = new Vector3(.5f, .5f, .5f);

            destroyed = false;
            Time.StartTimer(new VariableReference<bool>(() => destroyed, (val) => { destroyed = val; }).SetEndValue(true), timeActive);
        }
    }
}
