using System.Collections.Generic;
using Velentr.Audio.Tagging;

namespace Velentr.Audio.Playlists
{
    /// <summary>
    ///     Represents music in a playlist
    /// </summary>
    public class PlaylistMusic
    {
        /// <summary>
        ///     The name.
        /// </summary>
        private string _name;

        /// <summary>
        ///     The tags.
        /// </summary>
        public TagSet Tags;

        /// <summary>
        ///     Constructor.
        /// </summary>
        ///
        /// <param name="name"> The name. </param>
        /// <param name="tags"> The tags. </param>
        public PlaylistMusic(string name, TagSet tags)
        {
            _name = name.ToUpperInvariant();
            Tags = tags;
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        ///
        /// <param name="name">          The name. </param>
        /// <param name="tags">          (Optional) The tags. </param>
        /// <param name="exclusionTags"> (Optional) The exclusion tags. </param>
        /// <param name="requiredTags">  (Optional) The required tags. </param>
        public PlaylistMusic(string name, List<string> tags = null, List<string> exclusionTags = null, List<string> requiredTags = null)
        {
            _name = name.ToUpperInvariant();
            Tags = new TagSet(tags, exclusionTags, requiredTags);
        }

        /// <summary>
        ///     Gets the name.
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
    }
}