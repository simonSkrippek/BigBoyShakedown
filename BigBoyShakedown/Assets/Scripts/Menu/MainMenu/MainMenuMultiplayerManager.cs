using BigBoyShakedown.Manager;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BigBoyShakedown.UI.Input
{
    public class MainMenuMultiplayerManager : MonoBehaviour
    {
        public static MainMenuMultiplayerManager instance;
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
            input_.onActionTriggered += OnActionTriggeredHandler;
        }

        private void OnActionTriggeredHandler(InputAction.CallbackContext callbackContext)
        {
            //Debug.Log("action triggered");
            if (callbackContext.started && callbackContext.action.name == "Confirm") 
                PersistentMultiplayerManager.instance.LoadScene("CharacterSelectScene");
        }
    }
}
