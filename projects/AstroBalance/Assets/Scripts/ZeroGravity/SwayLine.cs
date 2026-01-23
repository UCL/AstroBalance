using Tobii.GameIntegration.Net;
using UnityEngine;

public class SwayLine : MonoBehaviour
{
    [SerializeField, Tooltip("Head x movement scaling (moving 1mm, moves this many unity units)")]
    private float headXScaling = 0.01f;
    [SerializeField, Tooltip("Head x movement tolerance (mm) - beyond this limit scoring stops")]
    private float headXTolerance = 100;
    [SerializeField, Tooltip("Inside x range colour")]
    private Color inRangeColor = Color.white;
    [SerializeField, Tooltip("Outside x range colour")]
    private Color outRangeColor = Color.black;

    private Tracker tracker;
    private SpriteRenderer spriteRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tracker = FindFirstObjectByType<Tracker>();   
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        HeadPose currentHeadPose = tracker.getHeadPose();
        float xPosMm = currentHeadPose.Position.X;
        float rollDegrees = currentHeadPose.Rotation.RollDegrees;

        if (Mathf.Abs(xPosMm) >= headXTolerance)
        {
            spriteRenderer.color = outRangeColor;
        } else
        {
            spriteRenderer.color = inRangeColor;
        }
        
        // We only move the sway line on the x axis - left/right (we don't care about
        // changes in head height, or distance from screen)
        transform.position = new Vector3(xPosMm * headXScaling, transform.position.y, 0);

        // We rotate only with head roll
        transform.eulerAngles = new Vector3(0, 0, -rollDegrees);
    }
}
