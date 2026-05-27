using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class SplashAnimation : MonoBehaviour
{
    [SerializeField, Tooltip("Background game object")]
    private SpriteRenderer background;

    [SerializeField, Tooltip("Rocket game object")]
    private GameObject rocket;

    [SerializeField, Tooltip("Speed to move the camera at")]
    private float cameraMoveSpeed = 1f;

    private Camera cam;
    private float maxCameraYPos;

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

        // Find max camera position (where aligned with top of background)
        maxCameraYPos = background.bounds.max.y - cam.orthographicSize;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }

    // Update is called once per frame
    void Update()
    {
        Vector3 camPosition = cam.transform.position;

        if (camPosition.y < maxCameraYPos)
        {
            Vector3 yTranslate = Vector3.up * cameraMoveSpeed * Time.deltaTime;
            if ((camPosition + yTranslate).y > maxCameraYPos)
            {
                cam.transform.position = new Vector3(camPosition.x, maxCameraYPos, camPosition.z);
            }
            else
            {
                cam.transform.Translate(yTranslate, Space.World);
            }
        }
    }
}
