using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInputManager))]
public class PlayerManager : MonoBehaviour
{
    Time gameTime;

    #region camera
    private Camera cam;
    private Vector3 cameraForward, cameraRight;
    #endregion

    #region input
    [Header("InputManager"), Tooltip("Input manager component")]
    private PlayerInputManager inputManager;
    #endregion

    #region players
    List<Player> allPlayers;

    [Header("Player Metrics"), Tooltip("default player metrics; REQUIRED"), SerializeField]
    private PlayerMetrics playerMetrics;
    #endregion

    private void Awake()
    {
        allPlayers = new List<Player>();
        gameTime = FindObjectOfType<Time>();
        InitializeCameraVars();
    }
    private void InitializeCameraVars()
    {
        cam = Camera.main;

        cameraForward = cam.transform.forward; // Set forward to equal the camera's forward vector
        cameraForward.y = 0; // make sure y is 0
        cameraForward.Normalize(); // make sure the length of vector is set to a max of 1.0
        cameraRight = Quaternion.Euler(new Vector3(0, 90, 0)) * cameraForward; // set the right-facing vector to be facing right relative to the camera's forward vector
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ApplySize();
        ApplyMovement();
        ApplyPunch();
        ApplyInteraction();
        ApplyPowerups();
    }

    private void ApplySize()
    {
        foreach (Player player in allPlayers)
        {
            if (player.CurrentCharacterScore >= playerMetrics.PlayerScore[player.CurrentCharacterSize] || player.CurrentCharacterScore < playerMetrics.PlayerScore[player.CurrentCharacterSize - 1])
            {
                CorrectPlayerSize(player.playerIndex);
            }
        }
    }
    private void CorrectPlayerSize(int playerIndex)
    {
        var score = allPlayers[playerIndex].CurrentCharacterScore;
        for (int i = 0; i < playerMetrics.PlayerScore.Length; i++)
        {
            if (playerMetrics.PlayerScore[i] > score && playerMetrics.PlayerScore[i-1] <= score)
            {
                if (allPlayers[playerIndex].CurrentCharacterSize != i - 1)
                {
                    allPlayers[playerIndex].CurrentCharacterSize = i - 1;
                }
            }
        }
    }
    private void ApplyMovement()
    {
        foreach (var player in allPlayers)
        {
            if (player.Unstunned)
            {
                if (player.MovementAllowed)
                {
                    if (player.CurrentMovement != Vector2.zero)
                    {

                        // possibly check for collision ???

                        Vector2 currentMovementScaled = player.CurrentMovement * playerMetrics.PlayerMoveSpeed[player.CurrentCharacterSize - 1] / 8;

                        Vector3 rightMovement = cameraRight * currentMovementScaled.x;
                        Vector3 upMovement = cameraForward * currentMovementScaled.y;
                        Vector3 heading = rightMovement + upMovement;

                        player.transform.forward = heading.normalized;


                        if (player.RotationAllowed)
                        {
                            player.transform.position += heading;
                        }
                    }
                }
            }
        }
    }
    private void ApplyPunch()
    {
        throw new NotImplementedException();
    }
    private void ApplyInteraction()
    {
        throw new NotImplementedException();
    }
    private void ApplyPowerups()
    {
        throw new NotImplementedException();
    }


    private void OnEnable()
    {
        inputManager.onPlayerJoined += OnPlayerJoinedHandler;
        inputManager.EnableJoining();
    }
    private void OnDisable()
    {
        inputManager.onPlayerJoined -= OnPlayerJoinedHandler;
        inputManager.DisableJoining();
    }

    private void OnPlayerLeftHandler(PlayerInput input)
    {
        Time.PauseGame();
    }

    private void OnPlayerJoinedHandler(PlayerInput input)
    {
        var player = input.GetComponent<Player>();
        player.playerIndex = allPlayers.Count;
        allPlayers.Add(player);

        switch (player.playerIndex)
        {
            case 0:
                input.onActionTriggered += OnActionTriggeredHandler0;
                break;
            case 1:
                input.onActionTriggered += OnActionTriggeredHandler1;
                break;
            case 2:
                input.onActionTriggered += OnActionTriggeredHandler2;
                break;
            case 3:
                input.onActionTriggered += OnActionTriggeredHandler3;
                break;
        }
        input.onDeviceLost += onDeviceLostHandler;
    }

    private void onDeviceLostHandler(PlayerInput input)
    {
        throw new NotImplementedException();
    }

    #region inputHandlers
    #region bigbullcrap
    private void OnActionTriggeredHandler0(InputAction.CallbackContext obj)
    {
        OnActionTriggeredHandler(obj, 0);
    }
    private void OnActionTriggeredHandler1(InputAction.CallbackContext obj)
    {
        OnActionTriggeredHandler(obj, 1);
    }
    private void OnActionTriggeredHandler2(InputAction.CallbackContext obj)
    {
        OnActionTriggeredHandler(obj, 2);
    }
    private void OnActionTriggeredHandler3(InputAction.CallbackContext obj)
    {
        OnActionTriggeredHandler(obj, 3);
    }
    #endregion
    private void OnActionTriggeredHandler(InputAction.CallbackContext context, int playerIndex)
    {
        var player = allPlayers[playerIndex]; 
        if (context.performed)
        {
            if (context.performed)
            {
                switch (context.action.name)
                {
                    case "Move":
                        player.CurrentMovement =context.ReadValue<Vector2>();
                        break;
                    case "Punch":
                        if (player.ReadyToPunch) player.PunchTriggered = true;
                        break;
                    case "Interact":
                        if (player.ReadyToInteract) player.InteractionTriggered = true;
                        break;
                }
            }
            else if (context.canceled)
            {
                switch (context.action.name)
                {
                    case "Move":
                        player.CurrentMovement = Vector2.zero;
                        break;
                }
            }
        }
    }
    #endregion
}
