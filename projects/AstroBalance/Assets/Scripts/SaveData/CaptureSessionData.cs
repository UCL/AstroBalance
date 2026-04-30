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
        else if (lastSession.sessionEndTime != "")
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

        if (lastSession.sessionEndTime == "")
        {
            lastSession.LogEndTime();
            TimeSpan sessionDuration = DateTime
                .Parse(lastSession.sessionEndTime)
                .Subtract(DateTime.Parse(lastSession.sessionStartTime));
            lastSession.totalSessionDuration = sessionDuration.ToString(@"hh\:mm\:ss");
            sessionData.Overwrite(lastSession);
        }
    }

    /// <summary>
    /// Mark a given game as complete in this session's data
    /// </summary>
    /// <param name="gameColumn">Name of relevant column e.g. nCompleteRocketLaunchGames</param>
    public static void MarkGameAsComplete(string gameColumn)
    {
        SaveData<SessionData> sessionData = new(saveFilename);
        SessionData lastSession = sessionData.GetLast();

        FieldInfo nGamesField = lastSession.GetType().GetField(gameColumn);
        int nGames = (int)nGamesField.GetValue(lastSession);

        nGamesField.SetValue(lastSession, nGames + 1);
        lastSession.UpdateTotalCompleteGames();
        sessionData.Overwrite(lastSession);
    }
}
