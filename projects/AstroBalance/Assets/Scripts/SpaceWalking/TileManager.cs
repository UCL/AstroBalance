using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using Tobii.GameIntegration.Net;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class TileManager : MonoBehaviour
{

    [SerializeField, Tooltip("Min number of mm of head movement to count as a step")]
    private int minStepMm = 300;
    [SerializeField, Tooltip("Max number of mm of head movement to count as a step")]
    private int maxStepMm = 2000;
    [SerializeField, Tooltip(
        "Number of mm of head movement perpendicular to the step direction to accept e.g. if step is forward, how many mm do we allow them to move left/right?"
    )]
    private int toleranceMm = 20;

    private List<Tile> directionTiles = new List<Tile>();
    private Tile centreTile;
    
    private Tile currentTile;
    private Tracker tracker;
    private Position restingHeadPosition;


    void Awake()
    {
        tracker = FindFirstObjectByType<Tracker>();

        Tile[] tiles = GetComponentsInChildren<Tile>();
        foreach (Tile tile in tiles)
        {
            if (tile.GetDirection() == Tile.Direction.None)
            {
                centreTile = tile;
            } else
            {
                directionTiles.Add(tile);
            }
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    public void StartSelectingTiles()
    {
        restingHeadPosition = tracker.getHeadPosition();

        Debug.Log("rest" + restingHeadPosition.X + "," + restingHeadPosition.Y + "," + restingHeadPosition.Z);

        if (currentTile == null)
        {
            Tile randomTile = directionTiles[UnityEngine.Random.Range(0, directionTiles.Count)];
            var bounds = GetTileHeadBounds(randomTile.GetDirection());
            randomTile.ActivateTile(bounds.xMin, bounds.xMax, bounds.zMin, bounds.zMax);
        }

    }

    private (float xMin, float xMax, float zMin, float zMax) GetTileHeadBounds(Tile.Direction direction)
    {

        switch (direction)
        {
            case Tile.Direction.Forward:
                return (
                    xMin: restingHeadPosition.X - toleranceMm,
                    xMax: restingHeadPosition.X + toleranceMm,
                    zMin: restingHeadPosition.Z + minStepMm,
                    zMax: restingHeadPosition.Z + maxStepMm
                );

            case Tile.Direction.Backward:
                return (
                    xMin: restingHeadPosition.X - toleranceMm,
                    xMax: restingHeadPosition.X + toleranceMm,
                    zMin: restingHeadPosition.Z - minStepMm,
                    zMax: restingHeadPosition.Z - maxStepMm
                );

            case Tile.Direction.Left:
                return (
                    xMin: restingHeadPosition.X - minStepMm,
                    xMax: restingHeadPosition.X - maxStepMm,
                    zMin: restingHeadPosition.Z - toleranceMm,
                    zMax: restingHeadPosition.Z + toleranceMm
                );

            case Tile.Direction.Right:
                return (
                    xMin: restingHeadPosition.X + minStepMm,
                    xMax: restingHeadPosition.X + maxStepMm,
                    zMin: restingHeadPosition.Z - toleranceMm,
                    zMax: restingHeadPosition.Z + toleranceMm
                );

            // central tile
            default:
                return (
                    xMin: restingHeadPosition.X - toleranceMm,
                    xMax: restingHeadPosition.X + toleranceMm,
                    zMin: restingHeadPosition.Z - toleranceMm,
                    zMax: restingHeadPosition.Z - toleranceMm
                );
        }
    }

    // Update is called once per frame
    void Update()
    {

        // needs to separate centre tile from teh other tiles
        // choose an ohter tile, then centre, then other, then centre...

        // Tile needs a head position range to consider as a 'hit'
        
    }
}
