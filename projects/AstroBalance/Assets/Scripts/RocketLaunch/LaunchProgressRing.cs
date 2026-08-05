using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LaunchProgressRing : MonoBehaviour
{
    [SerializeField, Tooltip("Ring fill colour")]
    private Color fillColor = Color.red;

    LaunchControl countdownController;

    private Image fillImage;
    private float delaySeconds = 0.1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        countdownController = FindFirstObjectByType<LaunchControl>();
        Image[] ringImages = GetComponentsInChildren<Image>();

        foreach (Image image in ringImages)
        {
            if (image.type == Image.Type.Filled)
            {
                fillImage = image;
                break;
            }
        }

        fillImage.color = fillColor;

        FitToCountdown();
    }

    /// <summary>
    /// Fit the progress ring position and size to the countdown target
    /// </summary>
    private void FitToCountdown() { /*
        GameObject targetObject = countdownController.TargetObject;
        fillImage.transform.position = Camera.main.WorldToScreenPoint(
            targetObject.transform.position
        );

        Renderer targetRenderer = targetObject.transform.GetComponent<Renderer>();
        float targetObjectWidth = 2 * targetRenderer.bounds.extents.x;
        float targetObjectHeight = 2 * targetRenderer.bounds.extents.y;

        // length of 1 unity world unit in this screen space
        float scalingFactor = Vector3.Distance(
            Camera.main.WorldToScreenPoint(new Vector3(0, 0, 0)),
            Camera.main.WorldToScreenPoint(new Vector3(1, 0, 0))
        );
        float requiredWidth = (targetObjectWidth * scalingFactor) / fillImage.canvas.scaleFactor;
        float requiredHeight = (targetObjectHeight * scalingFactor) / fillImage.canvas.scaleFactor;

        fillImage.rectTransform.sizeDelta = new Vector2(requiredWidth, requiredHeight);
        */
    }

    // Update is called once per frame
    void Update()
    {
        float progress = countdownController.GetProgress() / 100f;
        if (progress < 1f)
        {
            fillImage.fillAmount = progress;
        }
        else
        {
            StartCoroutine(HandleLaunchComplete());
        }
    }

    private IEnumerator HandleLaunchComplete()
    {
        yield return new WaitForSeconds(delaySeconds);
        Destroy(gameObject);
    }
}
