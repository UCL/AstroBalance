using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{

    [SerializeField, Tooltip("Number of mm of head movement to count as a step")]
    private int stepMm = 300;
    [SerializeField, Tooltip("stepMm +/- toleranceMm will still count as a step")]
    private int toleranceMm = 100;
    [SerializeField, Tooltip("Starting distance from screen in mm")]
    private int startDistance = 900; 

    private List<Tile> directionTiles = new List<Tile>();
    private List<Tile> directionTilesLeft = new List<Tile>();
    private Tile centreTile;
    
    private Tile currentTile;
    private Vector3 centralHeadPosition; // expected head position at central tile (matching tobii mm units)


    void Awake()
    {
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
    }

    public int GetStartDistance()
    {
        return startDistance;
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

    public int GetStepMm()
    {
        return stepMm;
    }

    public void ActivateNextTile()
    {
        bool scored = true;

        if(currentTile == null)
        {
            // This is our first tile (we need to start at the centre). It shouldn't 
            // be scored, as it just ensures the player starts the game from the right
            // position
            currentTile = centreTile;
            scored = false;
        } 
        else if (currentTile.GetDirection() != Tile.Direction.None)
        {
            // The last tile was a directional step, and now we need to go back to the centre
            currentTile = centreTile;
        }
        else
        {
            // We're at the centre - choose a direction
            ChooseRandomDirection();
        }

        var bounds = GetTileHeadBounds(currentTile.GetDirection());
        currentTile.ActivateTile(
            bounds.xMin, 
            bounds.xMax, 
            bounds.zMin, 
            bounds.zMax,
            scored
        );
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
        var bounds = (
            xMin: centralHeadPosition.x - toleranceMm,
            xMax: centralHeadPosition.x + toleranceMm,
            zMin: centralHeadPosition.z - toleranceMm,
            zMax: centralHeadPosition.z + toleranceMm
        );

        switch (direction)
        {
            case Tile.Direction.Forward:
                bounds.zMin -= stepMm;
                bounds.zMax -= stepMm;
                break;

            case Tile.Direction.Backward:
                bounds.zMin += stepMm;
                bounds.zMax += stepMm;
                break;

            case Tile.Direction.Left:
                bounds.xMin -= stepMm;
                bounds.xMax -= stepMm;
                break;

            case Tile.Direction.Right:
                bounds.xMin += stepMm;
                bounds.xMax += stepMm;
                break;
        }

        return bounds;
    }
    
}
