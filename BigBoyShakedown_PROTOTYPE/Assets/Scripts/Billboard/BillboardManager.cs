using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardManager : MonoBehaviour
{
    #region Managers
    [Header("Other Managers")]
    [Tooltip("Multiplayer Manager; ONLY ONE IN SCENE")]
    [SerializeField] MultiplayerManager multiplayerManager;
    #endregion
    #region borders
    [Header("corner points")]
    [Tooltip("empty game object used to mark bottom left & top right of the billboard; use gizmo script")]
    [SerializeField] private Transform worldStartPoint, worldEndPoint;
    private CoordinateSystem coordinateSystem;
    #endregion
    #region lines
    List<GraphManager> graphManagers;
    List<LineRenderer> graphs;

    [Header("graph")]
    [Tooltip("prefab from which all graphs are instantiated")]
    [SerializeField] private GameObject graphPrefab;

    #endregion

    [Header("Player Metrics"), Tooltip("default player metrics"), SerializeField]
    private PlayerMetrics playerMetrics;

    [SerializeField] scoreZone[] scoreZones;

    private void Awake()
    {
        multiplayerManager.NewPlayerJoinedEvent += NewPlayerJoinedEventHandler;

        graphManagers = new List<GraphManager>();
        graphs = new List<LineRenderer>();
        InitialiseGraphs();
        InitialiseCoordinateSystem();

        foreach (var zone in scoreZones)
        {
            zone.OnCharacterEnter += ScoreChangedEventHandler;
        }
    }
    private void InitialiseCoordinateSystem()
    {
        coordinateSystem = new CoordinateSystem((Vector2)worldStartPoint.localPosition, (Vector2)worldEndPoint.localPosition, playerMetrics.PlayerMinScore, playerMetrics.PlayerMaxScore);
    }
    private void InitialiseGraphs()
    {
        for (int i = 0; i < 4; i++)
        {
            GameObject g = Instantiate(graphPrefab, this.transform.root);
            graphs.Add(g.GetComponent<LineRenderer>());
        }
        Debug.Log("GraphsInitialized");
    }
    
    private void NewPlayerJoinedEventHandler(PlayerController player, Material material)
    {
        AddGraph(material);
        player.scoreChangedEvent += ScoreChangedEventHandler;
    }
    private void AddGraph(Material graphMaterial)
    {
        if (graphManagers.Count >= 4) return;

        var graphToAdd = graphs[graphManagers.Count];
        graphToAdd.material = graphMaterial;
        graphToAdd.gameObject.SetActive(true);
        graphManagers.Add(new GraphManager(graphToAdd, coordinateSystem, playerMetrics.PlayerStartScore, playerMetrics.ChangeAnimationRate));
        Debug.Log("Graphs added");
    }

    private void FixedUpdate()
    {
        UpdateGraphs();
    }
    private void UpdateGraphs()
    {
        foreach (var item in graphManagers)
        {
            item.FixedUpdate();
        }
    }

    private void ScoreChangedEventHandler(int playerIndex, int scoreChange)
    {
        graphManagers[playerIndex].ChangeBy(scoreChange);
    }
}
