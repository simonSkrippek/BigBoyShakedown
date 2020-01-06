using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class Player : MonoBehaviour
{
    Time gameTime;

    public int playerIndex;

    #region size
    private int currentCharacterSize = 1;
    public int CurrentCharacterSize
    {
        get => currentCharacterSize;
        set => currentCharacterSize = value;            
    }
    #endregion

    #region score
    private float currentCharacterScore = 0;
    public float CurrentCharacterScore
    {
        get => currentCharacterScore;
        set => currentCharacterScore = value;
    }
    #endregion

    #region actions
    private bool unstunned = true;
    public bool Unstunned
    {
        get => unstunned;
        private set => unstunned = value;
    }

    #region punch
    private bool readyToPunch = true;
    public bool ReadyToPunch
    {
        get => readyToPunch;
        private set => readyToPunch = value;
    }

    private bool punchTriggered = false;
    public bool PunchTriggered
    {
        get => punchTriggered;
        set => punchTriggered = value;
    }
    #endregion

    #region interaction
    private bool readyToInteract = true;
    public bool ReadyToInteract
    {
        get => readyToInteract;
        private set => readyToInteract = value;
    }

    private bool interactionTriggered = false;
    public bool InteractionTriggered
    {
        get => interactionTriggered;
        set => interactionTriggered = value;
    }
    #endregion

    #region movement
    private Vector2 currentMovement;
    public Vector2 CurrentMovement
    {
        get => currentMovement;
        set
        {
            animator.SetFloat("currentMovementX", value.x);
            animator.SetFloat("currentMovementY", value.y);
            currentMovement = value;
        }
    }

    private bool movementAllowed = true;
    public bool MovementAllowed
    {
        get => movementAllowed;
        private set => movementAllowed = value;
    }

    private bool rotationAllowed = true;
    public bool RotationAllowed
    {
        get => rotationAllowed;
        private set => rotationAllowed = value;
    }
    #endregion
    #endregion

    [Header("Animator"), Tooltip("Player Animator on the same object"),SerializeField]
    private Animator animator;

    
    private void Awake()
    {
        gameTime = FindObjectOfType<Time>();
    }
    void Start()
    {
        bool test = false;
        gameTime.StartTimer(new VariableReference<bool>(() => test, val => { test = val; }), 5f);
    }

    // Update is called once per frame
    void Update()
    {
        if (gameTime.IsRunning)
        {
            
        }
    }
}
