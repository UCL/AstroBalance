using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class SplashAnimation : MonoBehaviour
{
    [SerializeField, Tooltip("Background game object")]
    private SpriteRenderer background;

    [SerializeField, Tooltip("Rocket game object")]
    private GameObject rocket;

    private Camera cam;

    void Awake()
    {
        Vector3 backgroundScale = background.transform.localScale;
        cam = Camera.main;
        Vector3 cameraScale = cam.transform.localScale;

        // Match width of camera to width of background
        float requiredSize = background.bounds.size.x / (cam.aspect * 2f);
        cam.orthographicSize = requiredSize;

        // Move camera to bottom of background
        Vector3 cameraPosition = cam.transform.position;
        float yPos = background.bounds.min.y + cam.orthographicSize;
        cam.transform.position = new Vector3(cameraPosition.x, yPos, cameraPosition.z);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }

    // Update is called once per frame
    void Update() { }
}
