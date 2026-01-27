using System.IO;
using UnityEngine;

public class RocketLaunchData
{
    // currently defaults to "C:\Users\username\AppData\LocalLow\DefaultCompany\AstroBalance" on Windows
    // /home/username/.config/unity3d/DefaultCompany/AstroBalance/ on Linux
    private string dataPath = Path.Combine(
        Application.persistentDataPath,
        "RocketLaunchScores.json"
    );

    [System.Serializable]
    public class SaveData
    {
        public bool pitch; // true if head pitch was used, false if yaw was used
    }

    /// <summary>
    /// Save information about a Rocket Launch game.
    /// </summary>
    /// <param name="pitch">if pitch was used for speed control</param>
    public void Save(bool pitch)
    {
        SaveData data = new SaveData { pitch = pitch };
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(dataPath, json);
    }

    /// <summary>
    /// Load data about the last Rocket Launch game.
    /// </summary>
    /// <returns>SaveData or null (if no data available)</returns>
    public SaveData Load()
    {
        SaveData data;

        if (File.Exists(dataPath))
        {
            string json = File.ReadAllText(dataPath);
            data = JsonUtility.FromJson<SaveData>(json);
        }
        else
        {
            data = null;
        }

        return data;
    }
}
