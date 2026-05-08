using UnityEngine;

public static class MathsUtilities
{
    public static int RoundToNearestInt(float number)
    {
        return Mathf.FloorToInt(number + 0.5f);
    }

    public static float RoundTo2DecimalPlaces(float number)
    {
        return (float)Mathf.FloorToInt((number * 100) + 0.5f) / 100;
    }
}
