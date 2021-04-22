using System.Collections.Generic;
using Velentr.Audio.Tagging;

namespace Velentr.Audio.Playlists
{
    /// <summary>
    ///     Information required to create a playlist.
    /// </summary>
    public class PlaylistLoadInfo
    {
        /// <summary>
        ///     The name.
        /// </summary>
        private string _name;

        /// <summary>
        ///     Constructor.
        /// </summary>
        ///
        /// <param name="name">                   The name. </param>
        /// <param name="randomize">
        ///     (Optional)
        ///     True if randomize, false if not.
        /// </param>
        /// <param name="maxTriesToAvoidRepeats">
        ///     (Optional)
        ///     The maximum tries to avoid repeats.
        /// </param>
        public PlaylistLoadInfo(string name, bool randomize = true, int maxTriesToAvoidRepeats = 3) : this(name, new TagSet(), new TagSet(), randomize, maxTriesToAvoidRepeats)
        {
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        ///
        /// <param name="name">                   The name. </param>
        /// <param name="playlistValidTags">      The playlist valid tags. </param>
        /// <param name="playlistPausedTags">     The playlist paused tags. </param>
        /// <param name="randomize">
        ///     (Optional)
        ///     True if randomize, false if not.
        /// </param>
        /// <param name="maxTriesToAvoidRepeats">
        ///     (Optional)
        ///     The maximum tries to avoid repeats.
        /// </param>
        public PlaylistLoadInfo(string name, TagSet playlistValidTags, TagSet playlistPausedTags, bool randomize = true, int maxTriesToAvoidRepeats = 3)
        {
            _name = name.ToUpperInvariant();
            Randomize = randomize;
            MaxTriesToAvoidRepeats = maxTriesToAvoidRepeats;
            PlaylistPauasedTags = playlistPausedTags;
            PlaylistValidTags = playlistValidTags;
            MusicTracks = new List<PlaylistMusic>();
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        ///
        /// <param name="name">                       The name. </param>
        /// <param name="validPlaylistTags">          The valid playlist tags. </param>
        /// <param name="validPlaylistExclusionTags"> The valid playlist exclusion tags. </param>
        /// <param name="validPlaylistRequiredTags">  The valid playlist required tags. </param>
        /// <param name="pausePlaylistTags">          The pause playlist tags. </param>
        /// <param name="pausePlaylistExclusionTags"> The pause playlist exclusion tags. </param>
        /// <param name="pausePlaylistRequiredTags">  The pause playlist required tags. </param>
        /// <param name="randomize">
        ///     (Optional)
        ///     True if randomize, false if not.
        /// </param>
        /// <param name="maxTriesToAvoidRepeats">
        ///     (Optional)
        ///     The maximum tries to avoid repeats.
        /// </param>
        public PlaylistLoadInfo(string name, List<string> validPlaylistTags, List<string> validPlaylistExclusionTags, List<string> validPlaylistRequiredTags, List<string> pausePlaylistTags, List<string> pausePlaylistExclusionTags, List<string> pausePlaylistRequiredTags, bool randomize = true, int maxTriesToAvoidRepeats = 3) : this(name, new TagSet(validPlaylistTags, validPlaylistExclusionTags, validPlaylistRequiredTags), new TagSet(pausePlaylistTags, pausePlaylistExclusionTags, pausePlaylistRequiredTags), randomize, maxTriesToAvoidRepeats)
        {
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
        ///     Gets or sets a value indicating whether the randomize.
        /// </summary>
        ///
        /// <value>
        ///     True if randomize, false if not.
        /// </value>
        public bool Randomize { get; set; }

        /// <summary>
        ///     Gets or sets the maximum tries to avoid repeats.
        /// </summary>
        ///
        /// <value>
        ///     The maximum tries to avoid repeats.
        /// </value>
        public int MaxTriesToAvoidRepeats { get; set; }

        /// <summary>
        ///     The playlist valid tags.
        /// </summary>
        public TagSet PlaylistValidTags;

        /// <summary>
        ///     The playlist pauased tags.
        /// </summary>
        public TagSet PlaylistPauasedTags;

        public List<PlaylistMusic> MusicTracks;

    }
}
