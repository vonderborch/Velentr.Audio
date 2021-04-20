namespace Velentr.Audio.Audio
{
    /// <summary>
    ///     A music.
    /// </summary>
    ///
    /// <seealso cref="Audio"/>
    public class Music : Audio
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        ///
        /// <param name="manager">                  The manager. </param>
        /// <param name="name">                     The name. </param>
        /// <param name="path">                     Full pathname of the file. </param>
        /// <param name="defaultPitch">             (Optional) The default pitch. </param>
        /// <param name="autoLoad">                 (Optional) True to automatically load. </param>
        /// <param name="loadOnlyToCreateInstance">
        ///     (Optional) True to load only to create instance.
        /// </param>
        public Music(AudioManager manager, string name, string path, float defaultPitch = 0, bool autoLoad = false, bool loadOnlyToCreateInstance = false) : base(manager, name, path, defaultPitch, autoLoad, loadOnlyToCreateInstance)
        {

        }

    }
}
