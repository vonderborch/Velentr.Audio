using Microsoft.Xna.Framework;

namespace Velentr.Audio.Helpers
{
    /// <summary>
    ///     Interface for classes needing to be updated
    /// </summary>
    public interface IUpdateable
    {
        /// <summary>
        ///     Updates the class.
        /// </summary>
        ///
        /// <param name="gameTime"> The game time. </param>
        void Update(GameTime gameTime);
    }
}