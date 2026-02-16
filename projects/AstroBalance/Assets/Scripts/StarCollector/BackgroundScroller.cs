using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    [SerializeField, Tooltip("Background Scrolling Rate")] private float speed = 1.0f;

    private Material mat;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mat = GetComponent<SpriteRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        float t = Time.time;
        mat.mainTextureOffset = new Vector2(0, speed * t);
    }
}
