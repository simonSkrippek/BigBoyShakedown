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

    public event Action<Material> NewPlayerJoinedEvent; 

    private void Awake()
    {
        inputManager.onPlayerJoined += OnPlayerJoinedHandler;
    }

    private void OnPlayerJoinedHandler(PlayerInput input)
    {
        var newPlayerMaterial = playerColors.Length > input.playerIndex? playerColors[input.playerIndex] : defaultMaterial;
        input.gameObject.GetComponent<MeshRenderer>().material = newPlayerMaterial;

        input.gameObject.transform.position = new Vector3(20,3,20);

        NewPlayerJoinedEvent?.Invoke(newPlayerMaterial);
    }

    //public int GetPlayerIndex(PlayerInput input)
    //{
    //    input.playerIndex
    //}
}
