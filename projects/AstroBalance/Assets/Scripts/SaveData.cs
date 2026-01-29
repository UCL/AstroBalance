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

    /// <summary>
    /// Create a new Save Data collection. If available, this will
    /// be populated with previous save data from filename.json
    /// </summary>
    /// <param name="filename">The filename of the save file</param>
    public SaveData(string filename)
    {
        // currently defaults to "C:\Users\username\AppData\LocalLow\DefaultCompany\AstroBalance" on Windows
        dataPath = Path.Combine(
            Application.persistentDataPath,
            filename + ".json"
        );

        Load();
    }

    /// <summary>
    /// Add data from a new game session to the save file.
    /// </summary>
    /// <param name="gameData">Game data from this session</param>
    public void SaveGameData(T gameData)
    {
        savedGames.Add( gameData );
        Save();
    }
    
    /// <summary>
    /// Get data from the last played game session.
    /// </summary>
    public T GetLastGameData()
    {
        return savedGames.LastOrDefault();
    }

    public void Save()
    {
        string json = JsonUtility.ToJson(this, true);
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
