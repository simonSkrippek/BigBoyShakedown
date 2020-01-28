using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BigBoyShakedown.Player.Input
{
    [RequireComponent(typeof(Animator))]
    public class AnimationCallbackRelay : MonoBehaviour
    {
        public event Action<AnimationCallbackRelay> OnWindUpComplete;
        public event Action<AnimationCallbackRelay> OnRecoveryComplete;

        private void Awake()
        {
            OnWindUpComplete += (val) => { };
            OnRecoveryComplete += (val) => { };
        }

        public void WindUpComplete()
        {
            OnWindUpComplete.Invoke(this);
        }
        public void RecoveryComplete()
        {
            OnRecoveryComplete.Invoke(this);
        }
    }
}
