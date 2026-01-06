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
