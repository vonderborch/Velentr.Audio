using System;

namespace Velentr.Audio.Helpers
{
    /// <summary>
    ///     A randomizer using System.Random to generate the random numbers
    /// </summary>
    ///
    /// <seealso cref="Randomizer"/>
    public class SystemRandomizer : Randomizer
    {
        /// <summary>
        ///     A Random to process.
        /// </summary>
        private Random r;

        /// <summary>
        ///     Constructor.
        /// </summary>
        ///
        /// <param name="seed"> (Optional) The seed. </param>
        public SystemRandomizer(long? seed = null) : base(seed = null)
        {
            if (seed == null)
            {
                r = new Random();
            }
            else
            {
                r = new Random((int)seed);
            }
        }

        /// <summary>
        ///     Gets the next random double.
        /// </summary>
        ///
        /// <param name="minValue">       The minimum value. </param>
        /// <param name="maxValue">       The maximum value. </param>
        /// <param name="maxIsExclusive"> (Optional) True if maximum is exclusive. </param>
        ///
        /// <returns>
        ///     The next random double.
        /// </returns>
        ///
        /// <seealso cref="Velentr.Audio.Helpers.Randomizer.GetNextRandomDouble(double,double,bool)"/>
        public override double GetNextRandomDouble(double minValue, double maxValue, bool maxIsExclusive = true)
        {
            if (minValue.Equals(0d) && maxValue.Equals(1d))
            {
                return r.NextDouble();
            }

            var value = r.NextDouble();
            return value * GetNextRandomLong((long)Math.Floor(minValue), (long)Math.Ceiling(maxValue));
        }

        /// <summary>
        ///     Gets the next random float.
        /// </summary>
        ///
        /// <param name="minValue">       The minimum value. </param>
        /// <param name="maxValue">       The maximum value. </param>
        /// <param name="maxIsExclusive"> (Optional) True if maximum is exclusive. </param>
        ///
        /// <returns>
        ///     The next random float.
        /// </returns>
        ///
        /// <seealso cref="Velentr.Audio.Helpers.Randomizer.GetNextRandomFloat(float,float,bool)"/>
        public override float GetNextRandomFloat(float minValue, float maxValue, bool maxIsExclusive = true)
        {
            if (minValue.Equals(0f) && maxValue.Equals(1f))
            {
                return (float)r.NextDouble();
            }

            var value = (float)r.NextDouble();
            return value * GetNextRandomInt((int)Math.Floor(minValue), (int)Math.Ceiling(maxValue));
        }

        /// <summary>
        ///     Gets the next random int.
        /// </summary>
        ///
        /// <param name="minValue">       The minimum value. </param>
        /// <param name="maxValue">       The maximum value. </param>
        /// <param name="maxIsExclusive"> (Optional) True if maximum is exclusive. </param>
        ///
        /// <returns>
        ///     The next random int.
        /// </returns>
        ///
        /// <seealso cref="Velentr.Audio.Helpers.Randomizer.GetNextRandomInt(int,int,bool)"/>
        public override int GetNextRandomInt(int minValue, int maxValue, bool maxIsExclusive = true)
        {
            return r.Next(minValue, maxIsExclusive ? maxValue : maxValue + 1);
        }

        /// <summary>
        ///     Gets the next random long.
        /// </summary>
        ///
        /// <param name="minValue">       The minimum value. </param>
        /// <param name="maxValue">       The maximum value. </param>
        /// <param name="maxIsExclusive"> (Optional) True if maximum is exclusive. </param>
        ///
        /// <returns>
        ///     The next random long.
        /// </returns>
        ///
        /// <seealso cref="Velentr.Audio.Helpers.Randomizer.GetNextRandomLong(long,long,bool)"/>
        public override long GetNextRandomLong(long minValue, long maxValue, bool maxIsExclusive = true)
        {
            byte[] buf = new byte[8];
            r.NextBytes(buf);
            long longRand = BitConverter.ToInt64(buf, 0);
            maxValue = maxIsExclusive ? maxValue : maxValue + 1;
            return (Math.Abs(longRand % (maxValue - minValue)) + minValue);
        }

        /// <summary>
        ///     Updates the seed internal described by newSeed.
        /// </summary>
        ///
        /// <param name="newSeed"> The new seed. </param>
        ///
        /// <seealso cref="Velentr.Audio.Helpers.Randomizer.UpdateSeedInternal(long)"/>
        protected override void UpdateSeedInternal(long newSeed)
        {
            r = new Random((int)newSeed);
        }
    }
}