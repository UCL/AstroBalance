using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    [SerializeField, Tooltip("Background Scrolling Rate")]
    public float speed = 1.0f;

    private Material mat;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mat = GetComponent<SpriteRenderer>().material;
        ResetOffset();
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        mat.mainTextureOffset = mat.mainTextureOffset + new Vector2(0, speed * Time.deltaTime);
    }

    public Vector2 GetOffset()
    {
        return mat.mainTextureOffset;
    }

    public void ResetOffset()
    {
        mat.mainTextureOffset = new Vector2(0f, 0f);
    }
}
