using UnityEngine;

public class FlameOscillator : MonoBehaviour
{
    [SerializeField, Tooltip("Magnitude of flame wobble effect")]
    private float A;
    [SerializeField, Tooltip("Frequency of flame wobble effect")]
    private float f;

    private SpriteRenderer Renderer;
    private Vector3 original_scale;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Renderer = GetComponent<SpriteRenderer>();
        original_scale = Renderer.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 new_scale = new Vector3(original_scale.x, original_scale.y + A * Mathf.Sin(Mathf.PI * f * Time.time), original_scale.z);
        Renderer.transform.localScale = new_scale;
    }
}
