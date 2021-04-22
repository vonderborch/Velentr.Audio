using Microsoft.Xna.Framework;
using System;

namespace Velentr.Audio.Helpers
{
    /// <summary>
    ///     Time helper methods.
    /// </summary>
    public static class TimeHelpers
    {
        /// <summary>
        ///     Determines the elapsed milliseconds between two times
        /// </summary>
        ///
        /// <param name="startTime"> The start time. </param>
        /// <param name="endTime">   The end time. </param>
        ///
        /// <returns>
        ///     An uint.
        /// </returns>
        public static uint ElapsedMilliSeconds(TimeSpan startTime, GameTime endTime)
        {
            if (startTime == TimeSpan.MinValue)
            {
                return Convert.ToUInt32(endTime.TotalGameTime.TotalMilliseconds);
            }

            return Convert.ToUInt32(Math.Abs((endTime.TotalGameTime - startTime).TotalMilliseconds));
        }
    }
}