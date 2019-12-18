using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New PlayerMetrics", menuName = "PlayerMetrics", order = 51)]
public class PlayerMetrics : ScriptableObject
{
    #region sizes

    [Header("MinimumSize"), Tooltip("minimum size player can have"), SerializeField]
    [Min(1)]
    private int playerMinimumSize = 1;
    public int PlayerMinimumSize { get => playerMinimumSize; }
    
    [Header("MaximumSize"), Tooltip("maximum size player can have"), SerializeField]
    private int playerMaximumSize = 5;
    public int PlayerMaximumSize { get => playerMaximumSize; }

    [Header("StartSize"), Tooltip("size player starts on when joining the game"), SerializeField]
    private int playerStartSize = 2;
    public int PlayerStartSize { get => playerStartSize; }
    #endregion

    [Space()]

    #region score

    [Header("Score Stages"), Tooltip("monetary value associated with each plasyerSize"), SerializeField]
    private int[] playerScore = { 0, 500, 900, 1500, 2000, 3500 };
    public int[] PlayerScore { get => playerScore; }

    [Header("Start Score"), Tooltip("monetary value player starts on when joining the game"), SerializeField]
    private int playerStartScore = 600;
    public int PlayerStartScore { get => playerStartScore; }
    #endregion

    [Space()]

    #region stats
    #region movement
    [Header("Scale"), Tooltip("real world scale of player characters in all stages"), SerializeField]
    private float[] playerScale = { 0.75f, 1.0f, 1.5f, 2.0f, 2.5f };
    public float[] PlayerScale { get => playerScale; }


    [Header("MovementSpeed"), Tooltip("movement speed in all stages"), SerializeField]
    private float[] playerMoveSpeed = { 1.25f, 1.0f, .9f, .75f, .5f };
    public float[] PlayerMoveSpeed { get => playerMoveSpeed; }
    #endregion
    [Space()]
    #region combat
    [Header("AttackSpeed"), Tooltip("time between player attacks in all stages"), SerializeField]
    private float[] playerPunchSpeed = { 0.6f, .75f, 1.0f, 1.75f, 2.25f };
    public float[] PlayerPunchSpeed { get => playerPunchSpeed; }


    [Header("Range"), Tooltip("range player has in all stages"), SerializeField]
    //private static float[] playerPunchRange = { 1.75f, 1.8f, 2.7f, 3.6f, 4.5f };
    private float[] playerPunchRange = { 10f, 10f, 10f, 10f, 10f };
    public float[] PlayerPunchRange { get => playerPunchRange; }


    [Header("Damage"), Tooltip("damage player does in all stages"), SerializeField]
    private int[] playerDamage = { 40, 50, 80, 120, 200 };
    public int[] PlayerDamage { get => playerDamage; }
    #endregion
    #endregion

    [Space()]

    #region appearance
    [Header("player colors")]
    [Tooltip("array of colors that will be assigned to players, in order of joining"), SerializeField]
    private Material[] playerColors;
    public Material[] PlayerColors { get => playerColors; }
    [Tooltip("default material that will be assigned if not enough materials in playerColors"), SerializeField]
    private Material defaultMaterial;
    public Material DefaultMaterial { get => defaultMaterial; }


    [Header("player rings")]
    [Tooltip("array of colors that will be assigned to player rings, in order of joining"), SerializeField]
    private Material[] playerRings;
    public Material[] PlayerRings { get => playerRings; }
    [Tooltip("default material that will be assigned if not enough materials in playerRings"), SerializeField]
    private Material defaultRingMaterial;
    public Material DefaultRingMaterial { get => defaultRingMaterial;}
    #endregion
    
    [Space()]

    #region testingONLY
    [Header("Test Meshes"), Tooltip("meshes that get awapped in; \nTEST; INSTEAD OF ANIMATIONS"), SerializeField]
    private Mesh testMeshIdle, testMeshPunching;
    public Mesh TestMeshIdle { get => testMeshIdle; }
    public Mesh TestMeshPunching { get => testMeshPunching; }
    #endregion
}
