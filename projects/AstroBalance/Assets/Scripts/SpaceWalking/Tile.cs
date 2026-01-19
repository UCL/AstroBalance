using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField, Tooltip("The step direction of this tile. Use None for the centre tile.")]
    private Direction direction;

    private SpriteRenderer spriteRenderer;

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
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Direction GetDirection()
    {
        return direction;
    }

    public void SelectTile()
    {
        Debug.Log("Highlighting" + gameObject.name);
        spriteRenderer.color = Color.red;
    }
}
