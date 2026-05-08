using System;
using System.Collections.Generic;
using System.Linq;
using Tobii.GameIntegration.Net;

/// <summary>
/// Head rotation axes
/// </summary>
enum RotationAxis
{
    Roll,
    Pitch,
    Yaw,
}

/// <summary>
/// Position axes
/// </summary>
enum PositionAxis
{
    X,
    Y,
    Z,
}

/// <summary>
/// Holds a buffer for a head angle (pitch, yaw or roll).
/// </summary>
class HeadPoseBuffer : TobiiBuffer<HeadPoseItem>
{
    /// <summary>
    /// Initializes a new instance of the HeadAngleBuffer class.
    /// </summary>
    /// <param name="capacity">The maximum number of items that can be stored in the buffer.</param>
    /// <param name="minDataRequired">The minimum number of data points required to calculate a speed.</param>
    public HeadPoseBuffer(int capacity, int minDataRequired)
        : base(capacity, minDataRequired) { }

    /// <summary>
    /// Calculates the average speed of the buffer over a given time period.
    /// Speed is calculated as the average change in angle divided by the total change in time
    /// based on TimeStampMicroSeconds
    /// </summary>
    /// <param name="speedTime">The time period in seconds over which to calculate the average speed.</param>
    /// <returns>The average speed of the buffer over the given time period (in degrees per second)</returns>
    public float GetRotationSpeed(float speedTime, RotationAxis rotationAxis)
    {
        float averageSpeed = 0f;
        if (!hasEnoughData)
            return averageSpeed;

        List<TimestampedValue> rotationValues = GetRotationValues(speedTime, rotationAxis);
        return calculateAverageSpeed(rotationValues);
    }

    /// <summary>
    /// Calculates the average speed of the buffer over a given time period.
    /// Speed is calculated as the average change in position divided by the total change in time
    /// based on TimeStampMicroSeconds
    /// </summary>
    /// <param name="speedTime">The time period in seconds over which to calculate the average speed.</param>
    /// <returns>The average speed of the buffer over the given time period (in mm per second)</returns>
    public float GetPositionSpeed(float speedTime, PositionAxis positionAxis)
    {
        float averageSpeed = 0f;
        if (!hasEnoughData)
            return averageSpeed;

        List<TimestampedValue> positionValues = GetPositionValues(speedTime, positionAxis);
        return calculateAverageSpeed(positionValues);
    }

    private List<TimestampedValue> GetRotationValues(float timePeriod, RotationAxis rotationAxis)
    {
        int timeInMicroseconds = (int)(timePeriod * 1e6);
        List<HeadPoseItem> headposes = GetItems(timeInMicroseconds);
        List<TimestampedValue> rotationValues = new List<TimestampedValue>();

        foreach (HeadPoseItem headpose in headposes)
        {
            TimestampedValue rotationValue = new();
            rotationValue.value = headpose.GetRotation(rotationAxis);
            rotationValue.TimeStampMicroSeconds = headpose.TimeStampMicroSeconds();
        }

        return rotationValues;
    }

    private List<TimestampedValue> GetPositionValues(float timePeriod, PositionAxis positionAxis)
    {
        int timeInMicroseconds = (int)(timePeriod * 1e6);
        List<HeadPoseItem> headposes = GetItems(timeInMicroseconds);
        List<TimestampedValue> positionValues = new List<TimestampedValue>();

        foreach (HeadPoseItem headpose in headposes)
        {
            TimestampedValue positionValue = new();
            positionValue.value = headpose.GetPosition(positionAxis);
            positionValue.TimeStampMicroSeconds = headpose.TimeStampMicroSeconds();
        }

        return positionValues;
    }

