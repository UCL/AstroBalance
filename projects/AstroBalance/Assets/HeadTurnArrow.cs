using Tobii.GameIntegration.Net;
using UnityEngine;
using UnityEngine.UI;

public class HeadTurnArrow : MonoBehaviour
{
    [SerializeField, Tooltip("Arrow fill image")]
    private Image fillImage;

    [SerializeField, Tooltip("Which head rotation axis to use to fill the arrow")]
    private RotationAxis rotationAxis = RotationAxis.Yaw;

    [SerializeField, Tooltip("Starting angle for head = zero fill")]
    private int startHeadAngle = 0;

    [SerializeField, Tooltip("Ending angle for head = maximum fill")]
    private int endHeadAngle = 30;

    [SerializeField, Tooltip("Arrow fill colour")]
    private Color fillColor = Color.yellow;

    private Tracker tracker;

    /// <summary>
    /// Head rotation axis to use to fill arrow.
    /// </summary>
    public enum RotationAxis
    {
        Yaw,
        Pitch,
        Roll,
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tracker = FindFirstObjectByType<Tracker>();
        fillImage.color = fillColor;
    }

    // Update is called once per frame
    void Update()
    {
        Rotation headRotation = tracker.getHeadRotation();
        float currentAngle;
        if (rotationAxis == RotationAxis.Yaw)
        {
            currentAngle = headRotation.YawDegrees;
        }
        else if (rotationAxis == RotationAxis.Pitch)
        {
            currentAngle = headRotation.PitchDegrees;
        }
        else
        {
            currentAngle = headRotation.RollDegrees;
        }

        SetArrowFill(currentAngle);
    }

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
    }
}
