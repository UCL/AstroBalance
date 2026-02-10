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

    [
        SerializeField,
        Tooltip("Number of seconds to delay after arrow is full, before switching arrow direction")
    ]
    private float delaySeconds = 0;

    [SerializeField, Tooltip("Number of arrows to fill, before it is destroyed.")]
    private float nArrowsToFill = 2;

    [SerializeField, Tooltip("Outward movement label")]
    private string outLabel = "Turn head";

    [SerializeField, Tooltip("Back to centre movement label")]
    private string backLabel = "Back to centre";

    [SerializeField, Tooltip("Arrow fill colour")]
    private Color fillColor = Color.yellow;

    private Tracker tracker;
    private TextMeshProUGUI description;
    private Image[] arrowImages;
    private Image fillImage;
    private int nArrowsFilled = 0; // Number of times the arrow has been filled
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
        arrowImages = GetComponentsInChildren<Image>();

        foreach (Image image in arrowImages)
        {
            if (image.type == Image.Type.Filled)
            {
                fillImage = image;
                break;
            }
        }

        fillImage.color = fillColor;
        description.text = outLabel;
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
        nArrowsFilled++;
        yield return new WaitForSeconds(delaySeconds);

        if (nArrowsFilled >= nArrowsToFill)
        {
            Destroy(gameObject);
        }
        else
        {
            SwapDirection();
            fillLevelLocked = false;
        }
    }

    /// <summary>
    /// Flip the direction of the arrow by 180 degrees, and swap the fill head angles.
    /// E.g. if an arrow starts pointing left and fills from 0 to -30 degrees,
    /// calling this function will make it point right and fill from -30 to 0 degreees.
    /// </summary>
    private void SwapDirection()
    {
        // Rotate arrow images 180 degrees
        foreach (Image image in arrowImages)
        {
            image.transform.Rotate(new Vector3(0, 0, 180));
        }

        // Swap start / end angles, so arrow fills with opposite
        // head rotation
        int currentStartAngle = startHeadAngle;
        int currentEndAngle = endHeadAngle;
        startHeadAngle = currentEndAngle;
        endHeadAngle = currentStartAngle;

        // Swap description text
        if (description.text == outLabel)
        {
            description.text = backLabel;
        }
        else
        {
            description.text = outLabel;
        }
    }
}
