using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CharController1 : MonoBehaviour
{
    #region components
    [Header("Rigidbody")][Tooltip("players rigidbodyComponent; REQUIRED")]
    [SerializeField] private Rigidbody charRigidbody;
    #endregion

    #region camera
    [Header("Camera")][Tooltip("main camera object; REQUIRED")]
    [SerializeField] private Camera cam;
    private Vector3 cameraForward, cameraRight; // Keeps track of our relative forward and right vectors
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
    private float timeAcceleratingInDirection;
    [Header("acceleration time")]
    [Tooltip("maximum time counted towards acceleration")]
    [SerializeField] private float maxAccelerationTime;
    [Header("movement speed")]
    [Tooltip("base movement speed")]
    [SerializeField] private float moveSpeed;


    #endregion

    void Awake()
    {
        cameraForward = cam.transform.forward; // Set forward to equal the camera's forward vector
        cameraForward.y = 0; // make sure y is 0
        cameraForward.Normalize(); // make sure the length of vector is set to a max of 1.0
        cameraRight = Quaternion.Euler(new Vector3(0, 90, 0)) * cameraForward; // set the right-facing vector to be facing right relative to the camera's forward vector

        characterSize = 1;
    }


    void Update()
    {
        ProcessInput();
    }
    private void ProcessInput()
    {
        Vector2 newCurrentMovement = new Vector2(Input.GetAxis("HorizontalKey"), Input.GetAxis("VerticalKey")).normalized;

        if (newCurrentMovement == currentMovement)
        {
            timeAcceleratingInDirection += Time.deltaTime;
            if (timeAcceleratingInDirection > maxAccelerationTime) timeAcceleratingInDirection = maxAccelerationTime;
        }
        else
        {
            currentMovement = newCurrentMovement;
            timeAcceleratingInDirection = timeAcceleratingInDirection / characterSize;
        }

        if (Input.mouseScrollDelta.y != 0) ChangeCharacterSize(Input.mouseScrollDelta.y);
    }
    private void ChangeCharacterSize(float y)
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

            maxAccelerationTime = characterSize;
            currentMovement = Vector2.zero;
            timeAcceleratingInDirection = 0f;
        }
    }
    private void ApplyMovement()
    {
        Vector2 currentMovementUnscaled = new Vector2();
        if (currentMovement != Vector2.zero)
        {
            currentMovementUnscaled = currentMovement * timeAcceleratingInDirection;
        }
        Vector2 currentMovementScaled = currentMovementUnscaled * moveSpeed / characterSize * Time.fixedDeltaTime;

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