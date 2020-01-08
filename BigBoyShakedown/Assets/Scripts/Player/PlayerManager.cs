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
        if (Time.IsRunning)
        {
            foreach (Player player in allPlayers)
            {
                ApplySize(player);
                ApplyPowerups(player);
                if (!player.Stunned.Get())
                {
                    ApplyMovement(player);
                    ApplyPunch(player);
                    ApplyInteraction(player);
                }
            }
        }
    }

    private void ApplySize(Player player)
    {
        if (player.CurrentCharacterScore >= playerMetrics.PlayerScore[player.CurrentCharacterSize] || player.CurrentCharacterScore < playerMetrics.PlayerScore[player.CurrentCharacterSize - 1])
        {
            CorrectPlayerSize(player.playerIndex);
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

    private void ApplyPowerups(Player player)
    {
        throw new NotImplementedException();
    }

    private void ApplyMovement(Player player)
    {
        if (player.MovementAllowed.Get() && player.CurrentMovement != Vector2.zero)
        {
            // possibly check for collision ???

            Vector2 currentMovementScaled = player.CurrentMovement * playerMetrics.PlayerMoveSpeed[player.CurrentCharacterSize - 1] / 8;

            Vector3 rightMovement = cameraRight * currentMovementScaled.x;
            Vector3 upMovement = cameraForward * currentMovementScaled.y;
            Vector3 heading = rightMovement + upMovement;

            player.transform.forward = heading.normalized;


            if (player.RotationAllowed.Get())
            {
                player.transform.position += heading;
            }
        }
    }
    private void ApplyPunch(Player player)
    {
        if (player.PunchTriggered.Get())
        {
            player.StartPunch();

            Collider[] results = Physics.OverlapSphere(player.transform.position, playerMetrics.PlayerPunchRange[player.CurrentCharacterSize - 1], LayerMask.GetMask(playerMetrics.PunchingLayers));
            var closestCollider = SnapToClosest(results, "Player");
            if (closestCollider != null)
            {
                var newForward = closestCollider.transform.position - this.transform.position;
                newForward.y = 0;
                player.transform.forward = newForward.normalized;
                player.CurrentMovement = Vector2.zero;
            }
            else
            {
                player.CurrentMovement = Vector2.zero;
            }
        }
        else
        {
            Debug.Log("Cooldown");
        }

    }
    private List<Transform> GetAllEnemiesInCone(Collider[] results)
    {
        var enemiesHit = new List<Transform>();
        var maxDistance = Mathf.Sin(playerMetrics.PlayerPunchAngle * Mathf.Deg2Rad) / Mathf.Sin((180 - playerMetrics.PlayerPunchAngle) * Mathf.Deg2Rad / 2);
        foreach (var item in results)
        {
            if (item.transform.root != this.transform.root)
            {
                var vectorToCollider3D = item.ClosestPoint(this.transform.root.position) - this.transform.root.position;
                var vectorToCollider = new Vector2(vectorToCollider3D.x, vectorToCollider3D.z);
                vectorToCollider.Normalize();

                var facing3D = this.transform.forward;
                var facing = new Vector2(facing3D.x, facing3D.z);
                facing.Normalize();

                var distanceToCollider = (facing - vectorToCollider).magnitude;

                if (distanceToCollider < maxDistance)
                {
                    enemiesHit.Add(item.transform.root);
                }
            }
        }
        return enemiesHit;
    }
    private Collider SnapToClosest(Collider[] results, string priority)
    {
        Collider closestCollider = null;
        float distanceToClosestCollider = float.MaxValue;
        foreach (var item in results)
        {
            if (item.transform.root != this.transform.root)
            {
                //creating vector to enemy in range
                var vectorToCollider3D = item.ClosestPoint(this.transform.root.position) - this.transform.root.position;
                var vectorToCollider = new Vector2(vectorToCollider3D.x, vectorToCollider3D.z);
                vectorToCollider.Normalize();
                //tracking view direction
                var facing3D = this.transform.forward;
                var facing = new Vector2(facing3D.x, facing3D.z);
                facing.Normalize();

                var distanceToCollider = (facing - vectorToCollider).magnitude;
                //in case of higher importance or being closer to view direction, note as new closest
                int priorityLayer = LayerMask.NameToLayer(priority);
                if (closestCollider == null)
                {
                    distanceToClosestCollider = distanceToCollider;
                    closestCollider = item;
                }
                else if (item.gameObject.layer == closestCollider.gameObject.layer)
                {
                    if (distanceToCollider < distanceToClosestCollider)
                    {
                        distanceToClosestCollider = distanceToCollider;
                        closestCollider = item;
                    }
                }
                else if (item.gameObject.layer == priorityLayer)
                {
                    distanceToClosestCollider = distanceToCollider;
                    closestCollider = item;
                }
                else
                {
                    continue;
                }
            }
        }
        return closestCollider;
    }
    private void ApplyInteraction(Player player)
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
                        if (player.ReadyToPunch.Get()) player.PunchTriggered.Set(true);
                        break;
                    case "Interact":
                        if (player.ReadyToInteract.Get()) player.InteractionTriggered.Set(true);
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
