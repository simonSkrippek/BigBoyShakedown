using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BigBoyShakedown.Player.Manager
{
    [RequireComponent(typeof(PlayerInputManager))]
    public class MultiplayerManager : MonoBehaviour
    {
        public PlayerInputManager inputManager;

        public event Action<Material> OnplayerJoined;

        Material m;

        private void Awake()
        {
            inputManager = GetComponent<PlayerInputManager>();
            inputManager.onPlayerJoined += OnPlayerJoinedHandler;
            OnplayerJoined += (val) => { };
        }

        private void OnPlayerJoinedHandler(PlayerInput input)
        {
            Debug.Log("playerJoined: " + input.gameObject.name + ", index: " + input.playerIndex);
            OnplayerJoined.Invoke(m);
        }
    }
}
