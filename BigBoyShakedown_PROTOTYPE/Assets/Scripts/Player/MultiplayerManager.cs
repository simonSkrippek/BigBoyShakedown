using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MultiplayerManager : MonoBehaviour
{
    public PlayerInputManager inputManager;
    
    [Header("player colors")]
    [Tooltip("array of colors that will be assigned to players, in order of joining")]
    [SerializeField] private Material[] playerColors;
    [Tooltip("default material that will be assigned if not enough materials in playerColors")]
    [SerializeField] private Material defaultMaterial;
    [Header("player rings")]
    [Tooltip("array of colors that will be assigned to player rings, in order of joining")]
    [SerializeField]
    private Material[] playerRings;
    [Tooltip("default material that will be assigned if not enough materials in playerRings")]
    [SerializeField] private Material defaultRingMaterial;

    public event Action<Material> NewPlayerJoinedEvent; 

    private void Awake()
    {
        inputManager.onPlayerJoined += OnPlayerJoinedHandler;
    }

    private void OnPlayerJoinedHandler(PlayerInput input)
    {
        var allComponents = input.gameObject.GetComponent<PLayerComponents>();

        var newPlayerMaterial = playerColors.Length > input.playerIndex? playerColors[input.playerIndex] : defaultMaterial;
        allComponents.modelMeshRenderer.material = newPlayerMaterial;
        var newRingMaterial = playerRings.Length > input.playerIndex ? playerRings[input.playerIndex] : defaultRingMaterial;
        allComponents.quadMeshRenderer.material = newRingMaterial;

        input.gameObject.transform.position = new Vector3(20,10,20);

        NewPlayerJoinedEvent?.Invoke(newPlayerMaterial);
    }

    //public int GetPlayerIndex(PlayerInput input)
    //{
    //    input.playerIndex
    //}
}
