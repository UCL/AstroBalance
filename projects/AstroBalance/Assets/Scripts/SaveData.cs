

using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;

/// <summary>
/// Class to save / load data from multiple game sessions.
/// </summary>
/// <typeparam name="T">The type of game data (specific to each mini-game)</typeparam>
[System.Serializable]
public class SaveData<T> where T : GameData
{
    public List<T> savedGames = new List<T>();
    private string dataPath;

    public SaveData(string filename)
    {
        // currently defaults to "C:\Users\username\AppData\LocalLow\DefaultCompany\AstroBalance" on Windows
        dataPath = Path.Combine(
            Application.persistentDataPath,
            filename + ".json"
        );

        Load();
    }

    public void AddGameData(T gameData)
    {
        savedGames.Add( gameData );
    }

    public T GetLastGameData()
    {
        return savedGames.LastOrDefault();
    }

    public void Save()
    {
        string json = JsonUtility.ToJson(this);
        File.WriteAllText(dataPath, json);
    }

    /// <summary>
    /// Load data about previous games from file (if any).
    /// </summary>
    private void Load()
    {
        if (File.Exists(dataPath))
        {
            string json = File.ReadAllText(dataPath);
            SaveData<T> loadedData = JsonUtility.FromJson<SaveData<T>>(json);

            if (loadedData.savedGames != null)
            {
                this.savedGames = loadedData.savedGames;
            }
        }
    }

}
