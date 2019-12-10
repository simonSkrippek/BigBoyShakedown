using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    // Start is called before the first frame update
    public float score;
    float changeValue;
    [SerializeField] LineRenderer line;
    List<Vector3> linePositions;
    bool scoreChanged = false;
    [SerializeField] scoreZone[] scoreZones;

    bool changeInProgress = false;
    int framesChangeInProgress = 0;
    [Header("change length")][Tooltip("time score changes in frames")]
    [SerializeField] int scoreChangeLength = 3;
    

    void Start()
    {
        score = 0;
        linePositions = new List<Vector3>();

        foreach (var item in scoreZones)
        {
            item.OnCharacterEnter += ScoreChangedHandler;
        }
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (linePositions.Count <= 1)
        {
            var v = new Vector3(GetTime(), score, 0);
            linePositions.Add(v);
            linePositions.Add(v);
            line.positionCount = linePositions.Count;

        }

        if (scoreChanged)
        {
            if (!changeInProgress) linePositions.Add(new Vector3(GetTime(), score, 0));
            linePositions.Add(new Vector3(GetTime(), score, 0));
            line.positionCount = linePositions.Count;
            scoreChanged = false;
            Debug.Log("score no longer changed");
        }
        if (changeInProgress)
        {
            linePositions[linePositions.Count - 1] = new Vector3(GetTime(), score - (changeValue / (float)scoreChangeLength * ((float)scoreChangeLength - (float)framesChangeInProgress++)), 0);
            if (framesChangeInProgress == scoreChangeLength)
            {
                changeInProgress = false;
                Debug.Log("change no longer in progress");
                linePositions.Add(new Vector3(GetTime(), score, 0));
                line.positionCount = linePositions.Count;
            }
        }
        if (!changeInProgress && !scoreChanged)
        {

            linePositions[linePositions.Count - 1] = new Vector3(GetTime(), score, 0);
        }

        line.SetPositions(linePositions.ToArray());
    }

    float GetTime()
    {
        return Time.timeSinceLevelLoad / 5f;
    }

    public void ScoreChangedHandler(float scoreChange)
    {
        if (changeInProgress)
        {
            linePositions.Add(new Vector2(GetTime(), score));
        }

        scoreChanged = true;
        Debug.Log("score changed");

        score += scoreChange;
        changeValue = scoreChange;

        changeInProgress = true;
        Debug.Log("change in progress");
        framesChangeInProgress = 0;
    }
}
