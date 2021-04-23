using Microsoft.Xna.Framework;
using Velentr.Audio.Helpers;
using IUpdateable = Velentr.Audio.Helpers.IUpdateable;

namespace Velentr.Audio.Procedural
{
    /// <summary>
    ///     Can be used to create/configure a procedural song
    /// </summary>
    ///
    /// <seealso cref="IUpdateable"/>
    public abstract class ProceduralSong : IUpdateable
    {
        /// <summary>
        ///     Specialized constructor for use only by derived class.
        /// </summary>
        ///
        /// <param name="manager">     The manager. </param>
        /// <param name="name">        The name. </param>
        /// <param name="defaultSeed"> The default seed. </param>
        protected ProceduralSong(AudioManager manager, string name, long defaultSeed)
        {
            DefaultSeed = defaultSeed;
            Name = name;
        }

        /// <summary>
        ///     Gets or sets the beats per minute.
        /// </summary>
        ///
        /// <value>
        ///     The beats per minute.
        /// </value>
        public int BeatsPerMinute { get; set; }

        /// <summary>
        ///     Gets the default seed.
        /// </summary>
        ///
        /// <value>
        ///     The default seed.
        /// </value>
        public long DefaultSeed { get; protected set; }

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
        ///     Gets or sets the randomizer.
        /// </summary>
        ///
        /// <value>
        ///     The randomizer.
        /// </value>
        public abstract Randomizer Randomizer { get; set; }

        /// <summary>
        ///     Resets the given seed.
        /// </summary>
        public void Reset()
        {
            Reset(DefaultSeed);
        }

        /// <summary>
        ///     Resets the given seed.
        /// </summary>
        ///
        /// <param name="seed"> The seed. </param>
        public abstract void Reset(long seed);

        /// <summary>
        ///     Updates the class.
        /// </summary>
        ///
        /// <param name="gameTime"> The game time. </param>
        ///
        /// <seealso cref="IUpdateable.Update(GameTime)"/>
        public abstract void Update(GameTime gameTime);
    }
}