using System;

/// <summary>
/// Base class for a single game's save data.
/// All values that need to be recorded across ALL mini-games should go here.
/// </summary>
[System.Serializable]
public abstract class GameData
{
    public string date;
    public string startTime;
    public string endTime;
    public bool gameCompleted = false;

    public GameData()
    {
        date = DateTime.Now.ToString("yyyy-MM-dd");
        startTime = DateTime.Now.ToString("HH:mm:ss");
    }

    public void LogEndTime()
    {
        endTime = DateTime.Now.ToString("HH:mm:ss");
    }
}
