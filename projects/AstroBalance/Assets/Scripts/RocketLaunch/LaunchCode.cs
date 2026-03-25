using System.Collections.Generic;
using TMPro;
using Tobii.GameIntegration.Net;
using UnityEngine;

/// <summary>
/// Manages the display of a random integer that changes every second if
/// gaze is maintained.
/// </summary>
public class LaunchCode : MonoBehaviour
{
    [
        SerializeField,
        Tooltip("Set to true to substitute the mouse for the eye tracker (for debugging purposes)")
    ]
    private bool useMouseForTracker = false;

    [SerializeField, Tooltip("Time between new random numbers in seconds.")]
    private float timerDuration = 1.0F;

    [SerializeField, Tooltip("Sprites to display on the countdown.")]
    private List<Sprite> countDownSprites;

    [SerializeField, Tooltip("The capacity of the gaze buffer to use.")]
    private int gazeBufferCapacity = 100;

    [SerializeField, Tooltip("The time in seconds that the gaze should be steady for.")]
    private float gazeTime = 3.0f;

    [SerializeField, Tooltip("The tolerance in unity coordinates that gaze needs to stay within.")]
    private float gazeTolerance = 3.0f;

    [SerializeField, Tooltip("An optional status text window for debugging.")]
    private TextMeshProUGUI statusText;

    [SerializeField, Tooltip("The game object the user is supposed to look at.")]
    private GameObject targetObject;

    [
        SerializeField,
        Tooltip("Adaptive difficulty, higher numbers are more difficult"),
        Range(1, 10)
    ]
    private float adaptiveDifficulty;

    Tracker tracker;
    private float timeToSpriteChange;
    private Sprite countDownSprite;
    private GazeBuffer gazeBuffer;
    private int minDataRequired = 2; // we need at least 2 data points to calculate steadiness.
    private bool gazeIsSteady;
    public bool gazeSteady
    {
        get => gazeIsSteady;
    }
    private Vector3 startScale;

    void Start()
    {
        tracker = FindFirstObjectByType<Tracker>();
        countDownSprite = countDownSprites[Random.Range(0, countDownSprites.Count)];
        // remove the number from the list to avoid selected a repeat number next time.
        countDownSprites.Remove(countDownSprite);
        startScale = GetComponent<SpriteRenderer>().transform.localScale;
        GetComponent<SpriteRenderer>().transform.localScale = startScale / adaptiveDifficulty;
        gameObject.GetComponent<SpriteRenderer>().sprite = countDownSprite;
        timeToSpriteChange = timerDuration;
        gazeBuffer = new GazeBuffer(gazeBufferCapacity, minDataRequired);
        gazeIsSteady = false;
    }

    void Update()
    {
        GazeItem gazeItem = new GazeItem();
        if (useMouseForTracker)
        {
            var mousePos = Input.mousePosition;
            gazeItem.gazePoint.X = mousePos.x;
            gazeItem.gazePoint.Y = mousePos.y;
            gazeItem.gazePoint.TimeStampMicroSeconds = (long)(Time.timeSinceLevelLoad * 1000000);
        }
        else
        {
            gazeItem.gazePoint = tracker.getGazePoint();
            Vector2 worldGaze = tracker.ConvertGazePointToWorldCoordinates(gazeItem.gazePoint);
            gazeItem.gazePoint.X = worldGaze.x;
            gazeItem.gazePoint.Y = worldGaze.y;
        }

        gazeBuffer.addIfNew(gazeItem);
        float targetX = 0f;
        float targetY = 0f;
        if (targetObject != null)
        {
            // use centre of bounds in case the target object is not centred
            targetX = targetObject.transform.GetComponent<Renderer>().bounds.center.x;
            targetY = targetObject.transform.GetComponent<Renderer>().bounds.center.y;
            Vector2 gazeTol = new Vector2(
                targetObject.transform.GetComponent<Renderer>().bounds.extents.x,
                targetObject.transform.GetComponent<Renderer>().bounds.extents.y
            );
            gazeIsSteady = gazeBuffer.gazeSteady(
                gazeTime,
                gazeTolerance * gazeTol.magnitude,
                targetX,
                targetY
            );
        }
        else
        {
            gazeIsSteady = gazeBuffer.gazeSteady(gazeTime, gazeTolerance);
        }

        if (statusText != null)
        {
            string steadyText = gazeIsSteady ? "Gaze is steady" : "Gaze is not steady";
            statusText.text =
                "Look here -> "
                + targetX
                + ", "
                + targetY
                + "\n"
                + "Looking here -> "
                + gazeItem.gazePoint.X
                + ", "
                + gazeItem.gazePoint.Y
                + "\n"
                + steadyText;
        }

        if (timeToSpriteChange > 0)
        {
            if (gazeIsSteady)
            {
                timeToSpriteChange -= Time.deltaTime;
            }
        }
        else
        {
            timeToSpriteChange = timerDuration;

            Sprite newCountDownSprite = countDownSprites[Random.Range(0, countDownSprites.Count)];
            // remove the number from the list to avoid selected a repeat number next time.
            countDownSprites.Remove(newCountDownSprite);
            countDownSprites.Add(countDownSprite);
            countDownSprite = newCountDownSprite;
            GetComponent<SpriteRenderer>().transform.localScale = startScale / adaptiveDifficulty;
            gameObject.GetComponent<SpriteRenderer>().sprite = newCountDownSprite;
        }
    }
}
