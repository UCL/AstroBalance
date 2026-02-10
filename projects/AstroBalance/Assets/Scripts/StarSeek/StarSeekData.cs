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

    //public override string ToCsvHeader()
    //{
    //    string csvHeader = base.ToCsvHeader();
    //    csvHeader += "timeLimitSeconds,nStarsCollected";
    //    return csvHeader;
    //}
}
