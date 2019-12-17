using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerMetrices
{
    private static float[] playerScale = { 0.75f, 1.0f, 1.5f, 2.0f, 2.5f };
    public static float[] PlayerScale { get => playerScale; }


    private static float[] playerMoveSpeed = { 1.25f, 1.0f, .9f, .75f, .5f };
    public static float[] PlayerMoveSpeed { get => playerMoveSpeed; }


    private static float[] playerPunchSpeed = { 0.6f, .75f, 1.0f, 1.75f, 2.25f };
    public static float[] PlayerPunchSpeed { get => playerPunchSpeed; }


    private static float[] playerPunchRange = { 1.3f, 1f, 1f, 1f, 1f };
    public static float[] PlayerPunchRange { get => playerPunchRange; }

    
    private static float[] playerDamage = { 40, 50, 80, 120, 200 };
    public static float[] PlayerDamage { get => playerDamage; }
}
