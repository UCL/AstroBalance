using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class CountdownTimer : MonoBehaviour
{
    private TextMeshProUGUI timerText;

    private int timeLimit;  // total time limit in seconds
    private float timerStart;  // time when timer was started
    private float timeRemaining; // time remaining in seconds

    private bool timerRunning = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timerText = GetComponentInChildren<TextMeshProUGUI>();
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

    public void StartCountdown(int timeLimit)
    {
        this.timeLimit = timeLimit;
        timeRemaining = timeLimit;
        UpdateTimerText(timeLimit);
        timerStart = Time.time;
        timerRunning = true;
    }

    public void StopCountdown()
    {
        UpdateTimerText(0);
        timerRunning = false;
    }

    public float GetTimeRemaining()
    {
        return timeRemaining;
    }

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
