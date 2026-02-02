using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class StarSeekGenerator : MonoBehaviour
{
    [SerializeField, Tooltip("Star prefab to generate")]
    private GameObject starPrefab;

    [SerializeField, Tooltip("Number of unity units to offset stars from the edge of the screen.")]
    private int offset = 1;

    private List<Vector2> spawnLocations = new List<Vector2>();
    private GameObject currentStar;
    private int lastSpawnLocationIndex = -1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Calculate star spawn locations based on the camera extent. This ensures stars appear at the edge
        // of the screen for all screen sizes and aspect ratios.
        Vector2 leftPos =
            Camera.main.ViewportToWorldPoint(new Vector2(0, 0.5f)) + new Vector3(offset, 0, 0);
        Vector2 rightPos =
            Camera.main.ViewportToWorldPoint(new Vector2(1, 0.5f)) - new Vector3(offset, 0, 0);
        Vector2 topPos =
            Camera.main.ViewportToWorldPoint(new Vector2(0.5f, 1)) - new Vector3(0, offset, 0);
        Vector2 bottomPos =
            Camera.main.ViewportToWorldPoint(new Vector2(0.5f, 0)) + new Vector3(0, offset, 0);

        spawnLocations.Add(leftPos);
        spawnLocations.Add(rightPos);
        spawnLocations.Add(topPos);
        spawnLocations.Add(bottomPos);

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
