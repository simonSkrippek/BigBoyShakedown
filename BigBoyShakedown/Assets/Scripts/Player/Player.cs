using System;
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
    private bool invulnerable = false;
    public VariableReference<bool> Invulnerable;

    private bool stunned = true;
    public VariableReference<bool> Stunned;

    #region punch
    private bool readyToPunch = true;
    public VariableReference<bool> ReadyToPunch;

    private bool punchTriggered = false;
    public VariableReference<bool> PunchTriggered;
    #endregion

    #region interaction
    private bool readyToInteract = true;
    public VariableReference<bool> ReadyToInteract;

    private bool interactionTriggered = false;
    public VariableReference<bool> InteractionTriggered;
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
    public VariableReference<bool> MovementAllowed;

    private bool rotationAllowed = true;
    public VariableReference<bool> RotationAllowed;
    #endregion
    #endregion

    [Header("Animator"), Tooltip("Player Animator on the same object"),SerializeField]
    private Animator animator;
    [Header("Animator"), Tooltip("Player Animator on the same object"), SerializeField]
    private PlayerMetrics playerMetrics;

    private void Awake()
    {
        gameTime = FindObjectOfType<Time>();

        Invulnerable = new VariableReference<bool>(() => invulnerable, (val) => { invulnerable = val; });

        Stunned = new VariableReference<bool>(() => stunned, (val) => { stunned = val; });

        ReadyToPunch = new VariableReference<bool>(() => readyToPunch, (val) => { readyToPunch = val; });
        PunchTriggered = new VariableReference<bool>(() => punchTriggered, (val) => { punchTriggered = val; });
        
        ReadyToInteract = new VariableReference<bool>(() => readyToInteract, (val) => { readyToInteract = val; });
        InteractionTriggered = new VariableReference<bool>(() => interactionTriggered, (val) => { interactionTriggered = val; });

        MovementAllowed = new VariableReference<bool>(() => movementAllowed, (val) => { movementAllowed = val; });
        RotationAllowed = new VariableReference<bool>(() => rotationAllowed, (val) => { rotationAllowed = val; });
    }
    void Start()
    {
        bool test = false;
        Time.StartTimer(new VariableReference<bool>(() => test, val => { test = val; }), 5f);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.IsRunning)
        {
            
        }
    }

    public void StartPunch()
    {
        var punchSpeed = playerMetrics.PlayerPunchSpeed[this.CurrentCharacterSize - 1];
        this.ReadyToPunch.Set(false);
        Time.StartTimer(this.ReadyToPunch.SetEndValue(true), punchSpeed);
        this.ReadyToInteract.Set(false);
        Time.StartTimer(this.ReadyToInteract.SetEndValue(true), punchSpeed);
        this.MovementAllowed.Set(false);
        Time.StartTimer(this.MovementAllowed.SetEndValue(true), punchSpeed);
        this.RotationAllowed.Set(false);
        Time.StartTimer(this.RotationAllowed.SetEndValue(true), punchSpeed);
    }
}
