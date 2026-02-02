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

    Tracker tracker;
    private float timeToSpriteChange;
    private Sprite countDownSprite;
    private GazeBuffer gazeBuffer;

    void Start()
    {
        tracker = FindFirstObjectByType<Tracker>();
        countDownSprite = countDownSprites[Random.Range(0, countDownSprites.Count)];
        // remove the number from the list to avoid selected a repeat number next time.
        countDownSprites.Remove(countDownSprite);
        gameObject.GetComponent<SpriteRenderer>().sprite = countDownSprite;
        timeToSpriteChange = timerDuration;
        gazeBuffer = new GazeBuffer(gazeBufferCapacity);
    }

    void Update()
    {
        RocketGazePoint gazePoint = new RocketGazePoint();
        if (useMouseForTracker)
        {
            var mousePos = Input.mousePosition;
            gazePoint.gazePoint.X = mousePos.x;
            gazePoint.gazePoint.Y = mousePos.y;
            gazePoint.gazePoint.TimeStampMicroSeconds = (long)(Time.timeSinceLevelLoad * 1000000);
        }
        else
        {
            gazePoint.gazePoint = tracker.getGazePoint();
            Vector2 worldGaze = tracker.ConvertGazePointToWorldCoordinates(gazePoint.gazePoint);
            gazePoint.gazePoint.X = worldGaze.x;
            gazePoint.gazePoint.Y = worldGaze.y;
        }

        gazeBuffer.addIfNew(gazePoint);

        bool gazeIsSteady = false;
        GazePoint targetPoint = new GazePoint();
        if (targetObject != null)
        {
            targetPoint.X = targetObject.transform.position.x;
            targetPoint.Y = targetObject.transform.position.y;
            gazeIsSteady = gazeBuffer.gazeSteady(gazeTime, gazeTolerance, targetPoint);
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
                + targetPoint.X
                + ", "
                + targetPoint.Y
                + "\n"
                + "Looking here -> "
                + gazePoint.gazePoint.X
                + ", "
                + gazePoint.gazePoint.Y
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
            gameObject.GetComponent<SpriteRenderer>().sprite = newCountDownSprite;
        }
    }
}
