using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BigBoyShakedown.UI.Input
{
    [RequireComponent(typeof(PlayerInputManager))]
    public class UIMultiplayerManager : MonoBehaviour
    {
        [SerializeField]
        List<CharacterSelectionController> selectionControllers;
        PlayerInputManager inputManager;

        private void Awake()
        {
            inputManager = GetComponent<PlayerInputManager>();
        }

        private void Update()
        {
            if (CheckReady()) StartGame();
        }

        private void StartGame()
        {
            Debug.Log("Game Started");
            throw new NotImplementedException();
        }

        private bool CheckReady()
        {
            int readyCount = 0;
            for (int i = 0; i < 4; i++)
            {
                if (selectionControllers[i].active)
                {
                    if (!selectionControllers[i].ready)
                    {
                        return false;
                    }
                    else
                    {
                        readyCount++;
                    }
                }
            }
            return readyCount > 1;
        }

        private void OnEnable()
        {
            inputManager.onPlayerJoined += OnPlayerJoinedHandler;
        }
        private void OnDisable()
        {
            inputManager.onPlayerJoined -= OnPlayerJoinedHandler;
        }
        private void OnPlayerJoinedHandler(PlayerInput input)
        {
            Debug.Log("new player joined");
            var controller = selectionControllers[input.playerIndex];
            controller.Activate(input);
        }
    }
}
