using UnityEngine;
using System;
using BigBoyShakedown.Player.Input;

namespace BigBoyShakedown.Player.Appearance
{
    public enum AnimatedAction {  
                            Idle,
                            Run, Dash,
                            Punch1, Punch2, Punch3, GetHit,/*,
                            StartInteract, Interact, CompleteInteract, CancelInteract */
    }

    [RequireComponent(typeof(PlayerInputRelay))]
    public class PlayerAppearance : MonoBehaviour
    {
        [SerializeField, Header("Models"), Tooltip("all player models that are attached to this gameObject as children")] 
        GameObject[] playerModels;
        GameObject currentPlayerModel;

        Animator currentAnimator;

        PlayerInputRelay playerInputRelay;

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
            currentAnimator.StopPlayback();

            currentPlayerModel.SetActive(false);
            currentPlayerModel = playerModels[newSize - 1];
            currentPlayerModel.SetActive(true);

            currentAnimator = currentPlayerModel.GetComponent<Animator>();
            if (!currentAnimator)
            {
                currentAnimator = currentPlayerModel.GetComponentInChildren<Animator>();
                if (!currentAnimator) throw new MissingMemberException("Animator not found; PlayerPrefab setup compromised: " + this.gameObject.name);
            }
        }

        public void PlayAnimation(AnimatedAction action)
        {
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
                    currentAnimator.Play("punching_1");
                    break;
                case AnimatedAction.Punch2:
                    currentAnimator.Play("punching_2");
                    break;
                case AnimatedAction.Punch3:
                    currentAnimator.Play("punching_3");
                    break;
                case AnimatedAction.GetHit:
                    currentAnimator.Play("gettingHit");
                    break;
            }
        }
    }
}
