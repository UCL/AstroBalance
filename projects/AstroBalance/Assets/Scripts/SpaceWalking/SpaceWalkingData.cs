using System;
using System.Collections.Generic;

/// <summary>
/// Save data for a single space walking session
/// </summary>
[System.Serializable]
public class SpaceWalkingData : GameData
{
    public int nSteps;

    public SpaceWalkingData()
        : base() { }

    public SpaceWalkingData(Dictionary<string, string> headerToValue)
        : base(headerToValue)
    {
        nSteps = Int32.Parse(headerToValue["nSteps"]);
    }

    public override string ToCsvHeader()
    {
        string header = base.ToCsvHeader();
        return ToCsvString(new object[] { header, "nSteps" });
    }

    public override string ToCsvRow()
    {
        string row = base.ToCsvRow();
        return ToCsvString(new object[] { row, nSteps });
    }
}
