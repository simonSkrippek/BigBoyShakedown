using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    // Start is called before the first frame update
    public float score;
    [SerializeField] LineRenderer line;
    List<Vector3> linePositions;
    bool scoreChanged = false;
    [SerializeField] scoreZone[] scoreZones;

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
        if (scoreChanged)
        {
            linePositions.Add(new Vector3(GetTime(), score, 0));
            linePositions.Add(new Vector3(GetTime(), score, 0));
            line.positionCount = linePositions.Count;
            scoreChanged = false;
        }
        else
        {
            if (linePositions.Count > 1)
            {
                linePositions[linePositions.Count - 1] = new Vector3(GetTime(), score, 0);
            }
            else
            {
                var v = new Vector3(GetTime(), score, 0);
                linePositions.Add(v);
                linePositions.Add(v);
                line.positionCount = linePositions.Count;
            }
        }
        line.SetPositions(linePositions.ToArray());
    }

    float GetTime()
    {
        return Time.timeSinceLevelLoad / 5f;
    }

    public void ScoreChangedHandler(float scoreChange)
    {
        scoreChanged = true;
        score += scoreChange;
    }
}
