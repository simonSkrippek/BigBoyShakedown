using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using BigBoyShakedown.UI.Data;
using BigBoyShakedown.UI.Input;
using BigBoyShakedown.Player.Manager;
using BigBoyShakedown.Game.Data;

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
            if (currentScene == CurrentScene.InGame)
            {
                InGameTransitionPlayer.instance.animationComplete += InGameAnimComplete;
                InGameTransitionPlayer.instance.PlayEndOfLevelAnimation();
            }
            else  SceneManager.LoadScene(sceneName_);
        }
        private void InGameAnimComplete()
        {
            InGameTransitionPlayer.instance.animationComplete -= InGameAnimComplete;
            SceneManager.LoadScene("VictoryScene");
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
                    AudioManager.instance.StopPlaying();
                    AudioManager.instance.Play("The Path to Freedom (Ultra Despair Girls)");
                    playerInputs = new PlayerInput[1];
                    break;
                case CurrentScene.Options:
                    AudioManager.instance.Play("The Path to Freedom (Ultra Despair Girls)");
                    break;
                case CurrentScene.CharacterSelection:
                    AudioManager.instance.StopPlaying("The Path to Freedom (Ultra Despair Girls)");
                    AudioManager.instance.Play("Gimme, Gimme! (Options)");
                    foreach (var item in playerInputs)
                    {
                        if (item) Destroy(item.gameObject);
                    }
                    playerInputs = new PlayerInput[4];
                    break;
                case CurrentScene.InGame:
                    AudioManager.instance.StopPlaying("Gimme, Gimme! (Options)");
                    var roll = Random.Range(0, 2);
                    switch (roll)
                    {
                        case 0:
                            AudioManager.instance.Play("The Baseball Bat (Grease's Theme)");
                            break;
                        case 1:
                            AudioManager.instance.Play("The Computer (Speccy's Theme)");
                            break;

                    }
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
                    AudioManager.instance.StopPlaying();
                    AudioManager.instance.Play("The Path to Freedom (Ultra Despair Girls)");
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
