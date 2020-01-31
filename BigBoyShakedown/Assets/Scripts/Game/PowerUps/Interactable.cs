using BigBoyShakedown.Player.Controller;
using UnityEngine;

namespace BigBoyShakedown.Game.PowerUp
{
    public interface Interactable
    {
        MonoBehaviour GetObject();

        void StartInteraction(PlayerController player);
        void CancelInteraction();
        void CompleteInteraction();
        void DestroyInteractable();
    }
}
