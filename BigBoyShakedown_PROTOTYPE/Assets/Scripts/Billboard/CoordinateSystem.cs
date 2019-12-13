using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoordinateSystem
{
    public Vector2 worldStartPoint, worldEndPoint, worldDifference;       

    public Vector2 coordinateMin, coordinateMax, coordinateDifference;

    public CoordinateSystem(Vector2 worldStartPoint, Vector2 worldEndPoint, Vector2 coordinateMin, Vector2 coordinateMax)
    {
        this.worldStartPoint = worldStartPoint;
        this.worldEndPoint = worldEndPoint;
        worldDifference = worldEndPoint - worldStartPoint;

        this.coordinateMin = coordinateMin;
        this.coordinateMax = coordinateMax;
        coordinateDifference = coordinateMax - coordinateMin;
    }

    public Vector3 CoordinateToWorldPoint(Vector2 coordinatePoint)
    {
        float xCoordinate = coordinatePoint.x;
        float xWorldPosition = worldStartPoint.x + ((xCoordinate - coordinateMin.x) / coordinateDifference.x) * worldDifference.x;

        float yCoordinate = coordinatePoint.y;
        float yWorldPosition = worldStartPoint.y + ((yCoordinate - coordinateMin.y) / coordinateDifference.y) * worldDifference.y;
               
        return new Vector3(xWorldPosition, yWorldPosition, 0);
    }
    public Vector3 CoordinateToWorldPoint(float xCoordinate, float yCoordinate)
    {
        float xWorldPosition = worldStartPoint.x + ((xCoordinate - coordinateMin.x) / coordinateDifference.x) * worldDifference.x;

        float yWorldPosition = worldStartPoint.y + ((yCoordinate - coordinateMin.y) / coordinateDifference.y) * worldDifference.y;

        return new Vector3(xWorldPosition, yWorldPosition, 0);
    }
}
