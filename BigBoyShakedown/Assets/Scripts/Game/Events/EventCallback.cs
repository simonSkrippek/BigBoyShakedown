using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BigBoyShakedown.Game.Event
{
    public class EventCallback : MonoBehaviour
    {
        public event Action OnEventEnded;

        public void RelayEventEnded()
        {
            OnEventEnded?.Invoke();
        }
    }
}