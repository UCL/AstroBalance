using System.Collections;
using TMPro;
using Tobii.GameIntegration.Net;
using UnityEngine;
using UnityEngine.UI;

public class HeadTurnArrow : MonoBehaviour
{
    [SerializeField, Tooltip("Which head rotation axis to use to fill the arrow")]
    private RotationAxis rotationAxis = RotationAxis.Yaw;

    [SerializeField, Tooltip("Starting angle for head = zero fill")]
    private int startHeadAngle = 0;

    [SerializeField, Tooltip("Ending angle for head = maximum fill")]
    private int endHeadAngle = -40;

    [SerializeField, Tooltip("Number of seconds to delay before destroying the arrow on full fill")]
    private float delaySeconds = 0;

    [SerializeField, Tooltip("Arrow label")]
    private string arrowLabel = "Turn head";

    [SerializeField, Tooltip("Arrow fill colour")]
    private Color fillColor = Color.yellow;

    private Tracker tracker;
    private TextMeshProUGUI description;
    private Image fillImage;
    private bool fillLevelLocked = false;

    /// <summary>
    /// Head rotation axis to use to fill arrow.
    /// </summary>
    private enum RotationAxis
    {
        Yaw,
        Pitch,
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tracker = FindFirstObjectByType<Tracker>();
        description = GetComponentInChildren<TextMeshProUGUI>();
        Image[] arrowImages = GetComponentsInChildren<Image>();

        foreach (Image image in arrowImages)
        {
            if (image.type == Image.Type.Filled)
            {
                fillImage = image;
                break;
            }
        }

        fillImage.color = fillColor;
        description.text = arrowLabel;
    }

    // Update is called once per frame
    void Update()
    {
        if (fillLevelLocked)
        {
            return;
        }

        Rotation headRotation = tracker.getHeadRotation();
        float currentAngle;
        if (rotationAxis == RotationAxis.Yaw)
        {
            currentAngle = headRotation.YawDegrees;
        }
        else
        {
            currentAngle = headRotation.PitchDegrees;
        }

        SetArrowFill(currentAngle);
    }

    /// <summary>
    /// Set extent of arrow fill, based on how far the current angle is between the
    /// start and end angle.
    /// </summary>
    /// <param name="currentAngle">Current head angle in degrees</param>
    private void SetArrowFill(float currentAngle)
    {
        float fillFraction = (currentAngle - startHeadAngle) / (endHeadAngle - startHeadAngle);
        if (fillFraction < 0)
        {
            fillFraction = 0;
        }
        else if (fillFraction > 1)
        {
            fillFraction = 1;
        }

        fillImage.fillAmount = fillFraction;

        if (fillFraction == 1)
        {
            fillLevelLocked = true;
            StartCoroutine(HandleFilledArrow());
        }
    }

    private IEnumerator HandleFilledArrow()
    {
        yield return new WaitForSeconds(delaySeconds);
        Destroy(gameObject);
    }
}
