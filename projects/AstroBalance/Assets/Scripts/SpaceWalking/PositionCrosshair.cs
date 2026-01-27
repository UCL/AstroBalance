using Tobii.GameIntegration.Net;
using UnityEngine;

public class PositionCrosshair : MonoBehaviour
{
    [SerializeField, Tooltip("Direction tile manager")]
    private TileManager tileManager;

    private Tracker tracker;
    private float yScaling;
    private float xScaling;
    private Vector3 centrePosition;
    private SpriteRenderer spriteRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tracker = FindFirstObjectByType<Tracker>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        centrePosition = tileManager.GetTilePosition(Tile.Direction.None);
        float stepMm = tileManager.GetStepMm();

        // This assumes the forward / backward tile are equally spaced from the central tile
        float yDistUnityUnits =
            tileManager.GetTilePosition(Tile.Direction.Forward).y - centrePosition.y;
        yScaling = yDistUnityUnits / stepMm;

        // This assumes the left / right tile are equally spaced from the central tile
        float xDistUnityUnits =
            tileManager.GetTilePosition(Tile.Direction.Right).x - centrePosition.x;
        xScaling = xDistUnityUnits / stepMm;
    }

    // Update is called once per frame
    void Update()
    {
        if (tracker.isPlayerDetected())
        {
            spriteRenderer.enabled = true;
            Vector2 worldPos = GetWorldPosition();
            transform.position = new Vector3(worldPos.x, worldPos.y, 0);
        }
        else
        {
            spriteRenderer.enabled = false;
        }
    }

    /// <summary>
    /// Convert current head position to a point in unity world coordinates
    /// (matching the directional tile layout)
    /// </summary>
    private Vector2 GetWorldPosition()
    {
        Position headPosition = tracker.getHeadPosition();
        float xPosUnity = centrePosition.x + (headPosition.X * xScaling);
        // We use the head Z position (i.e. distance to/from the screen) from the eye tracker, to set the
        // position on the unity y axis
        float yPosUnity =
            centrePosition.y - ((headPosition.Z - tileManager.GetStartDistance()) * yScaling);

        return new Vector2(xPosUnity, yPosUnity);
    }
}
