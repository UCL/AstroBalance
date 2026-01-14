using System.Collections;
using UnityEngine;

public class StarMapStar : MonoBehaviour
{
    private Color highlightColor = Color.red;
    private Color defaultColor = Color.white;

    private SpriteRenderer spriteRenderer;

    void Awake()
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

    public void HighlightStar(int seconds)
    {
        spriteRenderer.color = highlightColor;
        StartCoroutine(ResetColor(seconds));
    }

    IEnumerator ResetColor(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        spriteRenderer.color = defaultColor;
    }
}
