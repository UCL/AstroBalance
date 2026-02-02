using System;
using System.Linq;
using Tobii.GameIntegration.Net;
using UnityEngine;

public interface ITimeStampMicroSeconds
{
    long TimeStampMicroSeconds();
}

public interface IBufferData
{
    float[] GetData();
}

public class RocketGazePoint : ITimeStampMicroSeconds, IBufferData
{
    public GazePoint gazePoint;

    public long TimeStampMicroSeconds() => gazePoint.TimeStampMicroSeconds;

    public float[] GetData() => new float[] { gazePoint.X, gazePoint.Y };
}

public class RocketHeadPitch : ITimeStampMicroSeconds, IBufferData
{
    public HeadPose headPose;

    public long TimeStampMicroSeconds() => headPose.TimeStampMicroSeconds;

    public float[] GetData() =>
        new float[] { (float)headPose.TimeStampMicroSeconds, headPose.Rotation.PitchDegrees };

    public RocketHeadPitch(HeadPose headPose)
    {
        this.headPose = headPose;
    }
}

public class RocketHeadYaw : ITimeStampMicroSeconds, IBufferData
{
    public HeadPose headPose;

    public long TimeStampMicroSeconds() => headPose.TimeStampMicroSeconds;

    public float[] GetData() =>
        new float[] { (float)headPose.TimeStampMicroSeconds, headPose.Rotation.YawDegrees };

    public RocketHeadYaw(HeadPose headPose)
    {
        this.headPose = headPose;
    }
}

