using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CharController : MonoBehaviour
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
    [Tooltip("minimum size of character")]
    [SerializeField] private int minimumCharacterSize;
    [Tooltip("maximum size of character")]
    [SerializeField] private int maximumCharacterSize;
    [Tooltip("current size of character; DO NOT CHANGE")]
    [SerializeField] private int characterSize;

    private Vector2 currentMovement;
    private Dictionary<Vector2, float> previousDirections;
    private float timeAcceleratingInDirection;
    private float maxAccelerationTime;
    [Header("movement speed")]
    [Tooltip("")]
    [SerializeField] private int moveSpeed;


    #endregion

    void Start()
    {
        cameraForward = cam.transform.forward; // Set forward to equal the camera's forward vector
        cameraForward.y = 0; // make sure y is 0
        cameraForward.Normalize(); // make sure the length of vector is set to a max of 1.0
        cameraRight = Quaternion.Euler(new Vector3(0, 90, 0)) * cameraForward; // set the right-facing vector to be facing right relative to the camera's forward vector

        previousDirections = new Dictionary<Vector2, float>();
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
            previousDirections.Add(currentMovement, characterSize);
            currentMovement = newCurrentMovement;
        }

        if (Input.mouseScrollDelta.y != 0) ChangeCharacterSize(Input.mouseScrollDelta.y);
    }
    private void ChangeCharacterSize(float y)
    {
        if (y > 0 && characterSize < maximumCharacterSize) characterSize++;        
        else if (characterSize > minimumCharacterSize) characterSize--;        
    }

    private void FixedUpdate()
    {
        ApplyMovement();
    }
    private void ApplyMovement()
    {
        Vector2 pastMovement = new Vector2();
        foreach (var keyValuePair in previousDirections)
        {
            Vector2 direction = keyValuePair.Key;
            float time = keyValuePair.Value;
        }

        Vector3 rightMovement = cameraRight;
        Vector3 upMovement = cameraForward;

        Vector3 heading = rightMovement + upMovement; 

        transform.forward = heading.normalized; 

        transform.position += heading;
    }

    void Move()
    {
        Vector3 direction = new Vector3(Input.GetAxis("HorizontalKey"), 0, Input.GetAxis("VerticalKey")); // setup a direction Vector based on keyboard input. GetAxis returns a value between -1.0 and 1.0. If the A key is pressed, GetAxis(HorizontalKey) will return -1.0. If D is pressed, it will return 1.0
        Vector3 rightMovement = cameraRight * maxMoveSpeed * Time.deltaTime * Input.GetAxis("HorizontalKey"); // Our right movement is based on the right vector, movement speed, and our GetAxis command. We multiply by Time.deltaTime to make the movement smooth.
        Vector3 upMovement = cameraForward * maxMoveSpeed * Time.deltaTime * Input.GetAxis("VerticalKey"); // Up movement uses the forward vector, movement speed, and the vertical axis inputs.
        Vector3 heading = Vector3.Normalize(rightMovement + upMovement); // This creates our new direction. By combining our right and forward movements and normalizing them, we create a new vector that points in the appropriate direction with a length no greater than 1.0
        transform.forward = heading; // Sets forward direction of our game object to whatever direction we're moving in
        transform.position += rightMovement; // move our transform's position right/left
        transform.position += upMovement; // Move our transform's position up/down
    }
}