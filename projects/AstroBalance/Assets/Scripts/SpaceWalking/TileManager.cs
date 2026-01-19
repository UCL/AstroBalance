using System;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{

    [SerializeField, Tooltip("Number of cm of head movement to count as a step")]
    private int stepCentimetres = 30;

    private List<Tile> directionTiles = new List<Tile>();
    private Tile centreTile;

    private Tile currentTile;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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

        StartSelectingTiles();
    }

    public void StartSelectingTiles()
    {
        Debug.Log(directionTiles.Count);
        if (currentTile == null)
        {
            Tile randomTile = directionTiles[UnityEngine.Random.Range(0, directionTiles.Count)];
            randomTile.SelectTile();
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
