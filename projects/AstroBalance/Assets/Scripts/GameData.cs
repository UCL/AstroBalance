using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor.PackageManager.UI;
using static UnityEngine.Rendering.DebugUI;

/// <summary>
/// Base class for a single game's save data.
/// All values that need to be recorded across ALL mini-games should go here.
/// </summary>
[System.Serializable]
public abstract class GameData
{
    public string date;
    public string startTime;
    public string endTime;
    public bool gameCompleted = false;

    public GameData()
    {
        date = DateTime.Now.ToString("yyyy-MM-dd");
        startTime = DateTime.Now.ToString("HH:mm:ss");
    }

    public GameData(Dictionary<string, string> headerToValue)
    {
        date = headerToValue["date"];
        startTime = headerToValue["startTime"];
        endTime = headerToValue["endTime"];
        gameCompleted = bool.Parse(headerToValue["gameCompleted"]);
    }

    //public GameData(string csvHeader, string csvRow)
    //{
    //    string[] fieldNames = csvHeader.Split(',');
    //    string[] csvValues = csvRow.Split(",");

    //    Type type = this.GetType();
    //    for (int i = 0; i < fieldNames.Length; i++)
    //    {
    //        FieldInfo field = type.GetField(fieldNames[i]);
    //        field.SetValue(this, (field.GetType()) csvValues[i]);
    //    }
    //}

    public void LogEndTime()
    {
        endTime = DateTime.Now.ToString("HH:mm:ss");
    }

    /// <summary>
    /// Return info on all public fields.
    /// Order is: date, startTime, endTime, gameCompleted, then any
    /// other fields in alphabetical order.
    /// </summary>
    private FieldInfo[] GetFields()
    {
        Type type = this.GetType();
        FieldInfo[] fields = type.GetFields();
        FieldInfo[] sortedFields = new FieldInfo[fields.Length];

        // To ensure they are always returned in the same order,
        // let's sort the fields.

        // We return date, startTime, endTime, gameCompleted first (as this is general data
        // for all games, and useful to have at the start of the csv)
        sortedFields[0] = type.GetField("date");
        sortedFields[1] = type.GetField("startTime");
        sortedFields[2] = type.GetField("endTime");
        sortedFields[3] = type.GetField("gameCompleted");

        // Then, all other fields sorted in alphabetical order
        Array.Sort(fields, (x, y) => String.Compare(x.Name, y.Name));

        int nextIndex = 4;
        foreach (FieldInfo field in fields)
        {
            if (!sortedFields.Contains(field))
            {
                sortedFields[nextIndex] = field;
                nextIndex++;
            }
        }

        return sortedFields;
    }

    private string ToCsvString<T>(T[] values)
    {
        StringBuilder csvString = new StringBuilder();

        for (int i = 0; i < values.Count(); i++)
        {
            csvString.Append(values[i]);
            if (i < values.Length - 1)
            {
                csvString.Append(",");
            }
        }
        return csvString.ToString();
    }

    public virtual string ToCsvHeader()
    {
        StringBuilder header = new StringBuilder();
        FieldInfo[] fields = GetFields();

        for (int i = 0; i < fields.Length; i++)
        {
            header.Append(fields[i].Name);
            if (i < fields.Length - 1)
            {
                header.Append(",");
            }
        }
        return header.ToString();
    }

    public virtual string ToCsvRow()
    {
        StringBuilder row = new StringBuilder();
        FieldInfo[] fields = GetFields();

        for (int i = 0; i < fields.Length; i++)
        {
            row.Append(fields[i].GetValue(this));
            if (i < fields.Length - 1)
            {
                row.Append(",");
            }
        }
        return row.ToString();
    }
}
