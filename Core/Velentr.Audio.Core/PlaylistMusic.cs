using System.Collections.Generic;

namespace Velentr.Audio
{
    public struct PlaylistMusic
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        ///
        /// <param name="name"> The name. </param>
        /// <param name="tags">
        ///     (Optional) The tags. These are effectively 'Any' tags. If any exist in the current set of
        ///     tags associated with the manager, this music is valid.
        /// </param>
        /// <param name="exclusionTags">
        ///     (Optional)
        ///     The exclusion tags. These are the opposite of 'Any' tags. If any exists in the current
        ///     set of tags associated with the manager, this music is _not_ valid.
        /// </param>
        /// <param name="requiredTags">
        ///     (Optional) The required tags. These are effectively 'All' tags. All of these tags need to
        ///     exist in the current set of tags associated with the manager for this music to be valid.
        /// </param>
        public PlaylistMusic(string name, List<string> tags = null, List<string> exclusionTags = null, List<string> requiredTags = null)
        {
            Name = name;
            Tags = new TagSet(tags, exclusionTags, requiredTags);
        }

        public string Name { get; }

        public TagSet Tags { get; set; }
    }
}
