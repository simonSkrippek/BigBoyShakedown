using BigBoyShakedown.Player.Controller;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BigBoyShakedown.Game.Event
{
    [RequireComponent(typeof(EventCallback), typeof(EventSpawnPositionData))]
    public class BullMarket : MonoBehaviour
    {
        #region player
        [SerializeField] int PLAYER_LAYER;
        #endregion
        #region theGiving
        [SerializeField] float moneyPerGive = 150f;
        [SerializeField] float range = 10f;
        bool readyForNextGive;
        [SerializeField] float timeBetweenGives;
        #endregion
        #region lifetime
        [SerializeField] float lifetime = 30f;
        bool destroyed, lifetimeOver, stoppedGiving;
        #endregion

        [SerializeField] GameObject indicator;
        EventCallback eventCallback;
        [SerializeField] Animator animator;

        private void Awake()
        {
            if (!this.TryGetComponent<EventCallback>(out eventCallback))
            {
                throw new Exception("problem with event prefab; no eventCallback Component found");
            }
            if (!this.TryGetComponent<Animator>(out animator))
            {
                throw new Exception("problem with event prefab; no animator Component found");
            }
            PLAYER_LAYER = 9;

            Manager.AudioManager.instance.Play("event_bullmarket");
        }

        private void Update()
        {
            if (Time.IsRunning)
            {
                if (readyForNextGive && !stoppedGiving)
                {
                    GiveMoney();
                }
                if (lifetimeOver)
                {
                    StopGiving();
                }
            }
        }

        public void StartGiving()
        {
            indicator.transform.localScale = new Vector3(range * 2, range * 2, range * 2);
            indicator.SetActive(true);
            GiveMoney();
            animator.Play("BullMarketMain");

            Time.StartTimer(new VariableReference<bool>(() => lifetimeOver, (val) => { lifetimeOver = val; }).SetEndValue(true), lifetime);
        }
        private void GiveMoney()
        {
            readyForNextGive = false;
            Time.StartTimer(new VariableReference<bool>(() => readyForNextGive, (val) => { readyForNextGive = val; }).SetEndValue(true), timeBetweenGives);
            var playersInRange = Physics.OverlapSphere(this.transform.position, range, LayerMask.GetMask("Player"), QueryTriggerInteraction.Collide);
            foreach (var item in playersInRange)
            {
                PlayerController playerController;
                item.transform.root.TryGetComponent<PlayerController>(out playerController);
                if (playerController)
                {
                    playerController.HitCallback((PlayerController)null, moneyPerGive);
                }
            }
        }
        public void StopGiving()
        {
            indicator.SetActive(false);
            stoppedGiving = true;
            animator.Play("BullMarketExit");
        }
        public void DestroyEvent()
        {
            eventCallback.RelayEventEnded();
            Destroy(this.gameObject);
        }
    }
}
