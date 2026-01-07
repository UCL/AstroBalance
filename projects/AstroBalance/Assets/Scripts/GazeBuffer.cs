using System;
using System.Linq;
using RingBuffer;
using Tobii.GameIntegration.Net;
using UnityEngine;

namespace GazeBuffer
{
    /// <summary>
    /// Will hold the gazepoint buffer and provide methods to check gaze stability and direction.
    /// </summary>
    ///
    ///
    /// TODO Understand how to make this thread safe.
    public class gazeBuffer : RingBuffer<GazePoint>
    {
        public gazeBuffer(int capacity, bool overflow = false)
            : base(capacity, overflow) { }

        // <summary> 
        // Adds a new gaze point to the buffer if it has a different timestamp to the last added gaze point.
        // returns true if the point was added, false otherwise.
        public new bool addIfNew(GazePoint gazePoint)
        {
            int last_entry = tail - 1;
            if (last_entry < 0)
                last_entry = size - 1;

            if (size == 0 || gazePoint.TimeStampMicroSeconds != buffer[last_entry].TimeStampMicroSeconds)
            {
                base.Add(gazePoint);
                return true;
            }
            return false;
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
            int _index = (tail - 1);
            if (_index < 0)
                _index = size - 1;

            int arrayIndex = 0;
            x_array = new float[size];
            y_array = new float[size];
            x_array[arrayIndex] = buffer[_index].X;
            y_array[arrayIndex] = buffer[_index].Y;
            long timestamp = buffer[_index].TimeStampMicroSeconds- gazeTime;

            arrayIndex++;
            if (_index > 0)
                _index--;
            else
                _index = size - 1;
          
            while (_index != head && buffer[_index].TimeStampMicroSeconds >= timestamp)
            {
                Debug.Log(DateTime.Now.ToFileTime() + "Adding point " + "(" + buffer[_index].X + ", " + buffer[_index].Y + ") at " + buffer[_index].TimeStampMicroSeconds + " > " + timestamp);
                x_array[arrayIndex] = buffer[_index].X;
                y_array[arrayIndex] = buffer[_index].Y;
                if (_index > 0)
                    _index--;
                else
                    _index = size - 1;
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
}
