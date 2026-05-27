using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class SplashAnimation : MonoBehaviour
{
    [SerializeField, Tooltip("Background game object")]
    private SpriteRenderer background;

    [SerializeField, Tooltip("Rocket game object")]
    private SpriteRenderer rocket;

    [SerializeField, Tooltip("Speed to move the camera at")]
    private float cameraMoveSpeed = 1f;

    [SerializeField, Tooltip("Speed to move the camera at")]
    private float textFadeSeconds = 1f;

    private Camera cam;
    private float maxCameraYPos;
    private float maxRocketYPos;
    private CanvasGroup canvas;

    void Awake()
    {
        canvas = FindFirstObjectByType<CanvasGroup>();
        canvas.alpha = 0f;
        canvas.interactable = false;
        cam = Camera.main;

        // Match width of camera to width of background
        float requiredSize = background.bounds.size.x / (cam.aspect * 2f);
        cam.orthographicSize = requiredSize;

        // Move camera to bottom of background
        Vector3 cameraPosition = cam.transform.position;
        float yPos = background.bounds.min.y + cam.orthographicSize;
        cam.transform.position = new Vector3(cameraPosition.x, yPos, cameraPosition.z);

        // Find max camera position (where aligned with top of background)
        maxCameraYPos = background.bounds.max.y - cam.orthographicSize;

        // Find max rocket position (just beyond background)
        maxRocketYPos = background.bounds.max.y + rocket.bounds.size.y;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }

    // Update is called once per frame
    void Update()
    {
        TranslateToMaxY(cam.transform, maxCameraYPos);
        TranslateToMaxY(rocket.transform, maxRocketYPos);

        bool moveFinished = (
            cam.transform.position.y == maxCameraYPos
            && rocket.transform.position.y == maxRocketYPos
        );

        if (canvas.alpha == 0 && moveFinished)
        {
            StartCoroutine(FadeInCanvas());
        }
    }

    private IEnumerator FadeInCanvas()
    {
        float elapsedTime = 0f;
        while (elapsedTime < textFadeSeconds)
        {
            elapsedTime += Time.deltaTime;
            canvas.alpha = Mathf.Lerp(0, 1, elapsedTime / textFadeSeconds);
            yield return null;
        }

        canvas.alpha = 1;
        canvas.interactable = true;
    }

    private void TranslateToMaxY(Transform transform, float maxY)
    {
        Vector3 yTranslate = Vector3.up * cameraMoveSpeed * Time.deltaTime;
        Vector3 position = transform.position;

        if (position.y < maxY)
        {
            if ((position + yTranslate).y > maxY)
            {
                transform.position = new Vector3(position.x, maxY, position.z);
            }
            else
            {
                transform.Translate(yTranslate, Space.World);
            }
        }
    }
}
