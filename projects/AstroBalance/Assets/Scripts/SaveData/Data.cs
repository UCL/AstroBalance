using System;

/// <summary>
/// Base class for a save data item (i.e. a single row in the csv file).
/// All values that need to be recorded across ALL save data should go here.
/// </summary>
[System.Serializable]
public abstract class Data
{
    public int sessionNumber = 1;
    public string sessionDate;
    public string sessionStartTime;
    public string sessionEndTime;

    public Data()
    {
        sessionDate = DateTime.Now.ToString("yyyy-MM-dd");
        sessionStartTime = DateTime.Now.ToString("HH:mm:ss");
    }

    public void LogEndTime()
    {
        sessionEndTime = DateTime.Now.ToString("HH:mm:ss");
    }
}
