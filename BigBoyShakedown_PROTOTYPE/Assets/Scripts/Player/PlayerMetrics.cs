using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerMetrics
{
    private static int playerMinimumSize = 1;
    public static int PlayerMinimumSize { get => playerMinimumSize; }


    private static int playerMaximumSize = 5;
    public static int PlayerMaximumSize { get => playerMaximumSize; }

    private static int playerStartSize = 2;
    public static int PlayerStartSize { get => playerStartSize; }


    private static float[] playerScale = { 0.75f, 1.0f, 1.5f, 2.0f, 2.5f };
    public static float[] PlayerScale { get => playerScale; }


    private static float[] playerMoveSpeed = { 1.25f, 1.0f, .9f, .75f, .5f };
    public static float[] PlayerMoveSpeed { get => playerMoveSpeed; }


    private static float[] playerPunchSpeed = { 0.6f, .75f, 1.0f, 1.75f, 2.25f };
    public static float[] PlayerPunchSpeed { get => playerPunchSpeed; }


    //private static float[] playerPunchRange = { 1.75f, 1.8f, 2.7f, 3.6f, 4.5f };
    private static float[] playerPunchRange = { 10f, 10f, 10f, 10f, 10f };
    public static float[] PlayerPunchRange { get => playerPunchRange; }

    
    private static float[] playerDamage = { 40, 50, 80, 120, 200 };
    public static float[] PlayerDamage { get => playerDamage; }
}
