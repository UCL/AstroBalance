using System.Collections.Generic;
using System.Linq;
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

    public static float StandardDeviation(List<float> numbers)
    {
        float average = numbers.Average();
        return Mathf.Sqrt(numbers.Average(v => Mathf.Pow(v - average, 2)));
    }

    public static float Median(List<float> numbers)
    {
        List<float> sortedList = new List<float>(numbers);
        sortedList.Sort();

        int size = sortedList.Count();
        int midPoint = size / 2;

        // Take middle value, or average of the two middle values if there's an even number of items
        float median =
            (size % 2 != 0)
                ? sortedList[midPoint]
                : (sortedList[midPoint] + sortedList[midPoint - 1]) / 2;

        return median;
    }
}
