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

        [SerializeField] CharacterSelectionData selectionData;

        [SerializeField] GameObject PlayerPrefab;

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

            PlayerPrefab.SetActive(false);
            var go = Instantiate(PlayerPrefab);
            var inputRelay = go.GetComponent<PlayerInputRelay>();
            inputRelay.ActivateInput(input_);
            go.SetActive(true);
            PlayerPrefab.SetActive(true);

            //spawn appropriate character based on @selection data
            //TODO
        }
    }
}
