using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

[RequireComponent(typeof(PlayerInput), typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    #region camera
    private Camera cam;
    private Vector3 cameraForward, cameraRight;
    #endregion

    #region movement
    [Header("character size")]
    [Tooltip("current size of character; DO NOT CHANGE")]
    [SerializeField] private int characterSize = 1;
    private bool sizeChanged = false;

    private Vector2 currentMovement;
    #endregion

    #region combat
    [Header("Combat"), Tooltip("attack angle for normal punches"), SerializeField]
    float attackAngle = 60;
    private float timeToNextPunch = 1.5f;
    private bool readyToPunch = true;

    #endregion

    #region components
    private PlayerComponents components;
    [Header("Player Metrics"), Tooltip("default player metrics; REQUIRED"), SerializeField]
    private PlayerMetrics playerMetrics;
    #endregion

    #region models
    [Header("Models"), Tooltip("character models, mapped to stances \n REQUIRED: Punch, Idle"), SerializeField]
    private Mesh punchingModel, idleModel;
    #endregion

    private void Awake()
    {
        components = this.transform.root.GetComponent<PlayerComponents>();

        cam = Camera.main;

        cameraForward = cam.transform.forward; // Set forward to equal the camera's forward vector
        cameraForward.y = 0; // make sure y is 0
        cameraForward.Normalize(); // make sure the length of vector is set to a max of 1.0
        cameraRight = Quaternion.Euler(new Vector3(0, 90, 0)) * cameraForward; // set the right-facing vector to be facing right relative to the camera's forward vector

        LoadPlayerMetrices();

        components.playerInput.onActionTriggered += HandleInputAction;
    }

    #region handleInput
    private void HandleInputAction(CallbackContext context)
    {
        if (context.performed)
        {
            switch (context.action.name)
            {
                case "Move":
                    HandleMoveAction(context.ReadValue<Vector2>());
                    break;
                case "Punch":
                    HandlePunchAction();
                    break;
                case "Grow":
                    HandleResizeAction(1f);
                    break;
                case "Shrink":
                    HandleResizeAction(-1f);
                    break;
            }
        }
        else if (context.canceled)
        {
            switch (context.action.name)
            {
                case "Move":
                    currentMovement = Vector2.zero;
                    break;
            }
        }
    }
    private void HandleMoveAction(Vector2 movement)
    {
        currentMovement = movement;
    }
    private void HandlePunchAction()
    {
        if (readyToPunch)
        {
            Debug.Log("punch");
            readyToPunch = false;
            ChangeModel("Punch");
            timeToNextPunch = playerMetrics.PlayerPunchSpeed[characterSize-1];

            Collider[] results = Physics.OverlapSphere(this.transform.position, playerMetrics.PlayerPunchRange[characterSize-1], LayerMask.GetMask(new string[] { "Player", "test" }));
            var closestCollider = SnapToClosest(results, "Player");
            if (closestCollider != null)
            {
                var newForward = closestCollider.transform.position - this.transform.position;
                newForward.y = 0;
                this.transform.forward = newForward.normalized;
                currentMovement = Vector2.zero;
            }
            else
            {
                Debug.Log("noone in range");
                return;
            }
            var enemiesHit = GetAllEnemiesInCone(results);
            ApplyHit(enemiesHit);
        }
        else
        {
            Debug.Log("Cooldown");
        }
    }
    private void ApplyHit(List<Transform> enemiesHit)
    {
        foreach (var item in enemiesHit)
        {
            Debug.Log("We punched em: " + item.name);
        }
    }
    private List<Transform> GetAllEnemiesInCone(Collider[] results)
    {
        var enemiesHit = new List<Transform>();
        var maxDistance = Mathf.Sin(attackAngle * Mathf.Deg2Rad) / Mathf.Sin((180 - attackAngle) * Mathf.Deg2Rad / 2);
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
                    enemiesHit.Add(item.transform);
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
                if (closestCollider == null || (item.gameObject.layer == closestCollider.gameObject.layer))
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

    private void HandleResizeAction(float y)
    {
        if (y > 0 && characterSize < playerMetrics.PlayerMaximumSize)
        {
            characterSize++;
            sizeChanged = true;
        }
        else if (y < 0 && characterSize > playerMetrics.PlayerMinimumSize)
        {
            characterSize--;
            sizeChanged = true;
        }
    }
    #endregion
    #region fixedUpdate
    private void FixedUpdate()
    {
        ApplySize();
        ApplyMovement();
    }
    private void ApplySize()
    {
        if (sizeChanged)
        {
            var position = new Vector3(this.transform.position.x, (characterSize-1) / 2f + .3f, this.transform.position.z);
            this.transform.localScale = new Vector3(characterSize, characterSize, characterSize);
            this.transform.position = position;
            sizeChanged = false;

            currentMovement = Vector2.zero;
        }
    }
    private void ApplyMovement()
    {
        if (currentMovement != Vector2.zero)
        {
            Vector2 currentMovementScaled = currentMovement * playerMetrics.PlayerMoveSpeed[characterSize-1] * Time.fixedDeltaTime * 5f;

            Vector3 rightMovement = cameraRight * currentMovementScaled.x;
            Vector3 upMovement = cameraForward * currentMovementScaled.y;

            Vector3 heading = rightMovement + upMovement;

            if (heading != Vector3.zero)
            {
                transform.forward = heading.normalized;
                transform.position += heading;
            }
        }
    }
    #endregion
    #region update
    private void Update()
    {
        if (!readyToPunch)
        {
            timeToNextPunch -= Time.deltaTime;
            if (timeToNextPunch <= 0)
            {
                readyToPunch = true;
                ChangeModel("Idle");
            }
        }
    }
    #endregion

    private void LoadPlayerMetrices()
    {
        characterSize = playerMetrics.PlayerStartSize;
    }
    
    private void ChangeModel(string newState)
    {
        Mesh mesh;
        switch (newState)
        {
            case "Punch":
                components.modelMeshFilter.mesh = punchingModel;
                return;
            case "Idle":
                components.modelMeshFilter.mesh = idleModel;
                return;
        }
    }
}
