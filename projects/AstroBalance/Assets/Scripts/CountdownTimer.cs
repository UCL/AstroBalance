using TMPro;
using UnityEngine;

public class CountdownTimer : MonoBehaviour
{
    [SerializeField, Tooltip("Format of time text")]
    private TimeFormat timeFormat = TimeFormat.MinutesSeconds;

    private TextMeshProUGUI timerText;
    private int timeLimit;  // total time limit in seconds
    private float timerStart;  // time when timer was started
    private float timeRemaining; // time remaining in seconds
    private bool timerRunning = false;

    private enum TimeFormat
    {
        MinutesSeconds,
        Seconds,    
    }

    private void Awake()
    {
        timerText = GetComponentInChildren<TextMeshProUGUI>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (!timerRunning)
        {
            return;
        }

        float elaspedTime = Time.time - timerStart;
        timeRemaining = timeLimit - elaspedTime;

        if (timeRemaining <= 0)
        {
            StopCountdown();
        }
        else
        {
            UpdateTimerText(timeRemaining);
        }
    }

    /// <summary>
    /// Start a countdown from the given number of seconds
    /// </summary>
    /// <param name="seconds">Max time in seconds</param>
    public void StartCountdown(int seconds)
    {
        timeLimit = seconds;
        timeRemaining = seconds;
        UpdateTimerText(seconds);
        timerStart = Time.time;
        timerRunning = true;
    }

    /// <summary>
    /// Stop the countdown, and set the timer to zero.
    /// </summary>
    public void StopCountdown()
    {
        UpdateTimerText(0);
        timerRunning = false;
    }

    /// <summary>
    /// Get remaining time in seconds
    /// </summary>
    public float GetTimeRemaining()
    {
        return timeRemaining;
    }

    /// <summary>
    /// Get total time limit or current countdown.
    /// </summary>
    public float GetTimeLimit()
    {
        return timeLimit;
    }

    /// <summary>
    /// Get the time when the timer was started
    /// </summary>
    public float GetTimerStart()
    {
        return timerStart;
    }

    private void UpdateTimerText(float secondsLeft)
    {
        if (secondsLeft < 0)
        {
            secondsLeft = 0;
        }

        if (timeFormat == TimeFormat.Seconds)
        {
            timerText.text = Mathf.CeilToInt(secondsLeft).ToString();
            return;
        }

        float minutes = Mathf.FloorToInt(secondsLeft / 60);
        float seconds = Mathf.CeilToInt(secondsLeft % 60);

        // If there's e.g. 59.5 seconds left, we want to display 1:00
        if (seconds == 60)
        {
            seconds = 0;
            minutes += 1;
        }

        timerText.text = $"{minutes:00}:{seconds:00}";
    }

}
