using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework.Constraints;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

/// <summary>
/// Class to save / load data from multiple game sessions.
/// </summary>
/// <typeparam name="T">The type of game data (specific to each mini-game)</typeparam>
[System.Serializable]
public class SaveData<T>
    where T : GameData
{
    public bool saveFileExists = false;
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
        dataPath = Path.Combine(Application.persistentDataPath, filename + ".csv");
        CheckSaveFileExists();
    }

    /// <summary>
    /// Add data from a new game session to the save file.
    /// </summary>
    /// <param name="gameData">Game data from this session</param>
    public void SaveGameData(T gameData)
    {
        using (StreamWriter sw = new StreamWriter(dataPath, true))
        {
            if (!saveFileExists)
            {
                sw.WriteLine(gameData.ToCsvHeader());
                saveFileExists = true;
            }

            sw.WriteLine(gameData.ToCsvRow());
        }
    }

    /// <summary>
    /// Get data from the last complete played game session.
    /// </summary>
    public T GetLastCompleteGameData()
    {
        if (!saveFileExists)
        {
            return null;
        }

        return savedGames.LastOrDefault();
    }

    /// <summary>
    /// Get data from the last n complete played games.
    /// </summary>
    /// <param name="nGames">Number of games to retrieve</param>
    public IEnumerable<T> GetLastNCompleteGamesData(int nGames)
    {
        return savedGames.TakeLast(nGames);
    }

    //public void Save()
    //{
    //    string json = JsonUtility.ToJson(this, true);
    //    File.WriteAllText(dataPath, json);
    //}

    private void CheckSaveFileExists()
    {
        if (File.Exists(dataPath))
        {
            saveFileExists = true;
        }
        else
        {
            saveFileExists = false;
        }
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
