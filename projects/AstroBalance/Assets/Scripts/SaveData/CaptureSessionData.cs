using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class CaptureSessionData
{
    private static readonly string saveFilename = "SessionSummary";

    /// <summary>
    /// Setup capturing session info on application start and quit
    /// </summary>
    [RuntimeInitializeOnLoadMethod]
    private static void SetupSessionCapture()
    {
        OnApplicationStart();
        Application.quitting += OnApplicationQuit;
    }

    /// <summary>
    /// Create a new SessionData entry when the application starts.
    /// </summary>
    private static void OnApplicationStart()
    {
        SaveData<SessionData> sessionData = new(saveFilename);
        SessionData newSession = new SessionData();
        newSession.sessionNumber = sessionData.GetNextSessionNumber();
        sessionData.Save(newSession);
    }

    /// <summary>
    /// Record the session end time / duration when the application is closed.
    /// </summary>
    private static void OnApplicationQuit()
    {
        SaveData<SessionData> sessionData = new(saveFilename);
        SessionData lastSession = sessionData.GetLast();

        lastSession.LogEndTime();
        TimeSpan sessionDuration = DateTime
            .Parse(lastSession.endTime)
            .Subtract(DateTime.Parse(lastSession.startTime));
        lastSession.totalSessionDuration = sessionDuration.ToString(@"hh\:mm\:ss");
        sessionData.Overwrite(lastSession);
    }

    /// <summary>
    /// Mark a given game as complete in this session's data
    /// </summary>
    /// <param name="gameColumn">Name of relevant column e.g. nCompleteRocketLaunchGames</param>
    public static void MarkGameAsComplete(string gameColumn)
    {
        SaveData<SessionData> sessionData = new(saveFilename);
        SessionData lastSession = sessionData.GetLast();

        if (lastSession is null)
        {
            return;
        }

        FieldInfo nGamesField = lastSession.GetType().GetField(gameColumn);
        int nGames = (int)nGamesField.GetValue(lastSession);

        nGamesField.SetValue(lastSession, nGames + 1);
        lastSession.UpdateTotalCompleteGames();
        sessionData.Overwrite(lastSession);
    }

    /// <summary>
    /// Get number of current session (last row in session save data)
    /// </summary>
    public static int CurrentSessionNumber()
    {
        SaveData<SessionData> sessionData = new(saveFilename);
        return sessionData.GetCurrentSessionNumber();
    }

    /// <summary>
    /// Create a summary of all sessions so far.
    /// This includes the overall total number of complete games played per mini-game.
    /// </summary>
    public static SummaryData SummaryOfAllSessions()
    {
        SummaryData summary = new SummaryData();
        SaveData<SessionData> sessionData = new(saveFilename);

        IEnumerable<SessionData> allSessions = sessionData.GetAll();
        if (allSessions.Count() == 0)
        {
            return summary;
        }

        foreach (SessionData session in allSessions)
        {
            summary.nCompleteRocketLaunchGames += session.nCompleteRocketLaunchGames;
            summary.nCompleteStarCollectorGames += session.nCompleteStarCollectorGames;
            summary.nCompleteStarSeekGames += session.nCompleteStarSeekGames;
            summary.nCompleteStarMapGames += session.nCompleteStarMapGames;
            summary.nCompleteSpaceWalkGames += session.nCompleteSpaceWalkGames;
            summary.nCompleteZeroGravityGames += session.nCompleteZeroGravityGames;
        }

        return summary;
    }
}
