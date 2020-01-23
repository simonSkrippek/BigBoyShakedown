using BigBoyShakedown.UI.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace BigBoyShakedown.UI.Input
{
    public class CharacterSelectionController : MonoBehaviour
    {
        public PlayerInput input;
        public int playerIndex;

        [SerializeField] float timeBetweenSwitches = 1f;
        bool canSwitch = true;

        public int currentChar;

        static string[] charOrder = { "mark", "ace", "specci", "grease" };

        public bool active;
        public bool ready;


        [SerializeField]
        List<GameObject> charImages = new List<GameObject>();
        public Image leftArrow, rightArrow;
        
        public void Activate (PlayerInput input_)
        {
            active = true;
            this.input = input_;
            this.playerIndex = input.playerIndex;

            this.ActivateCharImage(currentChar);
            this.leftArrow.gameObject.SetActive(true);
            this.rightArrow.gameObject.SetActive(true);

            input.onActionTriggered += OnActionTriggeredHandler;
        }

        private void OnActionTriggeredHandler(InputAction.CallbackContext callbackContext)
        {
            Debug.Log("action received: " + callbackContext.action.name);
            if (callbackContext.started)
            {
                switch (callbackContext.action.name)
                {
                    case "Right":
                        if (!ready) Switch(1);
                        break;
                    case "Left":
                        if (!ready) Switch(-1);
                        break;
                    case "Confirm":
                        ready = !ready;
                        break;
                }
            }
        }

        private void Switch(int v)
        {
            currentChar += v;
            if (currentChar < 0) currentChar += 4;
            else if (currentChar > 3) currentChar -= 4;

            ActivateCharImage(currentChar);
        }

        public void ActivateCharImage(int currentChar)
        {
            for (int i = 0; i < charImages.Count; i++)
            {
                if (i == currentChar) charImages[i].SetActive(true);
                else charImages[i].SetActive(false);
            }
        }
    }
}
