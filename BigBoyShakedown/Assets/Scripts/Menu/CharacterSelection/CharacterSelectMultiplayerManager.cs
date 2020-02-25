using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using BigBoyShakedown.UI.Data;
using BigBoyShakedown.Manager;

namespace BigBoyShakedown.UI.Input
{
    public class CharacterSelectMultiplayerManager : MonoBehaviour
    {
        public static CharacterSelectMultiplayerManager instance;

        public List<CharacterSelectionController> selectionControllers;
                
        public CharacterSelectionData selectionData;

        bool gameStarted;
        public int minPlayerCount = 4;

        private void Awake()
        {
            if (!instance)
            {
                instance = this;
                gameStarted = false;
                selectionData.ResetData();
            }
            else if (instance != this) Destroy(this.gameObject);
        }

        private void Update()
        {
            if (!gameStarted && CheckReady()) StartGame();
        }

        private void StartGame()
        {
            AudioManager.instance.Play("menu_transition");

            gameStarted = true;
            for (int i = 0; i < 4; i++)
            {
                var currentController = selectionControllers[i];
                if (currentController.active)
                {
                    selectionData.AddData(currentController.playerIndex, currentController.callbackContext, currentController.charImages[currentController.currentChar].name);
                } 
            }

            Debug.Log("Game Started");
            PersistentMultiplayerManager.instance.LoadScene("InGameScene");
            //throw new NotImplementedException();
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
            //return readyCount > 1;
            //FOR TESTING PURPOSES; CHANGE BACK ASAP
            return readyCount >= minPlayerCount;
        }

        public void OnPlayerJoined(PlayerInput input)
        {
            //Debug.Log("new player joined");
            var controller = selectionControllers[input.playerIndex];
            controller.Activate(input);
        }
    }
}
