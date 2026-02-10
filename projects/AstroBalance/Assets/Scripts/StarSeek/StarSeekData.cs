using System;
using System.Collections.Generic;

/// <summary>
/// Save data for a single star seek session
/// </summary>
[System.Serializable]
public class StarSeekData : GameData
{
    public int timeLimitSeconds;
    public int nStarsCollected;

    public StarSeekData()
        : base() { }

    public StarSeekData(Dictionary<string, string> headerToValue)
        : base(headerToValue)
    {
        timeLimitSeconds = Int32.Parse(headerToValue["timeLimitSeconds"]);
        nStarsCollected = Int32.Parse(headerToValue["nStarsCollected"]);
    }

    public override string ToCsvHeader()
    {
        string header = base.ToCsvHeader();
        return ToCsvString(new object[] { header, "timeLimitSeconds", "nStarsCollected" });
    }

    public override string ToCsvRow()
    {
        string row = base.ToCsvRow();
        return ToCsvString(new object[] { row, timeLimitSeconds, nStarsCollected });
    }
}
