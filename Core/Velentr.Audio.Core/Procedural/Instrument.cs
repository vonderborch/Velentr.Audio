namespace Velentr.Audio.Procedural
{
    /// <summary>
    ///     Represents an Instrument that can be played.
    /// </summary>
    public abstract class Instrument
    {
        /// <summary>
        ///     Specialized constructor for use only by derived class.
        /// </summary>
        ///
        /// <param name="manager"> The manager. </param>
        /// <param name="name">    The name. </param>
        protected Instrument(AudioManager manager, string name)
        {
            Name = name;
            Manager = manager;
        }

        /// <summary>
        ///     Gets the manager.
        /// </summary>
        ///
        /// <value>
        ///     The manager.
        /// </value>
        public AudioManager Manager { get; }

        /// <summary>
        ///     Gets the name.
        /// </summary>
        ///
        /// <value>
        ///     The name.
        /// </value>
        public string Name { get; }

        /// <summary>
        ///     Resets the given seed.
        /// </summary>
        ///
        /// <param name="seed"> The seed. </param>
        public abstract void Reset(long seed);

        /// <summary>
        ///     Updates the given millisecondsSinceLastTick.
        /// </summary>
        ///
        /// <param name="millisecondsSinceLastTick"> The milliseconds since last tick. </param>
        public abstract void Update(uint millisecondsSinceLastTick);
    }
}