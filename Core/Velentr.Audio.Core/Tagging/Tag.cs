using System;

namespace Velentr.Audio.Tagging
{
    /// <summary>
    ///     A tag.
    /// </summary>
    public readonly struct Tag
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        ///
        /// <param name="name">                 The name. </param>
        /// <param name="consumable">           True if consumable. </param>
        /// <param name="lifespanMilliseconds"> The lifespan in milliseconds. </param>
        public Tag(string name, bool consumable, uint lifespanMilliseconds)
        {
            Name = name;
            Consumable = consumable;
            LifespanMilliseconds = lifespanMilliseconds;
            CreationTime = AudioManager.LastUpdateTime;
        }

        /// <summary>
        ///     Gets a value indicating whether this object is consumable.
        /// </summary>
        ///
        /// <value>
        ///     True if consumable, false if not.
        /// </value>
        public bool Consumable { get; }

        /// <summary>
        ///     Gets the creation time.
        /// </summary>
        ///
        /// <value>
        ///     The creation time.
        /// </value>
        public TimeSpan CreationTime { get; }

        /// <summary>
        ///     Gets the lifespan milliseconds.
        /// </summary>
        ///
        /// <value>
        ///     The lifespan milliseconds.
        /// </value>
        public uint LifespanMilliseconds { get; }

        /// <summary>
        ///     Gets the name.
        /// </summary>
        ///
        /// <value>
        ///     The name.
        /// </value>
        public string Name { get; }
    }
}