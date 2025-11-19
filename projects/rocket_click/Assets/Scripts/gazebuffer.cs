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
        // returns true if the gaze points in the buffer are all within a small radius.
        // param time (float in seconds to sample over)
        // param tolerance (float the allowable range)
        // param x_target (if nan we use the mean)
        // param y_target (in nan we use the mean)
        public bool gazeSteady(float time, float tolerance, GazePoint targetGazePoint)
        {
            int timeInMicroseconds = (int)(time * 1e6);
            CopyToTwoArrays(timeInMicroseconds, out float[] x_array, out float[] y_array);
 	    float averageX = Queryable.Average(x_array.AsQueryable());
 	    float averageY = Queryable.Average(y_array.AsQueryable());
	
	    float sumOfSquaresX = x_array.Select(val => (val - averageX) * (val - averageX)).Sum();
	    float sumOfSquaresY = x_array.Select(val => (val - averageY) * (val - averageY)).Sum();
            // implement me.
            return false;
        }

        /** <summary>
        Copies the contents of the RingBuffer to two arrays,
        one for the x coordinates and one for the y, stopping
        when the timestamps are older than <paramref name="timestamp"/>
        TODO thread safety? What happens if another thread writes to the buffer
        Whilst this is running? There's a syncRoot object, but I'm not clear how
        it is used.
        </summary> */
        private void CopyToTwoArrays(int timestamp, out float[] x_array, out float[] y_array)
        {
            int _index = tail;
            int arrayIndex = 0;
            x_array = new float[size];
            y_array = new float[size];
            while (_index >= 0 && buffer[_index].TimeStampMicroSeconds >= timestamp)
            {
                x_array[arrayIndex] = buffer[_index].X;
                y_array[arrayIndex] = buffer[_index].Y;
                _index--;
                arrayIndex++;
            }
            Array.Resize(ref x_array, arrayIndex);
            Array.Resize(ref y_array, arrayIndex);
        }
    }
}
