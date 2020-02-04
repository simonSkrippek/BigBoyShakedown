using UnityEngine;
using System;
using BigBoyShakedown.Player.Input;
using BigBoyShakedown.Manager;

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
        AnimatedAction currentAnimation;

        PlayerInputRelay playerInputRelay;

        bool switchNeeded;
        int stageToSwitchTo;

        [SerializeField] GameObject subpoenaParticle;
        float[] subpoenaPositions = {0.01f, 0.01f, 0.01f, 0.01f, 0.01f };
        float[] subpoenaScale = {.7f, .1f, .15f, .2f, .27f };
        [SerializeField] GameObject moneyParticle;
        float[] moneyPositions = { 1.411f, 1.969f, 2.35f, 3.46f, 5.19f };
        float[] moneyScale = { .1f, .15f, .2f, .26f, .3f };
        [SerializeField] GameObject growParticle;
        float[] growPositions = { 0.67f, 1f, 1.04f, 1.73f, 2.67f };
        float[] growScale = { .1f, .15f, .2f, .26f, .3f };
        [SerializeField] GameObject playerMarker;
        float[] markerScale = { 3f, 4f, 5.3f, 7f, 8f };


        private void Awake()
        {
            playerInputRelay = this.GetComponent<PlayerInputRelay>();
        }
        private void OnEnable()
        {
            playerInputRelay.OnPlayerSizeChanged += OnPlayerSizeChangedHandler;
            playerInputRelay.OnPlayerScoreChanged += OnPlayerScoreChangedHandler; ;
        }


        private void OnDisable()
        {
            playerInputRelay.OnPlayerSizeChanged -= OnPlayerSizeChangedHandler;
        }

        private void OnPlayerSizeChangedHandler(int newSize)
        {
            stageToSwitchTo = newSize;
            switchNeeded = true;

            switch (currentAnimation)
            {
                case AnimatedAction.Idle:
                    PlayAnimation(AnimatedAction.Idle);
                    break;
                case AnimatedAction.Interact:
                    PlayAnimation(AnimatedAction.Interact);
                    break;
                case AnimatedAction.Run:
                    PlayAnimation(AnimatedAction.Run);
                    break;
            }
        }
        private void OnPlayerScoreChangedHandler(float change)
        {
            if (change < 0) TriggerMoneyRainEffect();
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

            playerMarker.transform.localScale = new Vector3(markerScale[newSize - 1], markerScale[newSize - 1], markerScale[newSize - 1]);
            playerMarker.SetActive(true);

            subpoenaParticle.transform.localScale = new Vector3(subpoenaScale[newSize - 1], subpoenaScale[newSize - 1], subpoenaScale[newSize - 1]);
            subpoenaParticle.transform.position = new Vector3(subpoenaParticle.transform.position.x, subpoenaPositions[newSize - 1], subpoenaParticle.transform.position.z);
            foreach (Transform transform in subpoenaParticle.transform)
            {
                transform.position = Vector3.zero;
                transform.localScale = new Vector3(subpoenaScale[newSize - 1], subpoenaScale[newSize - 1], subpoenaScale[newSize - 1]);
            }

            growParticle.transform.localScale = new Vector3(growScale[newSize - 1], growScale[newSize - 1], growScale[newSize - 1]);
            growParticle.transform.position = new Vector3(growParticle.transform.position.x, growPositions[newSize - 1], growParticle.transform.position.z);

            moneyParticle.transform.localScale = new Vector3(moneyScale[newSize - 1], moneyScale[newSize - 1], moneyScale[newSize - 1]);
            moneyParticle.transform.position = new Vector3(moneyParticle.transform.position.x, moneyPositions[newSize - 1], moneyParticle.transform.position.z);
            foreach (Transform transform in moneyParticle.transform)
            {
                transform.position = Vector3.zero;
                transform.localScale = new Vector3(moneyScale[newSize - 1], moneyScale[newSize - 1], moneyScale[newSize - 1]);
            }
            TriggerGrowEffect();
        }

        public void PlayAnimation(AnimatedAction action, float speed)
        {
            if (switchNeeded) 
            { 
                SwitchStage(stageToSwitchTo); 
            }

            SetAnimationSpeed(speed);

            currentAnimation = action;
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
        public void PlayAnimation(AnimatedAction action)
        {
            if (switchNeeded)
            {
                SwitchStage(stageToSwitchTo);
            }

            currentAnimation = action;
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

        void SetAnimationSpeed(float speed_)
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

        public void TriggerMoneyRainEffect()
        {
            moneyParticle.SetActive(true);
            foreach(Transform t in moneyParticle.transform)
            {
                t.gameObject.SetActive(true);
            }
        }
        public void TriggerGrowEffect()
        {
            growParticle.SetActive(true);
        }
        public void TriggerSubpoenaEffect()
        {
            subpoenaParticle.SetActive(true);
            foreach (Transform t in moneyParticle.transform)
            {
                t.gameObject.SetActive(true);
            }
        }

        public void PlaySound(string soundName_)
        {
            AudioManager.instance.Play(soundName_);
        }
    }
}
