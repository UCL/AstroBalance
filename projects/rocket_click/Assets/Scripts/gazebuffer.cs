using UnityEngine;
using RingBuffer;
using Tobii.GameIntegration.Net;

namespace GazeBuffer
{
    /// <summary>
    /// Will hold the gazepoint buffer and provide methods to check gaze stability and direction.
    /// </summary>
    /// 
    public class gazeBuffer : RingBuffer<GazePoint>
    {
        public gazeBuffer(int capacity, bool overflow = false) : base(capacity, overflow)
        {
        }
        
        // returns true if the gaze points in the buffer are all within a small radius.
        public bool gazeSteady()
        {
            // implement me.
            return false;
        }
    }
    

}