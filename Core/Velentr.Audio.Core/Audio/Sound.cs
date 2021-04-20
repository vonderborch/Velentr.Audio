using System.Collections.Generic;
using Velentr.Audio.Tagging;

namespace Velentr.Audio.Audio
{
    /// <summary>
    ///     A sound.
    /// </summary>
    ///
    /// <seealso cref="Audio"/>
    public class Sound : Audio
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        ///
        /// <param name="manager">                  The manager. </param>
        /// <param name="name">                     The name. </param>
        /// <param name="path">                     Full pathname of the file. </param>
        /// <param name="categories">               The categories. </param>
        /// <param name="defaultPitch">             (Optional) The default pitch. </param>
        /// <param name="autoLoad">                 (Optional) True to automatically load. </param>
        /// <param name="loadOnlyToCreateInstance">
        ///     (Optional) True to load only to create instance.
        /// </param>
        /// <param name="tags">                     (Optional) The tags. </param>
        /// <param name="exclusionTags">            (Optional) The exclusion tags. </param>
        /// <param name="requiredTags">             (Optional) The required tags. </param>
        public Sound(AudioManager manager, string name, string path, List<string> categories, float defaultPitch = 0, bool autoLoad = false, bool loadOnlyToCreateInstance = false, List<string> tags = null, List<string> exclusionTags = null, List<string> requiredTags = null) : base(manager, name, path, defaultPitch, autoLoad, loadOnlyToCreateInstance)
        {
            Categories = new List<string>(categories);
            Tags = new TagSet(tags, exclusionTags, requiredTags);
        }

        /// <summary>
        ///     The categories.
        /// </summary>
        public List<string> Categories;

        /// <summary>
        ///     The tags.
        /// </summary>
        public TagSet Tags;
    }
}
