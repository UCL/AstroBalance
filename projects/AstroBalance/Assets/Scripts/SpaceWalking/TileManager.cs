using System.Collections.Generic;
using Tobii.GameIntegration.Net;
using UnityEngine;

public class TileManager : MonoBehaviour
{

    [SerializeField, Tooltip("Min number of mm of head movement to count as a step")]
    private int minStepMm = 200;
    [SerializeField, Tooltip("Max number of mm of head movement to count as a step")]
    private int maxStepMm = 2000; // default to a very high value - we don't mind if the player steps too far
    [SerializeField, Tooltip(
        "Number of mm of head movement perpendicular to the step direction to accept e.g. if step is forward, how many mm do we allow them to move left/right?"
    )]
    private int toleranceMm = 100;
    [SerializeField, Tooltip("Starting distance from screen in mm")]
    private int startDistance = 700; 

    private List<Tile> directionTiles = new List<Tile>();
    private List<Tile> directionTilesLeft = new List<Tile>();
    private Tile centreTile;
    
    private Tile currentTile;
    private Tracker tracker;
    private Vector3 centralHeadPosition; // expected head position at central tile (matching tobii mm units)


    void Awake()
    {
        tracker = FindFirstObjectByType<Tracker>();
        centralHeadPosition = new Vector3(0, 0, startDistance);

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

        directionTilesLeft = new List<Tile>(directionTiles);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    void Update()
    {

        // needs to separate centre tile from teh other tiles
        // choose an ohter tile, then centre, then other, then centre...

        // Tile needs a head position range to consider as a 'hit'

    }

    public int GetStartDistance()
    {
        return startDistance;
    }

    public Vector3 GetCentreTilePosition()
    {
        return centreTile.transform.position;
    }

    public Vector3 GetTilePosition(Tile.Direction direction)
    {
        Tile chosenTile = centreTile;

        if (direction != Tile.Direction.None) {
            foreach (Tile tile in directionTiles)
            {
                if (tile.GetDirection() == direction)
                {
                    chosenTile = tile;
                    break;
                }
            }
        }

        return chosenTile.transform.position;
    }

    public int GetMinStepMm()
    {
        return minStepMm;
    }

    public void ActivateNextTile()
    {
        if(currentTile != null && currentTile.GetDirection() != Tile.Direction.None)
        {
            // Last tile was a directional step, now go back to centre
            currentTile = centreTile;
        }
        else
        {
            // We're at the centre - choose a direction
            ChooseRandomDirection();
        }

        var bounds = GetTileHeadBounds(currentTile.GetDirection());
        currentTile.ActivateTile(bounds.xMin, bounds.xMax, bounds.zMin, bounds.zMax);
    }

    private void ChooseRandomDirection()
    {
        if (directionTilesLeft.Count == 0)
        {
            directionTilesLeft = new List<Tile>(directionTiles);
        }

        // Choose a tile, then remove it from the list to select from.
        // This ensures we cover all directions in a random order, before
        // starting again (using a truly random order can result in many 
        // repeats of the same few directions in a row)
        currentTile = directionTilesLeft[Random.Range(0, directionTilesLeft.Count)];
        directionTilesLeft.Remove(currentTile);
    }

    private (float xMin, float xMax, float zMin, float zMax) GetTileHeadBounds(Tile.Direction direction)
    {

        switch (direction)
        {
            case Tile.Direction.Forward:
                return (
                    xMin: centralHeadPosition.x - toleranceMm,
                    xMax: centralHeadPosition.x + toleranceMm,
                    zMin: centralHeadPosition.z - maxStepMm,
                    zMax: centralHeadPosition.z - minStepMm
                );

            case Tile.Direction.Backward:
                return (
                    xMin: centralHeadPosition.x - toleranceMm,
                    xMax: centralHeadPosition.x + toleranceMm,
                    zMin: centralHeadPosition.z+ minStepMm,
                    zMax: centralHeadPosition.z + maxStepMm
                );

            case Tile.Direction.Left:
                return (
                    xMin: centralHeadPosition.x - maxStepMm,
                    xMax: centralHeadPosition.x - minStepMm,
                    zMin: centralHeadPosition.z - toleranceMm,
                    zMax: centralHeadPosition.z + toleranceMm
                );

            case Tile.Direction.Right:
                return (
                    xMin: centralHeadPosition.x + minStepMm,
                    xMax: centralHeadPosition.x + maxStepMm,
                    zMin: centralHeadPosition.z - toleranceMm,
                    zMax: centralHeadPosition.z + toleranceMm
                );

            // central tile
            default:
                return (
                    xMin: centralHeadPosition.x - toleranceMm,
                    xMax: centralHeadPosition.x + toleranceMm,
                    zMin: centralHeadPosition.z - toleranceMm,
                    zMax: centralHeadPosition.z + toleranceMm
                );
        }
    }
    
}
