using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnvironmentManager : MonoBehaviour
{
    public PlayerMetrics playerMetrics;
    public PlayerInputManager inputManager;

    private List<int> scoreList = new List<int>();


    private void Awake()
    {
        //inputManager.onPlayerJoined += OnPlayerJoinedHandler;
    }

    private void OnPlayerJoinedHandler(PlayerInput input)
    {
        var allComponents = input.GetComponent<PlayerComponents>();
       // allComponents.playerController.ObjectHitEvent += ObjectHitEventHandler;
    }

    private void ObjectHitEventHandler(PlayerController originPLayer, Transform hitObject, int damage)
    {
        throw new NotImplementedException();
    }
}
