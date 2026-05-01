/// <summary>
/// Save data for a single star map session
/// </summary>
[System.Serializable]
public class StarMapData : GameData
{
    public int trialNumber;
    public string repeatOrder; // string representation of StarMapManager.RepeatOrder enum
    public int sequenceLength;
    public bool responseCorrect;
    public float responseTimeSeconds;
    public int nSequencesRepeated;
    public int maxSequenceLength;
    public string constellationSize; // string representation of StarMapManager.ConstellationSize enum
}
