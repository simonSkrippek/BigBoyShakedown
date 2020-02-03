using BigBoyShakedown.Manager;
using BigBoyShakedown.UI.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;

namespace BigBoyShakedown.UI.Input
{
    public class CharacterSelectionController : MonoBehaviour
    {
        public PlayerInput input;
        public int playerIndex;
        public CallbackContext callbackContext;

        public int currentChar;

        static string[] charOrder = { "mark", "ace", "specci", "grease" };

        public bool active;
        public bool ready;

                
        public List<GameObject> charImages = new List<GameObject>();
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
        private void OnDisable()
        {
            if (input) input.onActionTriggered -= OnActionTriggeredHandler;
        }

        private void OnActionTriggeredHandler(InputAction.CallbackContext callbackContext_)
        {
            if (callbackContext_.started)
            {
                //Debug.Log("action received: " + callbackContext_.action.name);
                switch (callbackContext_.action.name)
                {
                    case "Right":
                        if (!ready) Switch(1);
                        break;
                    case "Left":
                        if (!ready) Switch(-1);
                        break;
                    case "Confirm":
                        ready = !ready;
                        if (ready)
                        {
                            AudioManager.instance.Play("menu_confirm" + charImages[currentChar].name);
                            AudioManager.instance.Play("announcer_" + charImages[currentChar].name);
                            this.leftArrow.gameObject.SetActive(false);
                            this.rightArrow.gameObject.SetActive(false);
                        }
                        else if (!ready)
                        {
                            AudioManager.instance.StopPlaying("announcer_" + charImages[currentChar].name);
                            this.leftArrow.gameObject.SetActive(true);
                            this.rightArrow.gameObject.SetActive(true);
                        }
                        break;
                }
            }
        }

        private void Switch(int v)
        {
            if (v < 0) AudioManager.instance.Play("menu_down");
            else if ( v > 0) AudioManager.instance.Play("menu_up");
            currentChar += v;
            if (currentChar < 0) currentChar += 4;
            else if (currentChar > 3) currentChar -= 4;

            ActivateCharImage(currentChar);
        }

        public void ActivateCharImage(int currentChar)
        {
            for (int i = 0; i < charImages.Count; i++)
            {
                if (i == currentChar)
                {
                    charImages[i].SetActive(true);
                }
                else charImages[i].SetActive(false);
            }
        }
    }
}
