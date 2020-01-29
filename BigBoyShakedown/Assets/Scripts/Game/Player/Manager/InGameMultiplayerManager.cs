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

        [SerializeField] BillboardManager billboardManager;
        [SerializeField] CharacterSelectionData selectionData;

        [SerializeField] GameObject[] pAce;
        [SerializeField] GameObject[] pGrease;
        [SerializeField] GameObject[] pMark;
        [SerializeField] GameObject[] pSpecci;
        [SerializeField] Material[] playerMaterials = new Material[4];

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

            PlayerPrefab.SetActive(false);
            var playerObject = Instantiate(PlayerPrefab);
            var inputRelay = playerObject.GetComponent<PlayerInputRelay>();
            var playerController = playerObject.GetComponent<Controller.PlayerController>();
            inputRelay.ActivateInput(input_);
            playerObject.SetActive(true);
            PlayerPrefab.SetActive(true);

            billboardManager.OnPlayerJoined(inputRelay, playerMaterials[input_.playerIndex], input_.playerIndex);

            //spawn appropriate character based on @selection data
            //TODO
        }
    }
}
