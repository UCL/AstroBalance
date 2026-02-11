using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HeadTurnScreen : MonoBehaviour
{
    [SerializeField, Tooltip("Head turn arrow prefabs")]
    private HeadTurnArrow[] headTurnArrows;

    private List<HeadTurnArrow> arrowsLeft; // arrow prefabs left to choose from
    private GameObject currentArrow;
    private bool arrowActive = false; // whether the screen is currently showing an active arrow
    private SpaceWalkingManager gameManager;

    void Awake()
    {
        arrowsLeft = new List<HeadTurnArrow>(headTurnArrows);
        gameManager = FindFirstObjectByType<SpaceWalkingManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameManager.IsGameActive() || !arrowActive)
        {
            return;
        }

        if (currentArrow.IsDestroyed())
        {
            arrowActive = false;
            gameObject.SetActive(false);
            gameManager.NextTile();
        }
    }

    /// <summary>
    /// Activate the screen, and spawn a random arrow pointing up / down / left or right.
    /// </summary>
    public void SpawnRandomArrow()
    {
        gameObject.SetActive(true);

        if (arrowsLeft.Count == 0)
        {
            arrowsLeft = new List<HeadTurnArrow>(headTurnArrows);
        }

        // Choose an arrow, then remove it from the list to select from.
        // This ensures we cover all directions in a random order, before
        // starting again.
        HeadTurnArrow chosenArrow = arrowsLeft[Random.Range(0, arrowsLeft.Count)];
        arrowsLeft.Remove(chosenArrow);

        currentArrow = Instantiate<GameObject>(chosenArrow.gameObject, transform);
        arrowActive = true;
    }
}
