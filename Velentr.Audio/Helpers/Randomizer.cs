namespace Velentr.Audio.Helpers
{
    /// <summary>
    ///     A randomizer.
    /// </summary>
    public abstract class Randomizer
    {
        /// <summary>
        ///     The seed.
        /// </summary>
        protected long _seed;

        /// <summary>
        ///     Specialized constructor for use only by derived class.
        /// </summary>
        ///
        /// <param name="seed">
        ///     (Optional)
        ///     The seed.
        /// </param>
        protected Randomizer(long? seed = null)
        {
        }

        /// <summary>
        ///     Gets or sets the seed.
        /// </summary>
        ///
        /// <value>
        ///     The seed.
        /// </value>
        public long Seed
        {
            get => _seed;
            set
            {
                _seed = value;
                UpdateSeedInternal(value);
            }
        }

        /// <summary>
        ///     Gets the next random double.
        /// </summary>
        ///
        /// <returns>
        ///     The next random double.
        /// </returns>
        public double GetNextRandomDouble()
        {
            return GetNextRandomDouble(0f, 1f, false);
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
        public abstract double GetNextRandomDouble(double minValue, double maxValue, bool maxIsExclusive = true);

        /// <summary>
        ///     Gets the next random float.
        /// </summary>
        ///
        /// <returns>
        ///     The next random float.
        /// </returns>
        public float GetNextRandomFloat()
        {
            return GetNextRandomFloat(0f, 1f, false);
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
        public abstract float GetNextRandomFloat(float minValue, float maxValue, bool maxIsExclusive = true);

        /// <summary>
        ///     Gets the next random int.
        /// </summary>
        ///
        /// <returns>
        ///     The next random int.
        /// </returns>
        public int GetNextRandomInt()
        {
            return GetNextRandomInt(int.MinValue, int.MaxValue, false);
        }

        /// <summary>
        ///     Gets the next random int.
        /// </summary>
        ///
        /// <param name="maxValue"> The maximum value. </param>
        ///
        /// <returns>
        ///     The next random int.
        /// </returns>
        public int GetNextRandomInt(int maxValue)
        {
            return GetNextRandomInt(0, maxValue, false);
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
        public abstract int GetNextRandomInt(int minValue, int maxValue, bool maxIsExclusive = true);

        /// <summary>
        ///     Gets the next random long.
        /// </summary>
        ///
        /// <returns>
        ///     The next random long.
        /// </returns>
        public long GetNextRandomLong()
        {
            return GetNextRandomLong(long.MinValue, long.MaxValue, true);
        }

        /// <summary>
        ///     Gets the next random long.
        /// </summary>
        ///
        /// <param name="maxValue"> The maximum value. </param>
        ///
        /// <returns>
        ///     The next random long.
        /// </returns>
        public long GetNextRandomLong(long maxValue)
        {
            return GetNextRandomLong(0, maxValue, false);
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
        public abstract long GetNextRandomLong(long minValue, long maxValue, bool maxIsExclusive = true);

        /// <summary>
        ///     Updates the seed described by newSeed.
        /// </summary>
        ///
        /// <param name="newSeed"> The new seed. </param>
        public void UpdateSeed(long newSeed)
        {
            _seed = newSeed;
            UpdateSeedInternal(newSeed);
        }

        /// <summary>
        ///     Updates the seed internal described by newSeed.
        /// </summary>
        ///
        /// <param name="newSeed"> The new seed. </param>
        protected abstract void UpdateSeedInternal(long newSeed);
    }
}