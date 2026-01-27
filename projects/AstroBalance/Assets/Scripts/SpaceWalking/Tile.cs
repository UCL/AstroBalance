using System.Collections;
using Tobii.GameIntegration.Net;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField, Tooltip("The step direction of this tile. Use None for the centre tile.")]
    private Direction direction;

    [SerializeField, Tooltip("Particle system to show on tile selection.")]
    private GameObject sparkleEffect;

    [SerializeField, Tooltip("Number of seconds to show particle system.")]
    private int sparkleTime = 1;

    [SerializeField, Tooltip("Tile activation colour")]
    private Color activatedColor = Color.red;

    private SpriteRenderer spriteRenderer;
    private Tracker tracker;
    private TileManager tileManager;
    private SpaceWalkingManager gameManager;
    private GameObject selectedSparkle;
    private float headXMin;
    private float headXMax;
    private float headZMin;
    private float headZMax;

    private bool tileActive = false;
    private bool allowScoring = false;

    /// <summary>
    /// Direction of step required to select this tile.
    /// </summary>
    public enum Direction
    {
        None,
        Forward,
        Backward,
        Left,
        Right,
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        tracker = FindFirstObjectByType<Tracker>();
        gameManager = FindFirstObjectByType<SpaceWalkingManager>();
        tileManager = GetComponentInParent<TileManager>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }

    // Update is called once per frame
    void Update()
    {
        if (!tileActive)
        {
            return;
        }

        Position headPosition = tracker.getHeadPosition();
        if (
            headPosition.X >= headXMin
            && headPosition.X <= headXMax
            && headPosition.Z >= headZMin
            && headPosition.Z <= headZMax
        )
        {
            SelectTile();
        }
    }

    public Direction GetDirection()
    {
        return direction;
    }

    /// <summary>
    /// Activate the tile - allowing it to be selected via head movement
    /// into a given x / z range.
    /// </summary>
    /// <param name="xMin">Minimum head x position (mm) for selection</param>
    /// <param name="xMax">Maximum head x position (mm) for selection</param>
    /// <param name="zMin">Minumum head z position (mm) for selection</param>
    /// <param name="zMax">Maximum head z position (mm) for selection</param>
    /// <param name="scored">Whether selection of this tile adds to the game's score</param>
    public void ActivateTile(float xMin, float xMax, float zMin, float zMax, bool scored)
    {
        headXMin = xMin;
        headXMax = xMax;
        headZMin = zMin;
        headZMax = zMax;
        allowScoring = scored;

        spriteRenderer.color = activatedColor;
        tileActive = true;
    }

    private void SelectTile()
    {
        tileActive = false;
        selectedSparkle = Instantiate<GameObject>(
            sparkleEffect,
            transform.position,
            Quaternion.identity
        );
        StartCoroutine(ResetTile());
    }

    private IEnumerator ResetTile()
    {
        yield return new WaitForSeconds(sparkleTime);

        Destroy(selectedSparkle);
        spriteRenderer.color = Color.white;

        if (direction == Direction.None && allowScoring)
        {
            // If the centre tile has been selected successfully, then we have completed a
            // step in and out + the score must be updated
            gameManager.UpdateScore();
        }

        if (gameManager.IsGameActive())
        {
            tileManager.ActivateNextTile();
        }
    }
}
