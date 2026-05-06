/// <summary>
/// Save data for a single star map session
/// </summary>
[System.Serializable]
public class StarMapData : GameData
{
    public int trialNumber;
    public string sequenceType;
    public int sequenceLength;
    public bool? responseCorrect; // ? makes the field nullable. This will be null for an un-finished trial.
    public float? responseTimeSeconds; // ? makes the field nullable. This will be null for an un-finished trial.
    public int totalNumberTrials;
    public int maxSpan;
    public string constellationSize; // string representation of StarMapManager.ConstellationSize enum
}
