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
    [Tooltip("minimum size of character; CANNOT BE ZERO")]
    [SerializeField] private int minimumCharacterSize; 
    [Tooltip("maximum size of character")]
    [SerializeField] private int maximumCharacterSize;
    [Tooltip("current size of character; DO NOT CHANGE")]
    [SerializeField] private int characterSize = 1;
    private bool sizeChanged = false;

    private Vector2 currentMovement;
    private Dictionary<Vector2, float> previousDirections;
    private float timeAcceleratingInDirection;
    [Header("acceleration time")]
    [Tooltip("maximum time counted towards acceleration")]
    [SerializeField] private float maxAccelerationTime;
    [Header("movement speed")]
    [Tooltip("base movement speed")]
    [SerializeField] private float moveSpeed;
    [Tooltip("scaling variable for pastMovements")]
    [SerializeField] private float pastMovementStrength;
    [Tooltip("scaling variable for currentMovements")]
    [SerializeField] private float currentMovementStrength;


    #endregion

    void Awake()
    {
        cameraForward = cam.transform.forward; // Set forward to equal the camera's forward vector
        cameraForward.y = 0; // make sure y is 0
        cameraForward.Normalize(); // make sure the length of vector is set to a max of 1.0
        cameraRight = Quaternion.Euler(new Vector3(0, 90, 0)) * cameraForward; // set the right-facing vector to be facing right relative to the camera's forward vector

        previousDirections = new Dictionary<Vector2, float>();
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
            if (currentMovement != Vector2.zero) AddToPreviousMovements(currentMovement, timeAcceleratingInDirection);
            currentMovement = newCurrentMovement;
            timeAcceleratingInDirection = Time.deltaTime;
        }

        if (Input.mouseScrollDelta.y != 0) ChangeCharacterSize(Input.mouseScrollDelta.y);
    }
    private void AddToPreviousMovements(Vector2 direction, float time)
    {
        float timeIfAlreadyInDic = 0;
        if (previousDirections.TryGetValue(direction, out timeIfAlreadyInDic))
        {
            time += timeIfAlreadyInDic;
            previousDirections.Remove(direction);
        }
        previousDirections.Add(direction, time);
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
            previousDirections.Clear();
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
        Vector2 pastMovementUnscaled = new Vector2();
        if (previousDirections != null && previousDirections.Count != 0)
        {
            foreach (var keyValuePair in previousDirections)
            {
                Vector2 direction = keyValuePair.Key.normalized;
                float time = keyValuePair.Value;

                pastMovementUnscaled += (direction * time);
            }
        }

        Vector2 pastMovementScaled = pastMovementUnscaled * moveSpeed * pastMovementStrength * Time.fixedDeltaTime;
        Vector2 currentMovementScaled = currentMovementUnscaled * moveSpeed * currentMovementStrength / characterSize * Time.fixedDeltaTime;
        Vector2 allMovement = pastMovementScaled + currentMovementScaled;

        Vector3 rightMovement = cameraRight * allMovement.x;
        Vector3 upMovement = cameraForward * allMovement.y;

        Vector3 heading = rightMovement + upMovement;

        if (heading != Vector3.zero)
        {
            transform.forward = heading.normalized;
            transform.position += heading;
        }

        ReduceTimeOnPreviousDirections();
    }
    private void ReduceTimeOnPreviousDirections()
    {
        Dictionary<Vector2, float> newTimeDic = new Dictionary<Vector2, float>();
        foreach (var keyValuePair in previousDirections)
        {
            Vector2 direction = keyValuePair.Key.normalized;
            float time = keyValuePair.Value - Time.fixedDeltaTime;
            ///(characterSize/2)

            if (time > 0) newTimeDic.Add(direction, time);
        }
        previousDirections = newTimeDic;
    }
}