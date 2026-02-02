using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class StarSeekGenerator : MonoBehaviour
{
    [SerializeField, Tooltip("Star prefab to generate")]
    private GameObject starPrefab;

    [SerializeField, Tooltip("Min distance between stars and the edge of the screen.")]
    private int edgeOffset = 1;

    [SerializeField, Tooltip("Number of rows in star spawn grid")]
    private int nRows = 4;

    [SerializeField, Tooltip("Number of columns in star spawn grid")]
    private int nColumns = 6;

    [
        SerializeField,
        Tooltip(
            "Positions in the spawn grid to exclude (e.g. overlapping with UI elements). (0, 0) is the bottom left star and (nColumns - 1, nRows - 1) is the top right star."
        )
    ]
    private List<Vector2> gridPositionsToExclude = new List<Vector2>();

    [SerializeField, Tooltip("Min distance between spawned stars")]
    private int minDistance = 8;

    private List<Vector2> spawnLocations = new List<Vector2>();
    private GameObject currentStar;
    private bool firstSpawn = true;
    private Vector2 lastSpawnLocation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        FillSpawnLocations();
        SpawnStar();

        //foreach (Vector2 location in spawnLocations)
        //{
        //    Instantiate(starPrefab, new Vector3(location.x, location.y, 0), Quaternion.identity);
        //}
    }

    /// <summary>
    /// Create a grid of spawn locations, excluding those in 'gridPositionsToExclude'
    /// </summary>
    private void FillSpawnLocations()
    {
        Vector2 gridBottomLeft =
            Camera.main.ViewportToWorldPoint(new Vector2(0, 0))
            + new Vector3(edgeOffset, edgeOffset, 0);
        Vector2 gridTopRight =
            Camera.main.ViewportToWorldPoint(new Vector2(1, 1))
            - new Vector3(edgeOffset, edgeOffset, 0);

        float xSpacing = (gridTopRight.x - gridBottomLeft.x) / (nColumns - 1);
        float ySpacing = (gridTopRight.y - gridBottomLeft.y) / (nRows - 1);

        float xLocation = gridBottomLeft.x;
        float yLocation = gridBottomLeft.y;
        for (int i = 0; i < nColumns; i++)
        {
            for (int j = 0; j < nRows; j++)
            {
                AddSpawnLocation(new Vector2(xLocation, yLocation), new Vector2(i, j));
                yLocation += ySpacing;
            }

            xLocation += xSpacing;
            yLocation = gridBottomLeft.y;
        }
    }

    private void AddSpawnLocation(Vector2 worldPosition, Vector2 gridPosition)
    {
        foreach (Vector2 excludeLocation in gridPositionsToExclude)
        {
            if (gridPosition == excludeLocation)
            {
                return;
            }
        }

        spawnLocations.Add(worldPosition);
    }

    private void SpawnStar()
    {
        List<Vector2> possibleLocations = new List<Vector2>();

        if (firstSpawn)
        {
            // If this is our first time generating a star, choose from all locations
            Debug.Log("FIRST");
            possibleLocations = spawnLocations;
            firstSpawn = false;
        }
        else
        {
            // If we have spawned a star previously, choose a new location that is at
            // least minDistance away
            foreach (Vector2 spawnLocation in spawnLocations)
            {
                float distance = Vector2.Distance(spawnLocation, lastSpawnLocation);
                if (distance >= minDistance)
                {
                    possibleLocations.Add(spawnLocation);
                }
            }
        }

        // Spawn a star at a randomly chosen location
        int chosenIndex = Random.Range(0, possibleLocations.Count);
        Vector2 chosenLocation = possibleLocations.ElementAt(chosenIndex);
        currentStar = Instantiate(
            starPrefab,
            new Vector3(chosenLocation.x, chosenLocation.y, 0),
            Quaternion.identity
        );
        lastSpawnLocation = chosenLocation;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentStar.IsDestroyed())
        {
            SpawnStar();
        }
    }
}
