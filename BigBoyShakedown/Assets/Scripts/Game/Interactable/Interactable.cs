using BigBoyShakedown.Player.Controller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField] float timeUntilCompletion;
    float timeLeftUntilCompletion;
    bool beingInteractedWith;
    [SerializeField] float monetaryReward;
    PlayerController interactingPlayer;

    public void StartInteraction(PlayerController player)
    {

    }
    public void CancelInteraction()
    {

    }
    public void CompleteInteraction()
    {

    }
}
