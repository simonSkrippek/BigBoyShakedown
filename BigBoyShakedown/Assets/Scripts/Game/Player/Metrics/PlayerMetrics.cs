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
        private string[] mask_collidables = { "Player", "SolidEnvironment" };
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

        //#region PlayerSize
        //[Header("Sizes"), Tooltip("minimum size player can have"), SerializeField]
        //[Min(1)]
        //private int playerMinimumSize = 1;
        //public int PlayerMinimumSize { get => playerMinimumSize; }

        //[Tooltip("maximum size player can have"), SerializeField]
        //private int playerMaximumSize = 5;
        //public int PlayerMaximumSize { get => playerMaximumSize; }
        //#endregion

        #region PlayerScale&Reach
        [Header("Scale"), Tooltip("real world scale of player characters in all stages; x => height, y => width"), SerializeField]
        private Vector2[] playerScale = { new Vector2(1f, .5f), new Vector2(1f, .5f), new Vector2(1f, .5f), new Vector2(1f, .5f), new Vector2(1f, .5f) };
        public Vector2[] PlayerScale { get => playerScale; }

        [Header("Range")]
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

        #region combat        
        [Header("Punch"), Tooltip("damage player does in all stages, on different combo hits (first param is size, second is combo number)"), SerializeField]
        private int[,] playerDamage = { { 40, 50, 80},
                                        { 40, 50, 80},
                                        { 40, 50, 80},
                                        { 40, 50, 80},
                                        { 40, 50, 80} };
        public int[,] PlayerDamage { get => playerDamage; }

        [Tooltip("knockback player does in all stages, on different combo hits (first param is size, second is combo number)"), SerializeField]
        private float[,] playerPunchKnockback = { { 1f, 1f, 1f},
                                        { 1f, 1f, 1f},
                                        { 1f, 1f, 1f},
                                        { 1f, 1f, 1f},
                                        { 1f, 1f, 1f}};
        public float[,] PlayerPunchKnockback { get => playerPunchKnockback; }

        [Tooltip("stun player does in all stages, on different combo hits (first param is size, second is combo number)"), SerializeField]
        private float[,] playerPunchStunDuration = { { 1f, 1f, 1f},
                                        { 1f, 1f, 1f},
                                        { 1f, 1f, 1f},
                                        { 1f, 1f, 1f},
                                        { 1f, 1f, 1f}};
        public float[,] PlayerPunchStunDuration { get => playerPunchStunDuration; }






        #region ZIADEDITTHISPLEASE
        //punch movement / animation length shit, hope u know how to edit

        [Tooltip("complete length of movement (in unity-meters)"), SerializeField]
        private float[,] playerPunchForwardMovementDistance = { { 2f, 2f, 2f},
                                        { 2f, 2f, 2f},
                                        { 2f, 2f, 3f},
                                        { 2f, 2f, 3f},
                                        { 2f, 2f, 3f}};
        public float[,] PlayerPunchForwardMovementDistance { get => playerPunchForwardMovementDistance; }

        [Tooltip("complete time of punch (in seconds)"), SerializeField]
        private float[,] playerPunchAnimationDurationIntended = { { 0.75f, 0.75f, 1f},
                                        { 0.75f, 0.75f, 1f},
                                        { 0.85f, 0.85f, 1.1f},
                                        { 1.2f, 1.2f, 1.5f},
                                        { 1.7f, 1.7f, 2f}};
        public float[,] PlayerPunchAnimationDurationIntended { get => playerPunchAnimationDurationIntended; }

        [Tooltip("point in animation at that movement starts (in percent)"), SerializeField]
        private float[,] playerPunchMovementStartPoint = { { .34f, .34f, .3f},
                                        { .34f, .4f, .3f},
                                        { .5f, .5f, .35f},
                                        { .5f, .5f, .35f},
                                        { .5f, .5f, .35f}};
        public float[,] PlayerPunchMovementStartPoint { get => playerPunchMovementStartPoint; }


        #endregion





        [Tooltip("fixed length of all punch animations"), SerializeField]
        float[] playerAnimationFixedDuration = { 12.458f, 12.458f, 9.967f, 9.967f, 9.967f };
        public float[] PlayerPunchAnimationFixedDuration { get => playerAnimationFixedDuration; }
        #endregion

        #region score
        [Header("Score"), Tooltip("monetary value associated with each plasyerSize"), SerializeField]
        private int[] playerScore = { 0, 500, 900, 1500, 2000, 3500 };
        public int[] PlayerScore { get => playerScore; }

        [Tooltip("monetary value player starts on when joining the game"), SerializeField]
        private int playerStartScore = 600;
        public int PlayerStartScore { get => playerStartScore; }

        [Tooltip("0 point of score coordinate system: time on x, score on y"), SerializeField]
        private Vector2 playerMinScore = new Vector2(0, 0);
        public Vector2 PlayerMinScore { get => playerMinScore; }

        [Tooltip("monetary value player starts on when joining the game"), SerializeField]
        private Vector2 playerMaxScore = new Vector2(0, 0);
        public Vector2 PlayerMaxScore { get => playerMaxScore; }

        [Tooltip("rate at which graphs change"), SerializeField]
        private float changeAnimationRate = 5f;
        public float ChangeAnimationRate { get => changeAnimationRate; }
        #endregion

        #region movement
        [Header("Movement"), Tooltip("movement speed in all stages"), SerializeField]
        private float[] playerMoveSpeed = { 1.25f, 1.0f, .9f, .75f, .5f };
        public float[] PlayerMoveSpeed { get => playerMoveSpeed; }

        [Tooltip("movement speed in all stages"), SerializeField]
        private float[] playerYMoveSpeed = { 1.25f, 1.0f, .9f, .75f, .5f };
        public float[] PlayerYMoveSpeed { get => playerYMoveSpeed; }

        [Header("Dash"), Tooltip("dash speed in all stages"), SerializeField]
        private float[] playerDashSpeed = { 1.25f, 1.0f, .9f, .75f, .5f };
        public float[] PlayerDashSpeed { get => playerDashSpeed; }

        [Tooltip("dash duration in all stages"), SerializeField]
        private float[] playerDashDuration = { 1.25f, 1.0f, .9f, .75f, .5f };
        public float[] PlayerDashDuration { get => playerDashDuration; }
        #endregion
    }
}
