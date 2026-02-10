using System;
using System.Collections.Generic;
using System.Linq;
using Tobii.GameIntegration.Net;

/// <summary>
/// Holds a buffer for a head angle (pitch, yaw or roll).
/// </summary>
class HeadAngleBuffer : TobiiBuffer<HeadAngleItem>
{
    public HeadAngleBuffer(int capacity)
        : base(capacity, 2) { }

    /// <summary>
    /// Calculates the average speed of the buffer over a given time period.
    /// Speed is
    /// calculated as the average change in angle returned by GetAngle
    /// Divided by the total change in time returned by TimeStampMicroSeconds
    /// </summary>
    /// <param name="speedTime">The time period in seconds over which to calculate the average speed.</param>
    /// <returns>The average speed of the buffer over the given time period.</returns>
    public float getSpeed(float speedTime)
    {
        float averageSpeed = 0f;
        if (!hasEnoughData)
            return averageSpeed;

        int timeInMicroseconds = (int)(speedTime * 1e6);
        List<HeadAngleItem> headAngles = GetItems(timeInMicroseconds);

        return calculateAverageSpeed(headAngles);
    }

    private float calculateAverageSpeed(List<HeadAngleItem> headAngles)
    {
        if (headAngles.Count() < 2)
        {
            return 0f;
        }
        float totalDistance = 0f;
        for (int i = 0; i < headAngles.Count() - 1; i++)
        {
            totalDistance += Math.Abs(headAngles[i + 1].GetAngle() - headAngles[i].GetAngle());
        }

        double totalTime =
            (
                headAngles[0].TimeStampMicroSeconds()
                - headAngles[headAngles.Count() - 1].TimeStampMicroSeconds()
            ) / 1e6;
        float averageSpeed = (float)(totalDistance / totalTime);

        return averageSpeed;
    }
}

/// <summary>
/// Holds the gazepoint buffer and provides a method to check gaze stability and direction.
/// </summary>
class GazeBuffer : TobiiBuffer<GazeItem>
{
    public GazeBuffer(int capacity)
        : base(capacity, 2) { }

    /// <summary>
    /// returns true if the data more recent than the time have a summed
    /// square distance from the target point less than the tolerance.
    /// </summary>
    /// <param name="time">in seconds to sample over</param>
    /// <param name="tolerance">the allowable range</param>
    /// <param name="targetPoint_x">x coordinate of the target point</param>
    /// <param name="targetPoint_y">y coordinate of the target point</param>
    public bool gazeSteady(float time, float tolerance, float targetPoint_x, float targetPoint_y)
    {
        if (!hasEnoughData)
            return false;
        int timeInMicroseconds = (int)(time * 1e6);
        List<GazeItem> gazePoints = GetItems(timeInMicroseconds);
        GetXYArrays(gazePoints, out float[] array_x, out float[] array_y);
        return dataSteadyImpl(array_x, array_y, targetPoint_x, targetPoint_y, tolerance);
    }

    // <summary>
    /// returns true if the data more recent than the time have a standard deviation
    /// less than the tolerance.
    /// </summary>
    /// <param name="time">in seconds to sample over</param>
    /// <param name="tolerance">the allowable standard deviation</param>
    public bool gazeSteady(float time, float tolerance)
    {
        if (!hasEnoughData)
            return false;
        int timeInMicroseconds = (int)(time * 1e6);
        List<GazeItem> gazePoints = GetItems(timeInMicroseconds);
        GetXYArrays(gazePoints, out float[] array_x, out float[] array_y);
        float targetPoint_0 = Queryable.Average(array_x.AsQueryable());
        float targetPoint_1 = Queryable.Average(array_y.AsQueryable());

        return dataSteadyImpl(array_x, array_y, targetPoint_0, targetPoint_1, tolerance);
    }

    ///<summary>
    /// Convert a list of gaze items into two arrays of x and y positions.
    ///</summary>
    /// <param name="array_x">The x array to fill</param>
    /// <param name="array_y">The y array to fill</param>
    private void GetXYArrays(List<GazeItem> gazeItems, out float[] array_x, out float[] array_y)
    {
        array_x = new float[gazeItems.Count()];
        array_y = new float[gazeItems.Count()];
        for (int i = 0; i < gazeItems.Count(); i++)
        {
            array_x[i] = gazeItems[i].getX();
            array_y[i] = gazeItems[i].getY();
        }
    }

