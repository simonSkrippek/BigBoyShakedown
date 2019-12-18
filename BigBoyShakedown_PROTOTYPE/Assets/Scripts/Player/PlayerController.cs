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
    public int CharacterSize { get => characterSize; set => characterSize = value; }
    private bool sizeChanged = false;
    public bool SizeChanged { get => sizeChanged; set => sizeChanged = value; }

    private Vector2 currentMovement;
    #endregion

    #region combat
    [Header("Combat"), Tooltip("attack angle for normal punches"), SerializeField]
    float attackAngle = 60;
    private float timeToNextPunch = 1.5f;
    private bool readyToPunch = true;

    #endregion

    #region score
    private int score;
    #endregion

    #region components
    private PlayerComponents components;
    [Header("Player Metrics"), Tooltip("default player metrics; REQUIRED"), SerializeField]
    private PlayerMetrics playerMetrics;

    private int playerIndex;
    #endregion

    #region models
    [Header("Models"), Tooltip("character models, mapped to stances \n REQUIRED: Punch, Idle"), SerializeField]
    private Mesh punchingModel, idleModel;
    #endregion

    #region events
    public event Action<int, int> scoreChangedEvent;
    #endregion

    private void Awake()
    {
        components = this.transform.root.GetComponent<PlayerComponents>();

        cam = Camera.main;

        cameraForward = cam.transform.forward; // Set forward to equal the camera's forward vector
        cameraForward.y = 0; // make sure y is 0
        cameraForward.Normalize(); // make sure the length of vector is set to a max of 1.0
        cameraRight = Quaternion.Euler(new Vector3(0, 90, 0)) * cameraForward; // set the right-facing vector to be facing right relative to the camera's forward vector

        LoadPlayerMetrics();

        components.playerInput.onActionTriggered += HandleInputAction;
        playerIndex = components.playerInput.playerIndex;
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
            timeToNextPunch = playerMetrics.PlayerPunchSpeed[CharacterSize-1];
            ChangeModel("Punch");

            Collider[] results = Physics.OverlapSphere(this.transform.position, playerMetrics.PlayerPunchRange[CharacterSize-1], LayerMask.GetMask(new string[] { "Player", "test" }));
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
    private void ApplyHit(List<Transform> enemiesHit)
    {
        foreach (var item in enemiesHit)
        {
            if (item.CompareTag("Player"))
            {
                var components = item.GetComponent<PlayerComponents>();
                components.playerController.TakeDamage(this.transform, characterSize);
                var scoreChange = playerMetrics.PlayerDamage[characterSize - 1];
                this.score += scoreChange;
                scoreChangedEvent?.Invoke(playerIndex, scoreChange);
            }
            Debug.Log("We punched em: " + item.name);
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
        if (SizeChanged)
        {
            var position = new Vector3(this.transform.position.x, CharacterSize / 2f + .3f, this.transform.position.z);
            this.transform.localScale = new Vector3(CharacterSize, CharacterSize, CharacterSize);
            this.transform.position = position;
            SizeChanged = false;

            currentMovement = Vector2.zero;
        }
    }
    private void ApplyMovement()
    {
        if (currentMovement != Vector2.zero)
        {
            Vector2 currentMovementScaled = currentMovement * playerMetrics.PlayerMoveSpeed[CharacterSize-1] * Time.fixedDeltaTime * 5f;

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
        CheckSize();
    }
    private void CheckSize()
    {
        if (score >= playerMetrics.PlayerScore[characterSize - 1])
        {
            characterSize -= 1;
            SizeChanged = true;
        }
        else if (score > playerMetrics.PlayerScore[characterSize])
        {
            characterSize += 1;
            SizeChanged = true;
        }
    }
    #endregion

    private void LoadPlayerMetrics()
    {
        CharacterSize = playerMetrics.PlayerStartSize;
        score = playerMetrics.PlayerStartScore;
        scoreChangedEvent?.Invoke(playerIndex, score);
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

    public void TakeDamage(Transform origin, int originSize)
    {
        this.transform.Translate(origin.forward);
        var scoreChange = -playerMetrics.PlayerDamage[originSize - 1];
        this.score += scoreChange;
        scoreChangedEvent?.Invoke(playerIndex, scoreChange*100);
        if (score < 0) Die();
    }

    private void Die()
    {
        Debug.Log("Player dies");
    }
}
