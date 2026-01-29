

/// <summary>
/// Save data for a single star map session
/// </summary>
[System.Serializable]
public class StarMapData : GameData
{
    public int nSequencesRepeated;
    public int maxSequenceLength;
    public string repeatOrder;  // string representation of StarMapManager.RepeatOrder enum
}
