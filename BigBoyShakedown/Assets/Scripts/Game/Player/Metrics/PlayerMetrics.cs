using UnityEngine;

namespace BigBoyShakedown.Player.Metrics
{
    [CreateAssetMenu(fileName = "New PlayerMetrics", menuName = "PlayerMetrics", order = 51)]
    public class PlayerMetrics : ScriptableObject
    {
        #region LayerMasks
        [Header("Masks")]
        [Tooltip("mask for all layers players should collide with")]
        private string[] mask_collidables = { "Player", "SolidEnvironment", "Interactables", "Attackables" };
        public string[] Mask_collidables { get => mask_collidables; }
        [Tooltip("mask for all layers players should be able to attack")]
        private string[] mask_attackables = { "Player", "Attackables" };
        public string[] Mask_attackables { get => mask_attackables; }
        [Tooltip("mask for all layers players should be able to interact with")]
        private string[] mask_interactables = { "Interactables" };
        public string[] Mask_interactables { get => mask_interactables; }
        [Tooltip("array containing layers with attack priority. these will be targetted primarily, even when objects of other layers are also in range.")]
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
        [Header("Scale"), Tooltip("real world scale of player characters in all stages; x => height, y => width")]
        private Vector2[] playerScale = { new Vector2(1.435583f, 0.6490859f), new Vector2(1.958734f, .79887f), new Vector2(2.419108f, 1.16518f), new Vector2(3.583863f, 1.819678f), new Vector2(5.054846f, 3.025215f) };
        public Vector2[] PlayerScale { get => playerScale; }

        [Header("Range")]
        [Tooltip("punch range player has in all stages")]
        //private static float[] playerPunchRange = { 1.75f, 1.8f, 2.7f, 3.6f, 4.5f };
        private float[] playerPunchRange = { 2f, 3f, 4f, 5f, 5.5f };
        public float[] PlayerPunchRange { get => playerPunchRange; }

        [Tooltip("angle of the cone in which the punch is applied")]
        private float[] playerPunchAngle = { 60f, 60f, 60f, 60f, 60f };
        public float[] PlayerPunchAngle { get => playerPunchAngle; }

        [Tooltip("interaction player has in all stages")]
        //private static float[] playerPunchRange = { 1.75f, 1.8f, 2.7f, 3.6f, 4.5f };
        private float[] playerInteractionRange = { 2f, 3f, 4f, 5f, 5.5f };
        public float[] PlayerInteractionRange { get => playerInteractionRange; }
        #endregion

        #region combat        
        [Header("Punch"), Tooltip("damage player does in all stages, on different combo hits (first param is size, second is combo number)")]
        private int[,] playerDamage = { { 30, 40, 70},
                                        { 40, 50, 80},
                                        { 45, 55, 85},
                                        { 70, 80, 90},
                                        { 80, 100, 120} };
        public int[,] PlayerDamage { get => playerDamage; }

        [Tooltip("knockback player does in all stages, on different combo hits (first param is size, second is combo number)")]
        private float[,] playerPunchKnockback = { { 1f, 1f, 2f},
                                        { 1f, 1f, 2.5f},
                                        { 1f, 1f, 3f},
                                        { 1f, 1f, 4f},
                                        { 1f, 1f, 5f}};
        public float[,] PlayerPunchKnockback { get => playerPunchKnockback; }

        [Tooltip("stun player does in all stages, on different combo hits (first param is size, second is combo number)")]
        private float[,] playerPunchStunDuration = { { .3f, .3f, .6f},
                                        { .4f, .4f, .8f},
                                        { .6f, .6f, 1f},
                                        { .9f, .9f, 1.1f},
                                        { 1f, 1f, 1.5f}};
        public float[,] PlayerPunchStunDuration { get => playerPunchStunDuration; }






        #region ZIADEDITTHISPLEASE
        //punch movement / animation length shit, hope u know how to edit

        [Tooltip("complete length of movement (in unity-meters)")]
        private float[,] playerPunchForwardMovementDistance = { { 2f, 2f, 2f},
                                        { 2f, 2f, 2f},
                                        { 2f, 2f, 3f},
                                        { 2f, 2f, 3f},
                                        { 2.5f, 2.5f, 3.5f}};
        public float[,] PlayerPunchForwardMovementDistance { get => playerPunchForwardMovementDistance; }

        [Tooltip("complete time of punch (in seconds)")]
        private float[,] playerPunchAnimationDurationIntended = { { 0.75f, 0.75f, 1f},
                                        { 0.75f, 0.75f, 1f},
                                        { 0.95f, 0.95f, 1.3f},
                                        { 1.6f, 1.6f, 1.9f},
                                        { 2.3f, 2.3f, 2.6f}};
        public float[,] PlayerPunchAnimationDurationIntended { get => playerPunchAnimationDurationIntended; }

        [Tooltip("point in animation at that movement starts (in percent)")]
        private float[,] playerPunchMovementStartPoint = { { .34f, .34f, .3f},
                                        { .34f, .4f, .3f},
                                        { .5f, .5f, .35f},
                                        { .5f, .5f, .35f},
                                        { .5f, .5f, .35f}};
        public float[,] PlayerPunchMovementStartPoint { get => playerPunchMovementStartPoint; }


        #endregion





        [Tooltip("fixed length of all punch animations")]
        float[] playerAnimationFixedDuration = { 12.458f, 12.458f, 9.967f, 9.967f, 9.967f };
        public float[] PlayerPunchAnimationFixedDuration { get => playerAnimationFixedDuration; }
        #endregion




        #region score
        [Header("Score"), Tooltip("monetary value associated with each plasyerSize")]
        private int[] playerScore = { 0, 1000, 2000, 3000, 4000, 5000 };
        public int[] PlayerScore { get => playerScore; }

        [Tooltip("monetary value player starts on when joining the game")]
        private int playerStartScore = 1000;
        public int PlayerStartScore { get => playerStartScore; }

        [Tooltip("0 point of score coordinate system: time on x, score on y")]
        private Vector2 playerMinScore = new Vector2(0, 0);
        public Vector2 PlayerMinScore { get => playerMinScore; }

        [Tooltip("monetary value player starts on when joining the game")]
        private Vector2 playerMaxScore = new Vector2(240, 5000);
        public Vector2 PlayerMaxScore { get => playerMaxScore; }

        [Tooltip("rate at which graphs change")]
        private float changeAnimationRate = 5f;
        public float ChangeAnimationRate { get => changeAnimationRate; }
        #endregion


        #region movement
        [Header("Movement"), Tooltip("movement speed in all stages")]
        private float[] playerMoveSpeed = { 1.25f, 1.0f, .9f, .75f, .4f };
        public float[] PlayerMoveSpeed { get => playerMoveSpeed; }

        [Tooltip("movement speed in all stages")]
        private float[] playerYMoveSpeed = { 1.25f, 1.0f, .9f, .75f, .4f };
        public float[] PlayerYMoveSpeed { get => playerYMoveSpeed; }

        [Header("Dash"), Tooltip("dash speed in all stages")]
        private float[] playerDashSpeed = { 1.25f, 1.0f, .9f, .75f, .5f };
        public float[] PlayerDashSpeed { get => playerDashSpeed; }

        [Tooltip("dash duration in all stages")]
        private float[] playerDashDuration = { 1.25f, 1.0f, .9f, .75f, .5f };
        public float[] PlayerDashDuration { get => playerDashDuration; }
        #endregion
    }
}
