using System;
using System.Linq;
using RingBuffer;
using Tobii.GameIntegration.Net;
using UnityEngine;

namespace TrackerBuffers
{
    /// <summary>
    /// Will hold the gazepoint buffer and provide methods to check gaze stability and direction.
    /// </summary>
    ///
    ///
    /// TODO Understand how to make this thread safe.
    public class GazeBuffer : RingBuffer<GazePoint>
    {
        public GazeBuffer(int capacity)
            : base(capacity, true) { }

        // <summary>
        // Adds a new gaze point to the buffer if it has a different timestamp to the last added gaze point.
        // returns true if the point was added, false otherwise.
        public bool addIfNew(GazePoint item)
        {

            if (size == 0 || item.TimeStampMicroSeconds != buffer[getLatestEntryIndex()].TimeStampMicroSeconds)
            {
                base.Add(item);
                return true;
            }
            return false;
        }

        private int getLatestEntryIndex()
        {
            int last_entry = tail - 1;
            if (last_entry < 0)
                last_entry = size - 1;
            return last_entry;
        }

              // <summary>
        // returns true if the gaze points in the buffer are all within a small radius.
        // param time (float in seconds to sample over)
        // param tolerance (float the allowable range)
        // param x_target (if nan we use the mean)
        // param y_target (in nan we use the mean)
        public bool gazeSteady(float time, float tolerance, GazePoint targetGazePoint)
        {
            bool steady = false;
            if (size == 0)
                return steady;
            int timeInMicroseconds = (int)(time * 1e6);
            Debug.Log(" (HEAD/TAIL = " + head + "/" + tail + ")");
            CopyToTwoArrays(timeInMicroseconds, out float[] x_array, out float[] y_array);
            float averageX = Queryable.Average(x_array.AsQueryable());
            float averageY = Queryable.Average(y_array.AsQueryable());

            float sumOfSquaresX = x_array.Select(val => (val - averageX) * (val - averageX)).Sum();
            float sumOfSquaresY = y_array.Select(val => (val - averageY) * (val - averageY)).Sum();

            if (sumOfSquaresX < tolerance && sumOfSquaresY < tolerance)
                steady = true;

            Debug.Log("Gaze is " + steady + " at " + averageX + " " + averageY
                + "(" + sumOfSquaresX + ", " + sumOfSquaresY + ")" + " Based on " + x_array.Length + " samples");
            return steady;
        }

        /** <summary>
        Copies the contents of the RingBuffer to two arrays,
        one for the x coordinates and one for the y, stopping
        when the timestamps are older than <paramref name="timestamp"/>
        TODO thread safety? What happens if another thread writes to the buffer
        Whilst this is running? There's a syncRoot object, but I'm not clear how
        it is used.
        </summary> */
        private void CopyToTwoArrays(long gazeTime, out float[] x_array, out float[] y_array)
        {
            if (size == 0)
            {
                x_array = new float[0];
                y_array = new float[0];
                return;
            }
            int _index = getLatestEntryIndex();

            int arrayIndex = 0;
            x_array = new float[size];
            y_array = new float[size];
            x_array[arrayIndex] = buffer[_index].X;
            y_array[arrayIndex] = buffer[_index].Y;
            long timestamp = buffer[_index].TimeStampMicroSeconds - gazeTime;

            _index = _index > 0 ? _index - 1 : size - 1;
            arrayIndex++;

            while (_index != head && buffer[_index].TimeStampMicroSeconds >= timestamp)
            {
                Debug.Log(DateTime.Now.ToFileTime() + "Adding point " + "(" + buffer[_index].X + ", " + buffer[_index].Y + ") at " + buffer[_index].TimeStampMicroSeconds + " > " + timestamp);
                x_array[arrayIndex] = buffer[_index].X;
                y_array[arrayIndex] = buffer[_index].Y;
                _index = _index > 0 ? _index - 1 : size - 1;
                arrayIndex++;
            }
            // Add head if it falls in time range
            if (size > 1 && _index == head && buffer[_index].TimeStampMicroSeconds >= timestamp)
            {
                Debug.Log(DateTime.Now.ToFileTime() + "Adding head point " + "(" + buffer[_index].X + ", " + buffer[_index].Y + ") at " + buffer[_index].TimeStampMicroSeconds + " > " + timestamp);
                x_array[arrayIndex] = buffer[_index].X;
                y_array[arrayIndex] = buffer[_index].Y;
                arrayIndex++;
            }

            Array.Resize(ref x_array, arrayIndex);
            Array.Resize(ref y_array, arrayIndex);
        }
    }


    /// Will hold the HeadPose buffer and provide methods to check movement speed.
    /// </summary>
    /// TODO I should be able to use inheritance to reduce duplication here, but
    /// had difficultly with templating.
    public class HeadPoseBuffer : RingBuffer<HeadPose>
    {
        public HeadPoseBuffer(int capacity)
            : base(capacity, true) { }

        // <summary>
        // Adds a new gaze point to the buffer if it has a different timestamp to the last added gaze point.
        // returns true if the point was added, false otherwise.
        public bool addIfNew(HeadPose item)
        {

            if (size == 0 || item.TimeStampMicroSeconds != buffer[getLatestEntryIndex()].TimeStampMicroSeconds)
            {
                base.Add(item);
                return true;
            }
            return false;
        }

        public float getSpeed(float speedTime)
        {
            // implement me
            return 0f;
        }

        private int getLatestEntryIndex()
        {
            int last_entry = tail - 1;
            if (last_entry < 0)
                last_entry = size - 1;
            return last_entry;
        }

        // <summary>
        // creates two arrays, one of x positions and one of timestamps, from which we can
        // calculate average speeds
        private void CopyToTwoArrays(long speedTime, out float[] x_array, out float[] timestamp_array)
        {
            if (size == 0)
            {
                x_array = new float[0];
                timestamp_array = new float[0];
                return;
            }
            int _index = getLatestEntryIndex();

            int arrayIndex = 0;
            x_array = new float[size];
            timestamp_array = new float[size];

            x_array[arrayIndex] = buffer[_index].Position.X;
            timestamp_array[arrayIndex] = buffer[_index].TimeStampMicroSeconds;
            long timestamp = buffer[_index].TimeStampMicroSeconds - speedTime;

            _index = _index > 0 ? _index - 1 : size - 1;
            arrayIndex++;

            while (_index != head && buffer[_index].TimeStampMicroSeconds >= timestamp)
            {
                x_array[arrayIndex] = buffer[_index].Position.X;
                timestamp_array[arrayIndex] = buffer[_index].TimeStampMicroSeconds;
                _index = _index > 0 ? _index - 1 : size - 1;
                arrayIndex++;
            }
            // Add head if it falls in time range
            if (size > 1 && _index == head && buffer[_index].TimeStampMicroSeconds >= timestamp)
            {
                x_array[arrayIndex] = buffer[_index].Position.X;
                timestamp_array[arrayIndex] = buffer[_index].TimeStampMicroSeconds;
                arrayIndex++;
            }

            Array.Resize(ref x_array, arrayIndex);
            Array.Resize(ref timestamp_array, arrayIndex);
        }

    }

}
