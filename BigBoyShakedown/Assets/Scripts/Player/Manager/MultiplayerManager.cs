using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BigBoyShakedown.Player.Manager
{
    public class MultiplayerManager : MonoBehaviour
    {
        public PlayerInputManager inputManager;

        private void Awake()
        {
            inputManager.onPlayerJoined += OnPlayerJoinedHandler;
        }

        private void OnPlayerJoinedHandler(PlayerInput input)
        {
            
        }
    }
}
