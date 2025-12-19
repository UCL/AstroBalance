using UnityEngine;

public class LockedOn : MonoBehaviour
{
    private SpriteRenderer sprite;
    private Color defaultColor = Color.white;
    private Color lockedColor = Color.red;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        sprite.color = defaultColor;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (sprite.color == defaultColor)
            {
                sprite.color = lockedColor;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (sprite.color == lockedColor)
            {
                sprite.color = defaultColor;
            }
        }
    }
}
