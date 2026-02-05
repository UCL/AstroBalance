using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    [SerializeField, Tooltip("Number of mm of head movement to count as a step")]
    private int stepMm = 300;

    [SerializeField, Tooltip("stepMm +/- toleranceMm will still count as a step")]
    private int toleranceMm = 100;

    [SerializeField, Tooltip("Starting distance from screen in mm")]
    private int startDistance = 800;

    [
        SerializeField,
        Tooltip(
            "UI screen showing the head turn commands (only used at highest level of stepping difficulty)"
        )
    ]
    private GameObject headTurnScreen;

    private List<Tile> directionTiles = new List<Tile>();
    private List<Tile> directionTilesLeft = new List<Tile>();
    private Tile centreTile;

    private Tile currentTile;
    private Vector3 centralHeadPosition; // expected head position at central tile (tobii mm units)

    void Awake()
    {
        centralHeadPosition = new Vector3(0, 0, startDistance);

        Tile[] tiles = GetComponentsInChildren<Tile>();
        foreach (Tile tile in tiles)
        {
            if (tile.GetDirection() == Tile.Direction.None)
            {
                centreTile = tile;
            }
            else
            {
                directionTiles.Add(tile);
            }
        }

        directionTilesLeft = new List<Tile>(directionTiles);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }

    void Update() { }

    /// <summary>
    /// Get the starting distance from the screen in mm.
    /// </summary>
    /// <returns>Distance in mm</returns>
    public int GetStartDistance()
    {
        return startDistance;
    }

    /// <summary>
    /// Get position of tile in unity world coordinates.
    /// </summary>
    /// <param name="direction">Direction of tile</param>
    /// <returns>Tile position as Vector3{x, y, z}</returns>
    public Vector3 GetTilePosition(Tile.Direction direction)
    {
        Tile chosenTile = centreTile;

        if (direction != Tile.Direction.None)
        {
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

    /// <summary>
    /// Activate the next tile - this will alternate between a directional tile
    /// (up / down / left / right) and the central tile.
    /// </summary>
    public void ActivateNextTile()
    {
        bool scored = true;

        if (currentTile == null)
        {
            // This is our first tile (we need to start at the centre). It shouldn't
            // be scored, as it just ensures the player starts the game from the right
            // position.
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
        currentTile.ActivateTile(bounds.xMin, bounds.xMax, bounds.zMin, bounds.zMax, scored);
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

    /// <summary>
    /// Get head position bounds (in mm) to select a given tile direction.
    /// We only give bounds on tobii's x axis (left-right) and z axis (towards /
    /// away from the screen), as we don't care about head up/down movement (y axis).
    /// </summary>
    /// <param name="direction">Tile direction</param>
    /// <returns>Bounds in mm as (xMin, xMax, zMin, zMax)</returns>
    private (float xMin, float xMax, float zMin, float zMax) GetTileHeadBounds(
        Tile.Direction direction
    )
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
