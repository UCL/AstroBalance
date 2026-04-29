using UnityEngine;

public class CaptureSessionData : MonoBehaviour
{
    private string saveFilename = "sessionSummary";

    void Start()
    {
        Debug.Log("running session start");
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
            sessionData.Overwrite(lastSession);
        }
    }
}
