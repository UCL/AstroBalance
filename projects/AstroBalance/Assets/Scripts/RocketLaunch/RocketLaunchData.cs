using System;
using System.Collections.Generic;

/// <summary>
/// Save data for a single rocket launch session
/// </summary>
[System.Serializable]
public class RocketLaunchData : GameData
{
    public bool pitch; // true if head pitch was used, false if yaw was used
    public int launchTimeSeconds;

    public RocketLaunchData()
        : base() { }

    public RocketLaunchData(Dictionary<string, string> headerToValue)
        : base(headerToValue)
    {
        pitch = bool.Parse(headerToValue["pitch"]);
        launchTimeSeconds = Int32.Parse(headerToValue["launchTimeSeconds"]);
    }

    public override string ToCsvHeader()
    {
        string header = base.ToCsvHeader();
        return ToCsvString(new object[] { header, "pitch", "launchTimeSeconds" });
    }

    public override string ToCsvRow()
    {
        string row = base.ToCsvRow();
        return ToCsvString(new object[] { row, pitch, launchTimeSeconds });
    }
}
