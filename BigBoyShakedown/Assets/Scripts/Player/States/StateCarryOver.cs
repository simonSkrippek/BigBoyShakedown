using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateCarryOver : MonoBehaviour
{
    [HideInInspector]
    public Vector3 previousMovement;
    [HideInInspector]
    public float stunDuration;
    [HideInInspector]
    public Vector3 knockbackDistance;

    void Awake()
    {
        Reset();
    }

    public void Reset()
    {
        stunDuration = 0f;
        previousMovement = Vector3.zero;
        knockbackDistance = Vector3.zero;
    }
}