    private bool dataSteadyImpl(
        float[] xPositions,
        float[] yPositions,
        float targetPointX,
        float targetPointY,
        float tolerance
    )
    {
        bool steady = false;
        float sumOfSquares_0 = xPositions
            .Select(val => (val - targetPointX) * (val - targetPointX))
            .Sum();
        float sumOfSquares_1 = yPositions
            .Select(val => (val - targetPointY) * (val - targetPointY))
            .Sum();
        float stddev_0 = (float)Math.Sqrt(sumOfSquares_0 / xPositions.Length);
        float stddev_1 = (float)Math.Sqrt(sumOfSquares_1 / yPositions.Length);

        if (stddev_0 < tolerance && stddev_1 < tolerance)
            steady = true;

        return steady;
    }
}

/// define two interfaces for the buffer data to enable us to create templated buffers.
interface ITimeStampMicroSeconds
{
    long TimeStampMicroSeconds();
}

/// <summary>
/// Wrapper for Tobii gazepoint pitch data, implementing GetData and timestamp interfaces.
/// </summary>
class GazeItem : ITimeStampMicroSeconds
{
    public GazePoint gazePoint;

    public long TimeStampMicroSeconds() => gazePoint.TimeStampMicroSeconds;

    public float getX() => gazePoint.X;

    public float getY() => gazePoint.Y;
}

/// <summary>
/// Wrapper for Tobii gazepoint pitch data, implementing GetData and timestamp interfaces.
/// </summary>
abstract class HeadAngleItem : ITimeStampMicroSeconds
{
    protected HeadPose headPose;

    public HeadAngleItem(HeadPose headPose)
    {
        this.headPose = headPose;
    }

    public long TimeStampMicroSeconds() => headPose.TimeStampMicroSeconds;

    public abstract float GetAngle();
}

/// <summary>
/// Wrapper for Tobii head pose pitch data, implementing GetData and timestamp interfaces.
/// </summary>
class HeadPitchItem : HeadAngleItem
{
    public HeadPitchItem(HeadPose headPose)
        : base(headPose) { }

    public override float GetAngle()
    {
        return headPose.Rotation.PitchDegrees;
    }
}

/// <summary>
/// Wrapper for Tobii head pose yaw data, implementing GetData and timestamp interfaces.
/// </summary>
class HeadYawItem : HeadAngleItem
{
    public HeadYawItem(HeadPose headPose)
        : base(headPose) { }

    public override float GetAngle()
    {
        return headPose.Rotation.YawDegrees;
    }
}

/// <summary>
/// Base class for the tracker buffers, provides functionality to add items in a continuous loop, overwriting
/// old data when the buffer is full.
/// Also provides functions to calculate speed for the pose data.
/// </summary>
class TobiiBuffer<T>
    where T : ITimeStampMicroSeconds
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
    /// Return all items in the buffer created less than 'maximumAge' ago.
    /// Items will be returned in order from newest to oldest.
    /// </summary>
    /// <param name="maximumAge">The maximum age (in microseconds) of the data to return.</param>
    public List<T> GetItems(long maximumAge)
    {
        List<T> bufferItems = new List<T>();

        if (!hasData)
        {
            return bufferItems;
        }

        int bufferIndex = lastAddedIndex;
        bufferItems.Add(buffer[bufferIndex]);
        long oldestAllowableTime = buffer[bufferIndex].TimeStampMicroSeconds() - maximumAge;

        bufferIndex = bufferIndex > 0 ? bufferIndex - 1 : buffer.Length - 1;

        while (
            bufferIndex != lastAddedIndex
            && buffer[bufferIndex].TimeStampMicroSeconds() >= oldestAllowableTime
        )
        {
            bufferItems.Add(buffer[bufferIndex]);
            bufferIndex = bufferIndex > 0 ? bufferIndex - 1 : buffer.Length - 1;
        }

        return bufferItems;
    }
}
