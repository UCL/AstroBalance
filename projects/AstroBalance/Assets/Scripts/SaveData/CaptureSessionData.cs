using System;
using System.Reflection;
using UnityEngine;

public class CaptureSessionData : MonoBehaviour
{
    private static readonly string saveFilename = "sessionSummary";

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
            lastSession.totalSessionDurationMinutes = sessionDuration.Minutes;
            sessionData.Overwrite(lastSession);
        }
    }

    /// <summary>
    /// Mark a given game as played in this session's data
    /// </summary>
    /// <param name="gameName">Game name in camel case e.g. starMap</param>
    public static void MarkGameAsPlayed(string gameName)
    {
        SaveData<SessionData> sessionData = new(saveFilename);
        SessionData lastSession = sessionData.GetLast();

        FieldInfo gamePlayedField = lastSession.GetType().GetField(gameName + "Played");
        bool gamePlayed = (bool)gamePlayedField.GetValue(lastSession);

        if (gamePlayed == false)
        {
            gamePlayedField.SetValue(lastSession, true);
            lastSession.totalGamesPlayed += 1;
            sessionData.Overwrite(lastSession);
        }
    }
}
