using System;

/// <summary>
/// Base class for all save data.
/// This represents a single row in the saved csv file.
/// All values that need to be recorded in ALL save data should go here.
/// </summary>
[System.Serializable]
public abstract class Data
{
    public string date;
    public string startTime;
    public string endTime;

    public Data()
    {
        date = DateTime.Now.ToString("yyyy-MM-dd");
        startTime = DateTime.Now.ToString("HH:mm:ss");
    }

    public void LogEndTime()
    {
        endTime = DateTime.Now.ToString("HH:mm:ss");
    }
}
