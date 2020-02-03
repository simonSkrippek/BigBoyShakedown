using BigBoyShakedown.Player.Input;
using BigBoyShakedown.Player.Metrics;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BillboardManager : MonoBehaviour
{
    #region borders
    [Header("corner points")]
    [Tooltip("empty game object used to mark bottom left & top right of the billboard; use gizmo script")]
    [SerializeField] private Transform worldStartPoint, worldEndPoint;
    private CoordinateSystem coordinateSystem;
    #endregion
    #region lines
    List<GraphManager> graphManagers;
    List<LineRenderer> graphs;
    [SerializeField] Material defaultMaterial;

    [Header("graph")]
    [Tooltip("prefab from which all graphs are instantiated")]
    [SerializeField] private GameObject graphPrefab;

    #endregion

    [Header("Player Metrics"), Tooltip("default player metrics"), SerializeField]
    private PlayerMetrics playerMetrics;

    private void Awake()
    {
        playerMetrics = Resources.FindObjectsOfTypeAll<PlayerMetrics>()[0];

        graphManagers = new List<GraphManager>();
        graphs = new List<LineRenderer>();
        InitialiseGraphs();
        InitialiseCoordinateSystem();
    }
    private void InitialiseCoordinateSystem()
    {
        coordinateSystem = new CoordinateSystem((Vector2)worldStartPoint.localPosition, (Vector2)worldEndPoint.localPosition, playerMetrics.PlayerMinScore, playerMetrics.PlayerMaxScore);
    }
    private void InitialiseGraphs()
    {
        for (int i = 0; i < 4; i++)
        {
            var pos = this.transform.root.position;
            pos.z = 0;
            GameObject g = Instantiate(graphPrefab, pos, Quaternion.identity);
            graphs.Add(g.GetComponent<LineRenderer>());
        }
        Debug.Log("GraphsInitialized");
    }

    public void OnPlayerJoined(PlayerInputRelay relay, Material material, int index)
    {
        if (relay is null)
        {
            throw new System.ArgumentNullException(nameof(relay));
        }
        if (!material) material = defaultMaterial;
        AddGraph(material);

        switch (index)
        {
            case 0:
                relay.OnPlayerScoreChanged += Player1ScoreChangedEventHandler;
                break;
            case 1:
                relay.OnPlayerScoreChanged += Player2ScoreChangedEventHandler;
                break;
            case 2:
                relay.OnPlayerScoreChanged += Player3ScoreChangedEventHandler;
                break;
            case 3:
                relay.OnPlayerScoreChanged += Player4ScoreChangedEventHandler;
                break;
        }
    }

    private void Player1ScoreChangedEventHandler(float scoreChange)
    {
        graphManagers[0].ChangeBy(scoreChange);
    }
    private void Player2ScoreChangedEventHandler(float scoreChange)
    {
        graphManagers[1].ChangeBy(scoreChange);
    }
    private void Player3ScoreChangedEventHandler(float scoreChange)
    {
        graphManagers[2].ChangeBy(scoreChange);
    }
    private void Player4ScoreChangedEventHandler(float scoreChange)
    {
        graphManagers[3].ChangeBy(scoreChange);
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
}
