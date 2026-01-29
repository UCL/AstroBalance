using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class StarSeekGenerator : MonoBehaviour
{
    [SerializeField, Tooltip("Star prefab to generate")]
    private GameObject starPrefab;

    private List<Vector2> spawnLocations = new List<Vector2>
    {
        new Vector2(0, 4), // up
        new Vector2(0, -4), // down
    };
    private GameObject currentStar;
    private int lastSpawnLocationIndex = -1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Add left + right spawn locations for stars. These are calculated based on the
        // camera extent, so the stars appear at the edge of the screen for all screen sizes and
        // aspect ratios (using a fixed left / right location means stars will be cut-off for certain
        // screens)
        Vector3 starOffset = new Vector3(1, 0, 0); // How many unity units to offset the star from the edge of the screen
        Vector2 leftSpawn = Camera.main.ViewportToWorldPoint(new Vector2(0, 0.5f)) + starOffset;
        Vector2 rightSpawn = Camera.main.ViewportToWorldPoint(new Vector2(1, 0.5f)) - starOffset;
        spawnLocations.Add(leftSpawn);
        spawnLocations.Add(rightSpawn);

        spawnStar();
    }

    private void spawnStar()
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
            spawnStar();
        }
    }
}
