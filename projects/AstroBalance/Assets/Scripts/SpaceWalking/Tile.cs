using NUnit.Framework;
using System.Collections;
using Tobii.GameIntegration.Net;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Tilemaps;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField, Tooltip("The step direction of this tile. Use None for the centre tile.")]
    private Direction direction;
    [SerializeField, Tooltip("Particle system to show on pending tile selection.")]
    private GameObject sparkleEffect;
    [SerializeField, Tooltip("Number of seconds to show particle system.")]
    private int sparkleTime = 1;

    private SpriteRenderer spriteRenderer;
    private Tracker tracker;
    private TileManager tileManager;
    private GameObject selectedSparkle;
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
        tileManager = GetComponentInParent<TileManager>();
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
        //Debug.Log(headPosition.X + "," + headPosition.Y + "," + headPosition.Z);
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
        Debug.Log("xmin " + xMin);
        Debug.Log("xmax " + xMax);
        Debug.Log("zmin " + zMin);
        Debug.Log("zmax " + zMax);

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
        selectedSparkle = Instantiate<GameObject>(sparkleEffect, transform.position, Quaternion.identity);
        StartCoroutine(ResetTile());
    }

    private IEnumerator ResetTile()
    {
        yield return new WaitForSeconds(sparkleTime);

        Destroy(selectedSparkle);
        spriteRenderer.color = Color.white;
        tileManager.ActivateNextTile();
    }
}