public class TobiiBuffer<T>
    where T : ITimeStampMicroSeconds, IBufferData
{
    protected int lastAddedIndex;
    private bool hasData; // flag to indicate if the buffer has any data
    protected bool hasEnoughData; // flag to indicate if the buffer has enough data to calculate speed or steadiness.
    private int minDataRequired;
    protected T[] buffer;

    public TobiiBuffer(int capacity, int minDataRequired)
    {
        buffer = new T[capacity];
        lastAddedIndex = -1;
        hasData = false;
        hasEnoughData = false;
        this.minDataRequired = minDataRequired;
    }

    /// <summary>
    /// Adds a new item to the buffer if it has a different timestamp to the last added item.
    /// returns true if the point was added, false otherwise.
    /// </summary>
    public bool addIfNew(T item)
    {
        if (
            !hasData
            || item.TimeStampMicroSeconds() != buffer[lastAddedIndex].TimeStampMicroSeconds()
        )
        {
            hasData = true;
            int newIndex = lastAddedIndex + 1;
            if (newIndex >= minDataRequired)
            {
                hasEnoughData = true;
            }
            if (newIndex >= buffer.Length)
            {
                newIndex = 0;
            }
            buffer[newIndex] = item;
            lastAddedIndex = newIndex;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Calculates the average speed of the head pose buffer over a given time period.
    /// This presumes that the GetData interface of the templated object returns a
    /// float vector of the form [time, position].
    /// </summary>
    /// <param name="speedTime">The time period in seconds over which to calculate the average speed.</param>
    /// <returns>The average speed of the head pose buffer over the given time period.</returns>
    public float getSpeed(float speedTime)
    {
        float averageSpeed = 0f;
        if (!hasEnoughData)
            return averageSpeed;

        int timeInMicroseconds = (int)(speedTime * 1e6);
        CopyToTwoArrays(
            timeInMicroseconds,
            out float[] timeStampMicroSecondsArray,
            out float[] posArray
        );

        return calculateAverageSpeed(timeStampMicroSecondsArray, posArray);
    }

    private float calculateAverageSpeed(float[] timeStampMicroSecondsArray, float[] posArray)
    {
        if (posArray.Length < 2)
        {
            return 0f;
        }
        float totalDistance = 0f;
        for (int i = 0; i < posArray.Length - 1; i++)
        {
            totalDistance += Math.Abs(posArray[i + 1] - posArray[i]);
        }

        double totalTime =
            (timeStampMicroSecondsArray[0] - timeStampMicroSecondsArray[posArray.Length - 1]) / 1e6;
        float averageSpeed = (float)(totalDistance / totalTime);

        return averageSpeed;
    }

    ///<summary>
    /// Copies items from the Buffer to two arrays if their timestamp is newer
    /// than the maximumAge
    /// The contents of each array are determined by the templated
    /// object's GetData interface.
    ///</summary>
    /// <param name="maximumAge">The maximum age (in microseconds) of the data to copy.</param>
    /// <param name="array_0">The first array to fill</param>
    /// <param name="array_1">The second array to fill</param>
    protected void CopyToTwoArrays(long maximumAge, out float[] array_0, out float[] array_1)
    {
        if (!hasData)
        {
            array_0 = new float[0];
            array_1 = new float[0];
            return;
        }

        int arrayIndex = 0;
        int bufferIndex = lastAddedIndex;
        array_0 = new float[buffer.Length];
        array_1 = new float[buffer.Length];
        array_0[arrayIndex] = buffer[bufferIndex].GetData()[0];
        array_1[arrayIndex] = buffer[bufferIndex].GetData()[1];
        long oldestAllowableTime = buffer[bufferIndex].TimeStampMicroSeconds() - maximumAge;

        bufferIndex = bufferIndex > 0 ? bufferIndex - 1 : buffer.Length - 1;
        arrayIndex++;

        while (
            bufferIndex != lastAddedIndex
            && buffer[bufferIndex].TimeStampMicroSeconds() >= oldestAllowableTime
        )
        {
            array_0[arrayIndex] = buffer[bufferIndex].GetData()[0];
            array_1[arrayIndex] = buffer[bufferIndex].GetData()[1];
            bufferIndex = bufferIndex > 0 ? bufferIndex - 1 : buffer.Length - 1;
            arrayIndex++;
        }

        Array.Resize(ref array_0, arrayIndex);
        Array.Resize(ref array_1, arrayIndex);
    }
}

/// <summary>
/// Holds the gazepoint buffer and provide a method to check gaze stability and direction.
/// </summary>
/// TODO Understand how to make this thread safe.
public class GazeBuffer : TobiiBuffer<RocketGazePoint>
{
    public GazeBuffer(int capacity)
        : base(capacity, 2) { }

    /// <summary>
    /// returns true if the gaze points more recent than the time have a summed
    /// square distance from the target point less than the tolerance.
    /// </summary>
    /// <param name="time">in seconds to sample over</param>
    /// <param name="tolerance">the allowable range</param>
    /// <param name="targetGazePoint">the target gaze point</param>
    public bool gazeSteady(float time, float tolerance, GazePoint targetGazePoint)
    {
        if (!hasEnoughData)
            return false;
        int timeInMicroseconds = (int)(time * 1e6);
        CopyToTwoArrays(timeInMicroseconds, out float[] x_array, out float[] y_array);
        return gazeSteadyImpl(x_array, y_array, targetGazePoint.X, targetGazePoint.Y, tolerance);
    }

    /// <summary>
    /// returns true if the gaze points more recent than the time have a standard deviation
    /// less than the tolerance.
    /// </summary>
    /// <param name="time">in seconds to sample over</param>
    /// <param name="tolerance">the allowable standard deviation</param>
    public bool gazeSteady(float time, float tolerance)
    {
        if (!hasEnoughData)
            return false;
        int timeInMicroseconds = (int)(time * 1e6);
        CopyToTwoArrays(timeInMicroseconds, out float[] x_array, out float[] y_array);
        float targetGazePointX = Queryable.Average(x_array.AsQueryable());
        float targetGazePointY = Queryable.Average(y_array.AsQueryable());

        return gazeSteadyImpl(x_array, y_array, targetGazePointX, targetGazePointY, tolerance);
    }

    private bool gazeSteadyImpl(
        float[] x_array,
        float[] y_array,
        float targetGazePointX,
        float targetGazePointY,
        float tolerance
    )
    {
        bool steady = false;
        float sumOfSquaresX = x_array
            .Select(val => (val - targetGazePointX) * (val - targetGazePointX))
            .Sum();
        float sumOfSquaresY = y_array
            .Select(val => (val - targetGazePointY) * (val - targetGazePointY))
            .Sum();
        float stddevX = (float)Math.Sqrt(sumOfSquaresX / x_array.Length);
        float stddevY = (float)Math.Sqrt(sumOfSquaresY / y_array.Length);

        if (stddevX < tolerance && stddevY < tolerance)
            steady = true;

        Debug.Log(
            "Gaze is "
                + steady
                + " at "
                + targetGazePointX
                + " "
                + targetGazePointY
                + "("
                + stddevX
                + ", "
                + stddevY
                + ")"
                + " Based on "
                + x_array.Length
                + " samples"
        );
        return steady;
    }
}

/// <summary>
/// Holds the HeadPose pitch buffer.
/// </summary>
public class RocketHeadPitchSpeedBuffer : TobiiBuffer<RocketHeadPitch>
{
    public RocketHeadPitchSpeedBuffer(int capacity)
        : base(capacity, 2) { }
}

/// <summary>
/// Holds the HeadPose yaw buffer.
/// </summary>
public class RocketHeadYawSpeedBuffer : TobiiBuffer<RocketHeadYaw>
{
    public RocketHeadYawSpeedBuffer(int capacity)
        : base(capacity, 2) { }
}
