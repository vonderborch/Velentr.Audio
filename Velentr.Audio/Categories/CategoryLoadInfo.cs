namespace Velentr.Audio.Categories
{
    /// <summary>
    ///     Information required to create a new category
    /// </summary>
    public struct CategoryLoadInfo
    {
        /// <summary>
        ///     The name.
        /// </summary>
        private string _name;

        /// <summary>
        ///     Constructor.
        /// </summary>
        ///
        /// <param name="name">   The name. </param>
        /// <param name="volume"> The volume. </param>
        public CategoryLoadInfo(string name, float volume)
        {
            _name = name.ToUpperInvariant();
            Volume = volume;
        }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        ///
        /// <value>
        ///     The name.
        /// </value>
        public string Name
        {
            get => _name;
            set => _name = value.ToUpperInvariant();
        }

        /// <summary>
        ///     Gets or sets the volume.
        /// </summary>
        ///
        /// <value>
        ///     The volume.
        /// </value>
        public float Volume { get; set; }
    }
}