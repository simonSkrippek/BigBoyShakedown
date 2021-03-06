﻿using System;
using System.Collections;
using BigBoyShakedown.Manager;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BigBoyShakedown.UI.Input
{
    public class MainMenuMultiplayerManager : MonoBehaviour
    {
        PlayerInput input;

        public static MainMenuMultiplayerManager instance;
        private void Awake()
        {
            if (!instance)
            {
                instance = this;
            }
            else if (instance != this) Destroy(this.gameObject);
        }

        private void Start()
        {
            var coroutine = PlayIntroSounds();
            StartCoroutine(coroutine);
        }

        private IEnumerator PlayIntroSounds()
        {
            AudioManager.instance.Play("The Path to Freedom (Ultra Despair Girls)");
            yield return new WaitForSeconds(4f);
            AudioManager.instance.Play("announcer_titlescreen_enter");
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
            if (callbackContext.started && callbackContext.action.name == "Confirm")
            {
                AudioManager.instance.Play("menu_confirm");
                AudioManager.instance.Play("announcer_titlescreen_play");
                PersistentMultiplayerManager.instance.LoadScene("CharacterSelectScene");
            }
            else if (callbackContext.started && callbackContext.action.name == "Start")
                PersistentMultiplayerManager.instance.QuitGame();
        }
    }
}
