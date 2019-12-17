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
    [Tooltip("minimum size of character; CANNOT BE ZERO")]
    [SerializeField] private int minimumCharacterSize;
    [Tooltip("maximum size of character")]
    [SerializeField] private int maximumCharacterSize;
    [Tooltip("current size of character; DO NOT CHANGE")]
    [SerializeField] private int characterSize = 1;
    private bool sizeChanged = false;

    private Vector2 currentMovement;
    [Header("movement speed")]
    [Tooltip("base movement speed")]
    [SerializeField] private float moveSpeed;
    #endregion

    #region combat
    [Header("Combat"), Tooltip("base attack range of the character"), SerializeField]
    private float range = 1.5f;
    [Tooltip("base attack speed"), SerializeField]
    private float timeBetweenPunches = 1f;
    [Tooltip("attack angle for normal punches"), SerializeField]
    float attackAngle = 60;
    private float timeToNextPunch = 1.5f;

    #endregion

    #region components
    private PlayerInput playerInput;
    #endregion

    private void Awake()
    {
        cam = Camera.main;

        cameraForward = cam.transform.forward; // Set forward to equal the camera's forward vector
        cameraForward.y = 0; // make sure y is 0
        cameraForward.Normalize(); // make sure the length of vector is set to a max of 1.0
        cameraRight = Quaternion.Euler(new Vector3(0, 90, 0)) * cameraForward; // set the right-facing vector to be facing right relative to the camera's forward vector

        characterSize = 1;

        playerInput = GetComponent<PlayerInput>();
        playerInput.onActionTriggered += HandleInputAction;
    }

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
    }
    private void HandleMoveAction(Vector2 movement)
    {
        currentMovement = movement;
    }
    private void HandlePunchAction()
    {
        Debug.Log("punch");
        Collider[] results = Physics.OverlapSphere(this.transform.position, range, LayerMask.GetMask(new string[] {"test" }));

        Collider closestCollider = null;
        float distanceToClosestCollider = float.MaxValue;
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

                if (distanceToCollider < distanceToClosestCollider)
                {
                    distanceToClosestCollider = distanceToCollider;
                    closestCollider = item;
                }
            }
        }

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
        }
        
        var maxDistance = Mathf.Sin(attackAngle * Mathf.Deg2Rad) / Mathf.Sin((180 - attackAngle) * Mathf.Deg2Rad / 2);
        //Debug.Log("maximum distance: " + maxDistance);
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
                //Debug.Log("distance to collider: " + distanceToCollider);

                if (distanceToCollider < maxDistance)
                {
                    Debug.Log("We punched em: " + item.name);
                }
            }
        }

    }
    private void HandleResizeAction(float y)
    {
        if (y > 0 && characterSize < maximumCharacterSize)
        {
            characterSize++;
            sizeChanged = true;
        }
        else if (y < 0 && characterSize > minimumCharacterSize)
        {
            characterSize--;
            sizeChanged = true;
        }
    }

    private void FixedUpdate()
    {
        ApplySize();
        ApplyMovement();
    }
    private void ApplySize()
    {
        if (sizeChanged)
        {
            var position = new Vector3(this.transform.position.x, characterSize / 2f + .3f, this.transform.position.z);
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
            Vector2 currentMovementScaled = currentMovement * (moveSpeed - characterSize) * Time.fixedDeltaTime;

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

}
