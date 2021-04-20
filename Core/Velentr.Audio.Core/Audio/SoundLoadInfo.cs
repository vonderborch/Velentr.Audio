using System.Collections.Generic;
using Velentr.Audio.Tagging;

namespace Velentr.Audio.Audio
{
    /// <summary>
    ///     Information required to load a sound.
    /// </summary>
    public struct SoundLoadInfo
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        ///
        /// <param name="name">                     The name. </param>
        /// <param name="path">                     The full pathname of the file. </param>
        /// <param name="categories">               The categories. </param>
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
        /// <param name="tags">
        ///     (Optional)
        ///     The tags.
        /// </param>
        /// <param name="exclusionTags">            (Optional) The exclusion tags. </param>
        /// <param name="requiredTags">             (Optional) The required tags. </param>
        public SoundLoadInfo(string name, string path, List<string> categories, float defaultPitch = 0, bool autoLoad = false, bool loadOnlyToCreateInstance = false, List<string> tags = null, List<string> exclusionTags = null, List<string> requiredTags = null)
        {
            Name = name;
            Path = path;
            DefaultPitch = defaultPitch;
            AutoLoad = autoLoad;
            LoadOnlyToCreateInstance = loadOnlyToCreateInstance;
            Tags = new TagSet(tags, exclusionTags, requiredTags);
            Categories = new List<string>(categories);
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

        /// <summary>
        ///     The tags.
        /// </summary>
        public TagSet Tags;

        /// <summary>
        ///     The categories.
        /// </summary>
        public List<string> Categories;
    }
}