    private float calculateAverageSpeed(List<TimestampedValue> signedPositions)
    {
        if (signedPositions.Count() < minDataRequired)
        {
            return 0f;
        }
        float totalDistance = 0f;
        for (int i = 0; i < signedPositions.Count() - 1; i++)
        {
            totalDistance += Math.Abs(signedPositions[i + 1].value - signedPositions[i].value);
        }

        double totalTime =
            (
                signedPositions[0].TimeStampMicroSeconds
                - signedPositions[signedPositions.Count() - 1].TimeStampMicroSeconds
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
    /// <summary>
    /// Initializes a new instance of the GazeBuffer class.
    /// </summary>
    /// <param name="capacity">The maximum number of items that can be stored in the buffer.</param>
    /// <param name="minDataRequired">The minimum number of data points required to calculate steadiness.</param>
    public GazeBuffer(int capacity, int minDataRequired)
        : base(capacity, minDataRequired) { }

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
        float targetPoint_x = Queryable.Average(array_x.AsQueryable());
        float targetPoint_y = Queryable.Average(array_y.AsQueryable());

        return dataSteadyImpl(array_x, array_y, targetPoint_x, targetPoint_y, tolerance);
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
        float sumOfSquares_x = xPositions
            .Select(val => (val - targetPointX) * (val - targetPointX))
            .Sum();
        float sumOfSquares_y = yPositions
            .Select(val => (val - targetPointY) * (val - targetPointY))
            .Sum();
        float stddev_x = (float)Math.Sqrt(sumOfSquares_x / xPositions.Length);
        float stddev_y = (float)Math.Sqrt(sumOfSquares_y / yPositions.Length);

        if (stddev_x < tolerance && stddev_y < tolerance)
            steady = true;

        return steady;
    }
}

/// define an interface for the buffer data to enable us to create templated buffers.
interface ITimeStampMicroSeconds
{
    long TimeStampMicroSeconds();
}

/// <summary>
/// A general class for a value with a timestamp
/// </summary>
class TimestampedValue
{
    public long TimeStampMicroSeconds;
    public float value;
}

/// <summary>
/// Wrapper for Tobii gazepoint data, implementing timestamp interface.
/// </summary>
class GazeItem : ITimeStampMicroSeconds
{
    public GazePoint gazePoint;

    public long TimeStampMicroSeconds() => gazePoint.TimeStampMicroSeconds;

    public float getX() => gazePoint.X;

    public float getY() => gazePoint.Y;
}

/// <summary>
/// Wrapper for Tobii headpose data, implementing timestamp interface.
/// </summary>
class HeadPoseItem : ITimeStampMicroSeconds
{
    protected HeadPose headPose;

    public HeadPoseItem(HeadPose headPose)
    {
        this.headPose = headPose;
    }

    public long TimeStampMicroSeconds() => headPose.TimeStampMicroSeconds;

    public float GetPosition(PositionAxis positionAxis)
    {
        if (positionAxis == PositionAxis.X)
        {
            return headPose.Position.X;
        }
        else if (positionAxis == PositionAxis.Y)
        {
            return headPose.Position.Y;
        }
        else
        {
            return headPose.Position.Z;
        }
    }

    public float GetRotation(RotationAxis rotationAxis)
    {
        if (rotationAxis == RotationAxis.Yaw)
        {
            return headPose.Rotation.YawDegrees;
        }
        else if (rotationAxis == RotationAxis.Pitch)
        {
            return headPose.Rotation.PitchDegrees;
        }
        else
        {
            return headPose.Rotation.RollDegrees;
        }
    }
}

/// <summary>
/// Base class for the tracker buffers, provides functionality to add items in a continuous loop, overwriting
/// old data when the buffer is full.
/// </summary>
class TobiiBuffer<T>
    where T : ITimeStampMicroSeconds
{
    protected int lastAddedIndex;
    private bool hasData; // flag to indicate if the buffer has any data
    protected bool hasEnoughData; // flag to indicate if the buffer has enough data to calculate speed or steadiness.
    protected int minDataRequired;
    protected T[] buffer;

    /// <summary>
    /// Initializes a new instance of the <see cref="TobiiBuffer{T}"/> class.
    /// </summary>
    /// <param name="capacity">The capacity of the buffer.</param>
    /// <param name="minDataRequired">The minimum number of data points required for calculations to be meaningful.</param>
    public TobiiBuffer(int capacity, int minDataRequired)
    {
        if (capacity <= 0 || minDataRequired <= 0 || minDataRequired > capacity)
        {
            throw new ArgumentException(
                "Capacity and minDataRequired must be positive and minDataRequired must be less than or equal to capacity."
            );
        }
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
            if (newIndex + 1 >= minDataRequired) // index starts at 0, so items added = newIndex + 1
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
    protected List<T> GetItems(long maximumAge)
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
            && buffer[bufferIndex] != null
            && buffer[bufferIndex].TimeStampMicroSeconds() >= oldestAllowableTime
        )
        {
            bufferItems.Add(buffer[bufferIndex]);
            bufferIndex = bufferIndex > 0 ? bufferIndex - 1 : buffer.Length - 1;
        }

        return bufferItems;
    }
}
