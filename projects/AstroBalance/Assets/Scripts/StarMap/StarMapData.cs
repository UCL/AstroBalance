/// <summary>
/// Save data for a single star map session
/// </summary>
[System.Serializable]
public class StarMapData : GameData
{
    public int trialNumber;
    public string sequenceType;
    public int sequenceLength;
    public bool responseCorrect;
    public float responseTimeSeconds;
    public int totalNumberTrials;
    public int maxSpan;
    public string constellationSize; // string representation of StarMapManager.ConstellationSize enum
}
