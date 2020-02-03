using BigBoyShakedown.Game.Data;
using BigBoyShakedown.Manager;
using BigBoyShakedown.UI.Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BigBoyShakedown.UI.Input
{
    public class VictoryScreenManager : MonoBehaviour
    {
        PlayerInput input;
        [SerializeField] GameObject victoryImageManager;
        [SerializeField] GameObject overlayPanel;
        [SerializeField] VictoryData victoryData;

        public static VictoryScreenManager instance;
        bool listening;

        private void Awake()
        {
            if (!instance)
            {
                instance = this;
            }
            else if (instance != this) Destroy(this.gameObject);

            listening = false;
        }

        private void Start()
        {
            victoryImageManager.GetComponent<PlayerImageManager>().ActivatePlayerImage(victoryData.characterName, victoryData.playerIndex);
        }

        public void OnPlayerJoined(PlayerInput input_)
        {
            if (!input)
            {
                input = input_;
                input.onActionTriggered += OnActionTriggeredHandler;
            }
        }

        private void OnActionTriggeredHandler(InputAction.CallbackContext callbackContext)
        {
            //Debug.Log("action triggered");
            if (listening)
            {
                if (callbackContext.started && callbackContext.action.name == "Confirm")
                {
                    AudioManager.instance.Play("menu_confirm");
                    PersistentMultiplayerManager.instance.LoadScene("CharacterSelectScene");
                }
                else if (callbackContext.started && callbackContext.action.name == "Start")
                {
                    var roll = Random.Range(1, 5);
                    AudioManager.instance.Play("announcer_exit" + roll);
                    PersistentMultiplayerManager.instance.QuitGame();
                }
            }
            else
            {
                listening = true;
                overlayPanel.SetActive(true);
            }
        }
    }
}
