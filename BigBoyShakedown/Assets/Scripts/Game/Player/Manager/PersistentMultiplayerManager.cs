﻿using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using BigBoyShakedown.UI.Data;
using BigBoyShakedown.UI.Input;
using BigBoyShakedown.Player.Manager;
using BigBoyShakedown.Game.Data;
using System;

namespace BigBoyShakedown.Manager
{
    enum CurrentScene { MainMenu, Options, CharacterSelection, InGame, Victory}
    [RequireComponent(typeof(PlayerInputManager))]
    public class PersistentMultiplayerManager : MonoBehaviour
    {
        public static PersistentMultiplayerManager instance;

        CurrentScene currentScene;

        PlayerInputManager inputManager;
        [SerializeField] CharacterSelectionData selectionData;
        [SerializeField] VictoryData victoryData;
        [SerializeField] PlayerInput[] playerInputs = { };



        #region unityEvents
        private void Awake()
        {
            if (!instance)
            {
                instance = this;
                DontDestroyOnLoad(this.gameObject);
                inputManager = this.GetComponent<PlayerInputManager>();
                inputManager.onPlayerJoined += OnPlayerJoinedHandler;
            }
            else if (instance != this) Destroy(this.gameObject);
        }
        private void OnEnable()
        {
            //Debug.Log("enabled");
            SceneManager.sceneLoaded += SceneLoadedHandler;
        }
        private void OnDisable()
        {
            //Debug.Log("disabled");
            SceneManager.sceneLoaded -= SceneLoadedHandler;
        }
        #endregion

        public void LoadScene(string sceneName_)
        {
            SceneManager.LoadScene(sceneName_);
        }

        private void SceneLoadedHandler(Scene currentScene_, LoadSceneMode loadSceneMode_)
        {
            switch (currentScene_.name)
            {
                case "InGameScene":
                    currentScene = CurrentScene.InGame;
                    break;
                case "CharacterSelectScene":
                    currentScene = CurrentScene.CharacterSelection;
                    break;
                case "MainMenuScene":
                    currentScene = CurrentScene.MainMenu;
                    break;
                case "OptionsScene":
                    currentScene = CurrentScene.Options;
                    break;
                case "VictoryScene":
                    currentScene = CurrentScene.Victory;
                    break;
            }
            HandleCurrentScene();
        }
        private void HandleCurrentScene()
        {
            switch (currentScene)
            {
                case CurrentScene.MainMenu:
                    playerInputs = new PlayerInput[1];
                    break;
                case CurrentScene.Options:
                    break;
                case CurrentScene.CharacterSelection:
                    foreach (var item in playerInputs)
                    {
                        if (item) Destroy(item.gameObject);
                    }
                    playerInputs = new PlayerInput[4];
                    break;
                case CurrentScene.InGame:
                    InGameMultiplayerManager.instance.OnPLayerWon += InGameMultiplayerManager_OnPlayerWon;
                    for (int i = 0; i < playerInputs.Length; i++)
                    {
                        var input = playerInputs[i];
                        if (input)
                        {
                            input.SwitchCurrentActionMap("InGame");
                            InGameMultiplayerManager.instance.OnPlayerJoined(input);
                        }
                    }
                    break;
                case CurrentScene.Victory:
                    var controllingPlayer = playerInputs[victoryData.playerIndex];
                    controllingPlayer.SwitchCurrentActionMap("Menu");
                    VictoryScreenManager.instance.OnPlayerJoined(controllingPlayer);
                    break;
            }
        }

        private void InGameMultiplayerManager_OnPlayerWon(int index)
        {
            InGameMultiplayerManager.instance.OnPLayerWon -= InGameMultiplayerManager_OnPlayerWon;

            victoryData.playerIndex = index;
            victoryData.characterName = selectionData.playerJoinData[index].characterName;

            LoadScene("VictoryScene");
        }

        private void OnPlayerJoinedHandler(PlayerInput input_)
        {
            Debug.Log("playerJoined: " + input_.gameObject.name + ", index: " + input_.playerIndex);

            if (input_.playerIndex < playerInputs.Length && input_.playerIndex >= 0)
            {
                playerInputs[input_.playerIndex] = input_;
                DontDestroyOnLoad(input_);
            }

            switch (currentScene)
            {
                case CurrentScene.MainMenu:
                    input_.SwitchCurrentActionMap("Menu");
                    MainMenuMultiplayerManager.instance.OnPlayerJoined(input_);
                    break;
                case CurrentScene.CharacterSelection:
                    input_.SwitchCurrentActionMap("Menu");
                    CharacterSelectMultiplayerManager.instance.OnPlayerJoined(input_);
                    break;
            }
        }

        
        public void QuitGame()
        {
            Application.Quit(0);
        }
    }
}
