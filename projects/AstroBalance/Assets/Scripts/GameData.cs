using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

    public GameData(Dictionary<string, string> headerToValue)
    {
        date = headerToValue["date"];
        startTime = headerToValue["startTime"];
        endTime = headerToValue["endTime"];
        gameCompleted = bool.Parse(headerToValue["gameCompleted"]);
    }

    public void LogEndTime()
    {
        endTime = DateTime.Now.ToString("HH:mm:ss");
    }

    public virtual string ToCsvHeader()
    {
        return ToCsvString(new object[] { "date", "startTime", "endTime", "gameCompleted" });
    }

    public virtual string ToCsvRow()
    {
        return ToCsvString(new object[] { date, startTime, endTime, gameCompleted });
    }

    protected string ToCsvString(object[] values)
    {
        StringBuilder csvString = new StringBuilder();

        for (int i = 0; i < values.Count(); i++)
        {
            csvString.Append(values[i]);
            if (i < values.Length - 1)
            {
                csvString.Append(",");
            }
        }
        return csvString.ToString();
    }
}
