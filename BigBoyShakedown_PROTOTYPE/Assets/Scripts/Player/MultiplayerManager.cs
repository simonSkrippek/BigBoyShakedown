using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MultiplayerManager : MonoBehaviour
{
    public PlayerInputManager inputManager;
    [Header("Player Metrics"), Tooltip("default player metrics; REQUIRED"), SerializeField]
    private PlayerMetrics playerMetrics;

    public event Action<Material> NewPlayerJoinedEvent; 

    private void Awake()
    {
        inputManager.onPlayerJoined += OnPlayerJoinedHandler;
    }

    private void OnPlayerJoinedHandler(PlayerInput input)
    {
        var allComponents = input.gameObject.GetComponent<PlayerComponents>();

        var newPlayerMaterial = playerMetrics.PlayerColors.Length > input.playerIndex? playerMetrics.PlayerColors[input.playerIndex] : playerMetrics.DefaultMaterial;
        allComponents.modelMeshRenderer.material = newPlayerMaterial;
        var newRingMaterial = playerMetrics.PlayerRings.Length > input.playerIndex ? playerMetrics.PlayerColors[input.playerIndex] : playerMetrics.DefaultMaterial;
        allComponents.quadMeshRenderer.material = newRingMaterial;

        input.gameObject.transform.position = new Vector3(20,10,20);

        NewPlayerJoinedEvent?.Invoke(newPlayerMaterial);
    }

    //public int GetPlayerIndex(PlayerInput input)
    //{
    //    input.playerIndex
    //}
}
