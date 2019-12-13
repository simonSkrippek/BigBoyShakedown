using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphManager
{
    #region objects
    private LineRenderer graph;
    private CoordinateSystem coordinateSystem;
    #endregion
    #region values
    private float graphValue;
    #region change
    private bool scoreChanged;
    private bool changeInProgress;

    private float changeValue;
    private float changeAnimationRate = .008f;
    #endregion
    private Vector3 pointToSet;
    #endregion
    
    public GraphManager(LineRenderer graph, CoordinateSystem coordinateSystem, float startValue)
    {
        this.graph = graph;
        this.coordinateSystem = coordinateSystem;
        graphValue = startValue;

        StartGraph();
    }
    public void StartGraph()
    {
        var point = coordinateSystem.CoordinateToWorldPoint(coordinateSystem.coordinateMin.x, graphValue);
        AddPointToGraph(coordinateSystem.CoordinateToWorldPoint(coordinateSystem.coordinateMin.x, graphValue));
        AddPointToGraph(coordinateSystem.CoordinateToWorldPoint(GetTime(), graphValue));
    }

    //change graph value by @value
    public void ChangeBy(float value)
    {
        changeValue += value;
        scoreChanged = true;
    }

    public void FixedUpdate()
    {
        if (scoreChanged)
        {
            changeInProgress = true;
            scoreChanged = false;
            AddPointToGraph(coordinateSystem.CoordinateToWorldPoint(GetTime(), graphValue));
        }
        if (changeInProgress) ApplyChangesToGraphValue();
        ChangeLastGraphPoint(coordinateSystem.CoordinateToWorldPoint(GetTime(), graphValue));
    }

    private void ApplyChangesToGraphValue()
    {
        var minVal = coordinateSystem.coordinateMin.y;
        var maxVal = coordinateSystem.coordinateMax.y;
        if (changeValue < 0)
        {
            graphValue -= changeAnimationRate;
            if (graphValue <= minVal)
            {
                AddPointToGraph(coordinateSystem.CoordinateToWorldPoint(GetTime(), graphValue));
                changeInProgress = false;
                graphValue = minVal;
                changeValue = 0;
                return;
            }
            changeValue += changeAnimationRate;
            if (changeValue >= 0)
            {
                AddPointToGraph(coordinateSystem.CoordinateToWorldPoint(GetTime(), graphValue));
                changeInProgress = false;
                changeValue = 0;
            }
        }
        else
        {
            graphValue += changeAnimationRate;
            if (graphValue >= maxVal)
            {
                changeInProgress = false;
                graphValue = maxVal;
                changeValue = 0;
                AddPointToGraph(coordinateSystem.CoordinateToWorldPoint(GetTime(), graphValue));
                return;
            }
            changeValue -= changeAnimationRate;
            if (changeValue < 0)
            {
                AddPointToGraph(coordinateSystem.CoordinateToWorldPoint(GetTime(), graphValue));
                changeInProgress = false;
                changeValue = 0;
            }
        }
    }

    private void AddPointToGraph(Vector3 point)
    {
        graph.positionCount++;
        pointToSet = point;
        SetLastGraphPoint();
    }
    private void ChangeLastGraphPoint(Vector3 vector3)
    {
        pointToSet = coordinateSystem.CoordinateToWorldPoint(GetTime(), graphValue);
        SetLastGraphPoint();
    }
    private void SetLastGraphPoint()
    {
        var lastIndex = graph.positionCount - 1;
        graph.SetPosition(lastIndex, pointToSet);
    }
    private float GetTime()
    {
        return Time.timeSinceLevelLoad;
    }
}
