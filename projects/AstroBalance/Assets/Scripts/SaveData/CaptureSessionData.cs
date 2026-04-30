using System;
using System.Reflection;
using UnityEngine;

public class CaptureSessionData : MonoBehaviour
{
    private static readonly string saveFilename = "SessionSummary";

    /// <summary>
    /// Create a new SessionData entry when the application is opened.
    /// </summary>
    void Start()
    {
        SaveData<SessionData> sessionData = new(saveFilename);
        SessionData lastSession = sessionData.GetLast();

        if (lastSession is null)
        {
            // There is no summary data yet
            SessionData newSession = new SessionData();
            newSession.sessionNumber = 1;
            sessionData.Save(newSession);
        }
        else if (lastSession.endTime != "")
        {
            // The last session has ended, so create a new one
            SessionData newSession = new SessionData();
            newSession.sessionNumber = lastSession.sessionNumber + 1;
            sessionData.Save(newSession);
        }
    }

    /// <summary>
    /// Record the session end time / duration when the application is closed.
    /// </summary>
    private void OnApplicationQuit()
    {
        SaveData<SessionData> sessionData = new(saveFilename);
        SessionData lastSession = sessionData.GetLast();

        if (lastSession.endTime == "")
        {
            lastSession.LogEndTime();
            TimeSpan sessionDuration = DateTime
                .Parse(lastSession.endTime)
                .Subtract(DateTime.Parse(lastSession.startTime));
            lastSession.totalSessionDuration = sessionDuration.ToString(@"hh\:mm\:ss");
            sessionData.Overwrite(lastSession);
        }
    }

    /// <summary>
    /// Mark a given game as played in this session's data
    /// </summary>
    /// <param name="gameColumn">Name of relevant column e.g. game1RocketLaunchPlayed</param>
    public static void MarkGameAsPlayed(string gameColumn)
    {
        SaveData<SessionData> sessionData = new(saveFilename);
        SessionData lastSession = sessionData.GetLast();

        FieldInfo gamePlayedField = lastSession.GetType().GetField(gameColumn);
        bool gamePlayed = (bool)gamePlayedField.GetValue(lastSession);

        if (gamePlayed == false)
        {
            gamePlayedField.SetValue(lastSession, true);
            lastSession.UpdateTotalGamesPlayed();
            sessionData.Overwrite(lastSession);
        }
    }
}
