using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BigBoyShakedown.Game.PowerUp
{
    public class PowerUpCallback : MonoBehaviour
    {
        public event Action OnPowerUpEnded;

        public void RelayPowerUpEnded()
        {
            OnPowerUpEnded?.Invoke();
        }
    }
}
