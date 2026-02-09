using System;
using System.Linq;
using System.Reflection;
using System.Text;

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

    public void LogEndTime()
    {
        endTime = DateTime.Now.ToString("HH:mm:ss");
    }

    private FieldInfo[] GetFields()
    {
        Type type = this.GetType();
        FieldInfo[] fields = type.GetFields();
        FieldInfo[] sortedFields = new FieldInfo[fields.Length];

        // To ensure they are always returned in the same order,
        // let's sort the fields.
        // We return date, startTime, endTime, gameCompleted first (as this is general data
        // for all games, and useful to have at the start of the csv)
        // then any other fields in alphabetical order
        sortedFields[0] = type.GetField("date");
        sortedFields[1] = type.GetField("startTime");
        sortedFields[2] = type.GetField("endTime");
        sortedFields[3] = type.GetField("gameCompleted");

        Array.Sort(fields, (x, y) => String.Compare(x.Name, y.Name));

        return fields;
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

        return "date,startTime,endTime,gameCompleted";
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

        return $"{date},{startTime},{endTime},{gameCompleted}";
    }
}
