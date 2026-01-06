using System.IO;
using UnityEngine;

public class StarCollectorData
{

    private string dataPath = Path.Combine(Application.persistentDataPath, "StarCollectorScores.json");

    [System.Serializable]
    public class SaveData
    {
        public int timeLimit;
        public int score;
        public float percentCollected;
    }

    /// <summary>
    /// Save information about a Star Collector game.
    /// </summary>
    /// <param name="timeLimit">time limit in seconds</param>
    /// <param name="score">total score (number of stars collected)</param>
    /// <param name="percentCollected">total percent of stars collected</param>
    public void Save(int timeLimit, int score, float percentCollected)
    {
        SaveData data = new SaveData
        {
            timeLimit = timeLimit,
            score = score,
            percentCollected = percentCollected
        };

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(dataPath, json);
    }

    /// <summary>
    /// Load data about the last Star Collector game.
    /// </summary>
    /// <returns>SaveData or null (if no data available)</returns>
    public SaveData Load()
    {
        SaveData data;

        if (File.Exists(dataPath))
        {
            string json = File.ReadAllText(dataPath);
            data = JsonUtility.FromJson<SaveData>(json);
        } else
        {
            data = null;
        }

        return data;
    }
}
