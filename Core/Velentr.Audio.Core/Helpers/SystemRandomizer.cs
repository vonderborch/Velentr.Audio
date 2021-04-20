using System;

namespace Velentr.Audio.Helpers
{
    public class SystemRandomizer : Randomizer
    {

        private Random r;

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

        protected override void UpdateSeedInternal(long newSeed)
        {
            r = new Random((int)newSeed);
        }

        public override int GetNextRandomInt(int minValue, int maxValue, bool maxIsExclusive = true)
        {
            return r.Next(minValue, maxValue + 1);
        }

        public override long GetNextRandomLong(long minValue, long maxValue, bool maxIsExclusive = true)
        {
            return r.Next((int)minValue, (int)maxValue + 1);
        }

        public override float GetNextRandomFloat(float minValue, float maxValue, bool maxIsExclusive = true)
        {
            if (minValue.Equals(0f) && maxValue.Equals(1f))
            {
                return (float)r.NextDouble();
            }

            var value = (float) r.NextDouble();
            return value * GetNextRandomInt((int)Math.Floor(minValue), (int)Math.Ceiling(maxValue));
        }

        public override double GetNextRandomDouble(double minValue, double maxValue, bool maxIsExclusive = true)
        {
            if (minValue.Equals(0d) && maxValue.Equals(1d))
            {
                return r.NextDouble();
            }

            var value = r.NextDouble();
            return value * GetNextRandomLong((long)Math.Floor(minValue), (long)Math.Ceiling(maxValue));
        }

    }
}
