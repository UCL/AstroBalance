using System;
using System.Collections.Generic;

/// <summary>
/// Save data for a single star map session
/// </summary>
[System.Serializable]
public class StarMapData : GameData
{
    public int nSequencesRepeated;
    public int maxSequenceLength;
    public string repeatOrder; // string representation of StarMapManager.RepeatOrder enum
    public string constellationSize; // string representation of StarMapManager.ConstellationSize enum

    public StarMapData()
        : base() { }

    public StarMapData(Dictionary<string, string> headerToValue)
        : base(headerToValue)
    {
        nSequencesRepeated = Int32.Parse(headerToValue["nSequencesRepeated"]);
        maxSequenceLength = Int32.Parse(headerToValue["maxSequenceLength"]);
        repeatOrder = headerToValue["repeatOrder"];
        constellationSize = headerToValue["constellationSize"];
    }

    public override string ToCsvHeader()
    {
        string header = base.ToCsvHeader();
        return ToCsvString(
            new object[]
            {
                header,
                "nSequencesRepeated",
                "maxSequenceLength",
                "repeatOrder",
                "constellationSize",
            }
        );
    }

    public override string ToCsvRow()
    {
        string row = base.ToCsvRow();
        return ToCsvString(
            new object[]
            {
                row,
                nSequencesRepeated,
                maxSequenceLength,
                repeatOrder,
                constellationSize,
            }
        );
    }
}
