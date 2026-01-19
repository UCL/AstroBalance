using Tobii.GameIntegration.Net;
using UnityEditor.Tilemaps;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField, Tooltip("The step direction of this tile. Use None for the centre tile.")]
    private Direction direction;
    [SerializeField, Tooltip("Particle system to show on pending tile selection.")]
    private GameObject sparkleEffect;

    private SpriteRenderer spriteRenderer;
    private Tracker tracker;
    private float headXMin;
    private float headXMax;
    private float headZMin;
    private float headZMax;

    private bool tileActive = false;

    public enum Direction
    {
        None,
        Forward,
        Backward,
        Left,
        Right
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        tracker = FindFirstObjectByType<Tracker>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (!tileActive)
        {
            return;
        }

        Position headPosition = tracker.getHeadPosition();
        if (
            headPosition.X >= headXMin && 
            headPosition.X <= headXMax && 
            headPosition.Z >= headZMin &&
            headPosition.Z <= headZMax
            )
        {
            Debug.Log(headPosition.X + "," + headPosition.Y + "," + headPosition.Z);
            Debug.Log("SELECTING TILE");
            SelectTile();
        }
        
    }

    public Direction GetDirection()
    {
        return direction;
    }

    public void ActivateTile(float xMin, float xMax, float zMin, float zMax)
    {
        Debug.Log("bounds");
        Debug.Log(xMin);
        Debug.Log(xMax);
        Debug.Log(zMin);
        Debug.Log(zMax);

        headXMin = xMin;
        headXMax = xMax;
        headZMin = zMin;
        headZMax = zMax;

        Debug.Log("Highlighting" + gameObject.name);
        spriteRenderer.color = Color.red;

        tileActive = true;
    }

    private void SelectTile()
    {
        tileActive = false;

        GameObject selectedSparkle = Instantiate<GameObject>(sparkleEffect, transform.position, Quaternion.identity);
    }
}
