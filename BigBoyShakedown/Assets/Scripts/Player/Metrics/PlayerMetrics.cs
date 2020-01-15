using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BigBoyShakedown.Player.Metrics
{
    [CreateAssetMenu(fileName = "New PlayerMetrics", menuName = "PlayerMetrics", order = 51)]
    public class PlayerMetrics : ScriptableObject
    {
        #region LayerMasks
        [Header("Masks")]
        [Tooltip("mask for all layers players should collide with"), SerializeField]
        private string[] mask_collidables = { "Player", "SolidEnvironment"};
        public string[] Mask_collidables { get => mask_collidables; }
        [Tooltip("mask for all layers players should be able to attack"), SerializeField]
        private string[] mask_attackables = { "Player" };
        public string[] Mask_attackables { get => mask_attackables; }
        [Tooltip("mask for all layers players should be able to interact with"), SerializeField]
        private string[] mask_interactables = { "Interactables" };
        public string[] Mask_interactables { get => mask_interactables; }
        [Tooltip("array containing layers with attack priority. these will be targetted primarily, even when objects of other layers are also in range."), SerializeField]
        private string[] priorityAttackables = { "Player" };
        public string[] PriorityAttackables { get => priorityAttackables; }
        #endregion

        #region PlayerSize
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

        #region PlayerScale&Reach
        [Header("Scale"), Tooltip("real world scale of player characters in all stages; x => height, y => width"), SerializeField]
        private Vector2[] playerScale = { new Vector2(1f, .5f), new Vector2(1f, .5f), new Vector2(1f, .5f), new Vector2(1f, .5f), new Vector2(1f, .5f) };
        public Vector2[] PlayerScale { get => playerScale; }

        [[Header("Range")]
        [Tooltip("punch range player has in all stages"), SerializeField]
        //private static float[] playerPunchRange = { 1.75f, 1.8f, 2.7f, 3.6f, 4.5f };
        private float[] playerPunchRange = { 10f, 10f, 10f, 10f, 10f };
        public float[] PlayerPunchRange { get => playerPunchRange; }

        [Tooltip("angle of the cone in which the punch is applied"), SerializeField]
        private float[] playerPunchAngle = { 60f, 60f, 60f, 60f, 60f };
        public float[] PlayerPunchAngle { get => playerPunchAngle; }

        [Tooltip("interaction player has in all stages"), SerializeField]
        //private static float[] playerPunchRange = { 1.75f, 1.8f, 2.7f, 3.6f, 4.5f };
        private float[] playerInteractionRange = { 10f, 10f, 10f, 10f, 10f };
        public float[] PlayerInteractionRange { get => playerInteractionRange; }
        #endregion

        #region score
        [Header("Score Stages"), Tooltip("monetary value associated with each plasyerSize"), SerializeField]
        private int[] playerScore = { 0, 500, 900, 1500, 2000, 3500 };
        public int[] PlayerScore { get => playerScore; }

        [Header("Start Score"), Tooltip("monetary value player starts on when joining the game"), SerializeField]
        private int playerStartScore = 600;
        public int PlayerStartScore { get => playerStartScore; }

        [Header("Min Score"), Tooltip("0 point of score coordinate system: time on x, score on y"), SerializeField]
        private Vector2 playerMinScore = new Vector2(0, 0);
        public Vector2 PlayerMinScore { get => playerMinScore; }

        [Header("Max Score"), Tooltip("monetary value player starts on when joining the game"), SerializeField]
        private Vector2 playerMaxScore = new Vector2(0, 0);
        public Vector2 PlayerMaxScore { get => playerMaxScore; }

        [Header("Change Animation Rate"), Tooltip("rate at which graphs change"), SerializeField]
        private float changeAnimationRate = 5f;
        public float ChangeAnimationRate { get => changeAnimationRate; }
        #endregion


        #region movement


        [Header("MovementSpeed"), Tooltip("movement speed in all stages"), SerializeField]
        private float[] playerMoveSpeed = { 1.25f, 1.0f, .9f, .75f, .5f };
        public float[] PlayerMoveSpeed { get => playerMoveSpeed; }

        [Header("VerticalMovementSpeed"), Tooltip("movement speed in all stages"), SerializeField]
        private float[] playerYMoveSpeed = { 1.25f, 1.0f, .9f, .75f, .5f };
        public float[] PlayerYMoveSpeed { get => playerMoveSpeed; }
        #endregion


        #region combat
        [Header("AttackTimings")]
        [Tooltip("wind up time for player attacks in all stages"), SerializeField]
        private float[] playerWindUpTime = { 0.6f, .75f, 1.0f, 1.75f, 2.25f };
        public float[] PlayerWindUpTime { get => playerWindUpTime; }

        [Tooltip("recovery time for player attacks in all stages"), SerializeField]
        private float[] playerPunchRecoveryTime = { 0.6f, .75f, 1.0f, 1.75f, 2.25f };
        public float[] PlayerPunchRecoveryTime { get => playerPunchRecoveryTime; }



        [Header("Damage"), Tooltip("damage player does in all stages"), SerializeField]
        private int[] playerDamage = { 40, 50, 80, 120, 200 };
        public int[] PlayerDamage { get => playerDamage; }

        [Header("Range"), Tooltip("range player has in all stages"), SerializeField]
        private float[] playerComboModifier = { 10f, 10f, 10f, 10f, 10f };
        public float[] PlayerComboModifier { get => playerComboModifier; }
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
        public Material DefaultRingMaterial { get => defaultRingMaterial; }
        #endregion

    }
}   
