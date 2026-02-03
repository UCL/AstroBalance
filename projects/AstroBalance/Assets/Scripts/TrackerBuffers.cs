using System;
using System.Linq;
using Tobii.GameIntegration.Net;
using UnityEngine;

/// <summary>
/// Holds the gazepoint buffer and provides a method to check gaze stability and direction.
/// </summary>
class GazeBuffer : TobiiBuffer<RocketGazePoint>
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
        return base.dataSteady(time, tolerance, targetGazePoint.X, targetGazePoint.Y);
    }

    /// <summary>
    /// returns true if the gaze points more recent than the time have a standard deviation
    /// less than the tolerance.
    /// </summary>
    /// <param name="time">in seconds to sample over</param>
    /// <param name="tolerance">the allowable standard deviation</param>
    public bool gazeSteady(float time, float tolerance)
    {
        return base.dataSteady(time, tolerance);
    }
}

/// <summary>
/// Holds the HeadPose pitch buffer, ad provides methods to calculate the average speed of the buffer over a given time period.
/// </summary>
class RocketHeadPitchSpeedBuffer : TobiiBuffer<RocketHeadPitch>
{
    public RocketHeadPitchSpeedBuffer(int capacity)
        : base(capacity, 2) { }

    /// <summary>
    /// Calculates the average speed of the buffer over a given time period. Speed is
    /// calculated to the average change in pitch the total change in time.
    /// </summary>
    /// <param name="speedTime">The time period in seconds over which to calculate the average speed.</param>
    /// <returns>The average speed of the head pose buffer over the given time period.</returns>
    public float getPitchSpeed(float speedTime)
    {
        return base.getSpeed(speedTime);
    }
}

/// <summary>
/// Holds the HeadPose yaw buffer, and provides methods to calculate the average speed of the buffer over a given time period.
/// </summary>
class RocketHeadYawSpeedBuffer : TobiiBuffer<RocketHeadYaw>
{
    public RocketHeadYawSpeedBuffer(int capacity)
        : base(capacity, 2) { }

    /// <summary>
    /// Calculates the average speed of the buffer over a given time period. Speed is
    /// calculated to the average change in yaw divided by the total change in time.
    /// </summary>
    /// <param name="speedTime">The time period in seconds over which to calculate the average speed.</param>
    /// <returns>The average speed of the head pose buffer over the given time period.</returns>
    public float getYawSpeed(float speedTime)
    {
        return base.getSpeed(speedTime);
    }
}

/// define two interfaces for the buffer data to enable us to create templated buffers.
interface ITimeStampMicroSeconds
{
    long TimeStampMicroSeconds();
}

interface IBufferData
{
    float[] GetData();
}

/// <summary>
/// Wrapper for Tobii gazepoint pitch data, implementing GetData and timestamp interfaces.
/// </summary>
class RocketGazePoint : ITimeStampMicroSeconds, IBufferData
{
    public GazePoint gazePoint;

    public long TimeStampMicroSeconds() => gazePoint.TimeStampMicroSeconds;

    public float[] GetData() => new float[] { gazePoint.X, gazePoint.Y };
}

/// <summary>
/// Wrapper for Tobii head pose pitch data, implementing GetData and timestamp interfaces.
/// </summary>
class RocketHeadPitch : ITimeStampMicroSeconds, IBufferData
{
    private HeadPose headPose;

    public long TimeStampMicroSeconds() => headPose.TimeStampMicroSeconds;

    public float[] GetData() =>
        new float[] { (float)headPose.TimeStampMicroSeconds, headPose.Rotation.PitchDegrees };

    public RocketHeadPitch(HeadPose headPose)
    {
        this.headPose = headPose;
    }
}

/// <summary>
/// Wrapper for Tobii head pose yaw data, implementing GetData and timestamp interfaces.
/// </summary>
class RocketHeadYaw : ITimeStampMicroSeconds, IBufferData
{
    private HeadPose headPose;

    public long TimeStampMicroSeconds() => headPose.TimeStampMicroSeconds;

    public float[] GetData() =>
        new float[] { (float)headPose.TimeStampMicroSeconds, headPose.Rotation.YawDegrees };

    public RocketHeadYaw(HeadPose headPose)
    {
        this.headPose = headPose;
    }
}

/// <summary>
/// Base class for the tracker buffers, provides functionality to add items in a continuous loop, overwriting
/// old data when the buffer is full.
/// Also provides functions to calculate speed for the pose data.
/// </summary>
internal class TobiiBuffer<T>
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
    /// Calculates the average speed of the buffer over a given time period. Speed is
    /// calculated to the average change in position of the second array returned by GetData
    /// Divided by the total change in the first array returned by GetData
    /// This presumes that the GetData interface of the templated object returns a
    /// float vector of the form [time, position].
    /// </summary>
    /// <param name="speedTime">The time period in seconds over which to calculate the average speed.</param>
    /// <returns>The average speed of the head pose buffer over the given time period.</returns>
    protected float getSpeed(float speedTime)
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

    /// <summary>
    /// returns true if the data more recent than the time have a summed
    /// square distance from the target point less than the tolerance.
    /// </summary>
    /// <param name="time">in seconds to sample over</param>
    /// <param name="tolerance">the allowable range</param>
    /// <param name="targetPoint_0">the target point for the first item returned by GetData</param>
    /// <param name="targetPoint_1">the target point for the second item returned by GetData</param>
    protected bool dataSteady(float time, float tolerance, float targetPoint_0, float targetPoint_1)
    {
        if (!hasEnoughData)
            return false;
        int timeInMicroseconds = (int)(time * 1e6);
        CopyToTwoArrays(timeInMicroseconds, out float[] array_0, out float[] array_1);
        return dataSteadyImpl(array_0, array_1, targetPoint_0, targetPoint_1, tolerance);
    }

    /// <summary>
    /// returns true if the data more recent than the time have a standard deviation
    /// less than the tolerance.
    /// </summary>
    /// <param name="time">in seconds to sample over</param>
    /// <param name="tolerance">the allowable standard deviation</param>
    protected bool dataSteady(float time, float tolerance)
    {
        if (!hasEnoughData)
            return false;
        int timeInMicroseconds = (int)(time * 1e6);
        CopyToTwoArrays(timeInMicroseconds, out float[] array_0, out float[] array_1);
        float targetPoint_0 = Queryable.Average(array_0.AsQueryable());
        float targetPoint_1 = Queryable.Average(array_1.AsQueryable());

        return dataSteadyImpl(array_0, array_1, targetPoint_0, targetPoint_1, tolerance);
    }

    private bool dataSteadyImpl(
        float[] array_0,
        float[] array_1,
        float targetPoint_0,
        float targetPoint_1,
        float tolerance
    )
    {
        bool steady = false;
        float sumOfSquares_0 = array_0
            .Select(val => (val - targetPoint_0) * (val - targetPoint_0))
            .Sum();
        float sumOfSquares_1 = array_1
            .Select(val => (val - targetPoint_1) * (val - targetPoint_1))
            .Sum();
        float stddev_0 = (float)Math.Sqrt(sumOfSquares_0 / array_0.Length);
        float stddev_1 = (float)Math.Sqrt(sumOfSquares_1 / array_1.Length);

        if (stddev_0 < tolerance && stddev_1 < tolerance)
            steady = true;

        return steady;
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
    private void CopyToTwoArrays(long maximumAge, out float[] array_0, out float[] array_1)
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
