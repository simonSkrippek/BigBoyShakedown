using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BigBoyShakedown.Player.Input
{
    public class AnimationCallbackRelay : MonoBehaviour
    {
        public event Action OnWindUpComplete;
        public event Action OnRecoveryComplete;

        private void Awake()
        {
            OnWindUpComplete += () => { };
            OnRecoveryComplete += () => { };
        }

        public void WindUpComplete()
        {
            OnWindUpComplete.Invoke();
        }
        public void RecoveryComplete()
        {
            OnRecoveryComplete.Invoke();
        }
    }
}
