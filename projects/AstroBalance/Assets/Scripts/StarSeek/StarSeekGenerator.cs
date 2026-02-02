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

    private List<Vector2> spawnLocations = new List<Vector2>();
    private GameObject currentStar;
    private int lastSpawnLocationIndex = -1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        FillSpawnLocations();
        SpawnStar();

        //foreach (Vector2 location in spawnLocations)
        //{
        //    Instantiate(
        //    starPrefab,
        //    new Vector3(location.x, location.y, 0),
        //    Quaternion.identity
        //);
        //}
    }

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
            spawnLocations.Add(new Vector2(xLocation, yLocation));

            for (int j = 0; j < nRows - 1; j++)
            {
                yLocation += ySpacing;
                spawnLocations.Add(new Vector2(xLocation, yLocation));
            }

            xLocation += xSpacing;
            yLocation = gridBottomLeft.y;
        }
    }

    private void SpawnStar()
    {
        int chosenIndex;
        if (lastSpawnLocationIndex == -1)
        {
            // If this is our first time generating a star, choose from all locations
            chosenIndex = Random.Range(0, spawnLocations.Count);
        }
        else
        {
            // If we have spawned a star previously, make sure it moves to a new location
            IEnumerable<int> indexes = Enumerable.Range(0, spawnLocations.Count);
            indexes = indexes.Except(new int[] { lastSpawnLocationIndex });

            chosenIndex = indexes.ElementAt(Random.Range(0, indexes.Count()));
        }

        // Spawn a star at the randomly chosen location
        Vector2 chosenLocation = spawnLocations.ElementAt(chosenIndex);
        currentStar = Instantiate(
            starPrefab,
            new Vector3(chosenLocation.x, chosenLocation.y, 0),
            Quaternion.identity
        );
        lastSpawnLocationIndex = chosenIndex;
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
