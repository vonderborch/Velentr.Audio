
namespace Velentr.Audio.Helpers
{
    public abstract class Randomizer
    {

        protected long _seed;

        protected Randomizer(long? seed = null)
        {

        }

        public long Seed
        {
            get => _seed;
            set
            {
                _seed = value;
                UpdateSeedInternal(value);
            }
        }

        public void UpdateSeed(long newSeed)
        {
            _seed = newSeed;
            UpdateSeedInternal(newSeed);
        }

        protected abstract void UpdateSeedInternal(long newSeed);

        public int GetNextRandomInt()
        {
            return GetNextRandomInt(int.MinValue, int.MaxValue, false);
        }

        public int GetNextRandomInt(int maxValue)
        {
            return GetNextRandomInt(0, maxValue, false);
        }

        public abstract int GetNextRandomInt(int minValue, int maxValue, bool maxIsExclusive = true);

        public long GetNextRandomLong()
        {
            return GetNextRandomLong(long.MinValue, long.MaxValue, false);
        }

        public long GetNextRandomLong(long maxValue)
        {
            return GetNextRandomLong(0, maxValue, false);
        }

        public abstract long GetNextRandomLong(long minValue, long maxValue, bool maxIsExclusive = true);

        public float GetNextRandomFloat()
        {
            return GetNextRandomFloat(0f, 1f, false);
        }

        public abstract float GetNextRandomFloat(float minValue, float maxValue, bool maxIsExclusive = true);

        public double GetNextRandomDouble()
        {
            return GetNextRandomDouble(0f, 1f, false);
        }

        public abstract double GetNextRandomDouble(double minValue, double maxValue, bool maxIsExclusive = true);

    }
}
