namespace Velentr.Audio.Audio
{
    /// <summary>
    ///     Information required to load a music track.
    /// </summary>
    public struct MusicLoadInfo
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        ///
        /// <param name="name">                     The name. </param>
        /// <param name="path">                     The full pathname of the file. </param>
        /// <param name="defaultPitch">
        ///     (Optional)
        ///     The default pitch.
        /// </param>
        /// <param name="autoLoad">
        ///     (Optional)
        ///     True if automatic load, false if not.
        /// </param>
        /// <param name="loadOnlyToCreateInstance">
        ///     (Optional)
        ///     True if load only to create instance, false if not.
        /// </param>
        public MusicLoadInfo(string name, string path, float defaultPitch = 0, bool autoLoad = false, bool loadOnlyToCreateInstance = false)
        {
            Name = name;
            Path = path;
            DefaultPitch = defaultPitch;
            AutoLoad = autoLoad;
            LoadOnlyToCreateInstance = loadOnlyToCreateInstance;
        }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        ///
        /// <value>
        ///     The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the full pathname of the file.
        /// </summary>
        ///
        /// <value>
        ///     The full pathname of the file.
        /// </value>
        public string Path { get; set; }

        /// <summary>
        ///     Gets or sets the default pitch.
        /// </summary>
        ///
        /// <value>
        ///     The default pitch.
        /// </value>
        public float DefaultPitch { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the automatic load.
        /// </summary>
        ///
        /// <value>
        ///     True if automatic load, false if not.
        /// </value>
        public bool AutoLoad { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the only to create instance should be loaded.
        /// </summary>
        ///
        /// <value>
        ///     True if load only to create instance, false if not.
        /// </value>
        public bool LoadOnlyToCreateInstance { get; set; }
    }
}
