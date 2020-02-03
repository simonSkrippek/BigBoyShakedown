﻿using UnityEngine;
using System;
using BigBoyShakedown.Player.Input;

namespace BigBoyShakedown.Player.Appearance
{
    public enum AnimatedAction {  
                            Idle,
                            Run, Dash,
                            Punch1, Punch2, Punch3, GetHit,
                            Interact
    }

    [RequireComponent(typeof(PlayerInputRelay))]
    public class PlayerAppearance : MonoBehaviour
    {
        [SerializeField, Header("Models"), Tooltip("all player models that are attached to this gameObject as children")] 
        GameObject[] playerModels;
        GameObject currentPlayerModel;
        [Header("AttackIndicator"), Tooltip("the attackCone attached to this object"), SerializeField] GameObject attackIndicator;

        Animator currentAnimator;

        PlayerInputRelay playerInputRelay;

        bool switchNeeded;
        int stageToSwitchTo;

        private void Awake()
        {
            playerInputRelay = this.GetComponent<PlayerInputRelay>();
        }
        private void OnEnable()
        {
            playerInputRelay.OnPlayerSizeChanged += OnPlayerSizeChangedHandler;
        }
        private void OnDisable()
        {
            playerInputRelay.OnPlayerSizeChanged -= OnPlayerSizeChangedHandler;
        }

        private void OnPlayerSizeChangedHandler(int newSize)
        {
            stageToSwitchTo = newSize;
            switchNeeded = true;
        }

        public void SwitchStage(int newSize)
        {
            if (currentPlayerModel) currentPlayerModel.SetActive(false);
            currentPlayerModel = playerModels[newSize - 1];
            currentPlayerModel.SetActive(true);

            currentAnimator = currentPlayerModel.GetComponent<Animator>();
            if (!currentAnimator)
            {
                currentAnimator = currentPlayerModel.GetComponentInChildren<Animator>();
                if (!currentAnimator) throw new MissingMemberException("Animator not found; PlayerPrefab setup compromised: " + this.gameObject.name);
            }

            switchNeeded = false;
        }

        public void PlayAnimation(AnimatedAction action)
        {
            if (switchNeeded) 
            { 
                SwitchStage(stageToSwitchTo); 
            }
            switch (action)
            {
                case AnimatedAction.Idle:
                    currentAnimator.Play("idling");
                    break;
                case AnimatedAction.Run:
                    currentAnimator.Play("running");
                    break;
                case AnimatedAction.Dash:
                    currentAnimator.Play("dashing");
                    break;
                case AnimatedAction.Punch1:
                    currentAnimator.Play("punching1");
                    break;
                case AnimatedAction.Punch2:
                    currentAnimator.Play("punching2");
                    break;
                case AnimatedAction.Punch3:
                    currentAnimator.Play("punching3");
                    break;
                case AnimatedAction.GetHit:
                    currentAnimator.Play("gettingHit");
                    break;
                case AnimatedAction.Interact:
                    currentAnimator.Play("interacting");
                    break;
            }
        }

        public void SetAnimationSpeed(float speed_)
        {
            if (speed_ <= 0) throw new Exception("animation speed cannot be zeron or less");
            currentAnimator.SetFloat("Speed", speed_);
        }

        public void ScaleAttackIndicator(float range)
        {
            attackIndicator.transform.localScale = new Vector3(range, range, range) / this.transform.localScale.x;
        }
        public void ShowAttackIndicator()
        {
            attackIndicator.SetActive(true);
        }
        public void HideAttackIndicator()
        {
            attackIndicator.SetActive(false);
        }
    }
}
