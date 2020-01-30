using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using BigBoyShakedown.UI.Data;
using BigBoyShakedown.Player.Input;

namespace BigBoyShakedown.Player.Manager
{    
    public class InGameMultiplayerManager : MonoBehaviour
    {
        public static InGameMultiplayerManager instance;

        [Header("other managers")]
        [SerializeField] BillboardManager billboardManager;

        [Header("player character selection data")]
        [SerializeField] CharacterSelectionData selectionData;

        [Header("player models")]
        [SerializeField] GameObject[] pAce;
        [SerializeField] GameObject[] pGrease;
        [SerializeField] GameObject[] pMark;
        [SerializeField] GameObject[] pSpecci;
        [SerializeField] Material[] playerMaterials = new Material[4];

        [Header("Timing")]
        [SerializeField] float gameStartDelay = 5f;

        [Header("spacing & positions")]
        [SerializeField] Transform[] playerSpawnPositions;

        private void Awake()
        {
            if (!instance)
            {
                instance = this;
            }
            else if (instance != this) Destroy(this.gameObject);
        }

        public void OnPlayerJoined(PlayerInput input_)
        {
            Debug.Log("playerJoined: " + input_.gameObject.name + ", index: " + input_.playerIndex);

            var PlayerPrefab = pMark[input_.playerIndex];
            //spawn appropriate character based on @selection data, currently ALWAYS MARK

            PlayerPrefab.SetActive(false);
            var playerObject = Instantiate(PlayerPrefab);
            var inputRelay = playerObject.GetComponent<PlayerInputRelay>();
            var playerController = playerObject.GetComponent<Controller.PlayerController>();
            inputRelay.ActivateInput(input_);
            inputRelay.enabled = false;
            Time.StartTimer(new VariableReference<bool>(() => { return inputRelay.enabled; }, (val) => { inputRelay.enabled = val; }).SetEndValue(true), gameStartDelay);
            playerObject.SetActive(true);
            PlayerPrefab.SetActive(true);

            billboardManager.OnPlayerJoined(inputRelay, playerMaterials[input_.playerIndex], input_.playerIndex);

            //TODO
            playerObject.transform.position = playerSpawnPositions[input_.playerIndex].position;
        }
    }
}