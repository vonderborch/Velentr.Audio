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
        /// <param name="playlistType">           Type of the playlist. </param>
        /// <param name="randomize">
        ///     (Optional)
        ///     True if randomize, false if not.
        /// </param>
        /// <param name="maxTriesToAvoidRepeats">
        ///     (Optional)
        ///     The maximum tries to avoid repeats.
        /// </param>
        public PlaylistLoadInfo(string name, PlaylistType playlistType, bool randomize = true, int maxTriesToAvoidRepeats = 3) : this(name, playlistType, new TagSet(true), new TagSet(true), randomize, maxTriesToAvoidRepeats)
        {
        }

        public PlaylistLoadInfo(string name, PlaylistType playlistType, TagSet playlistValidTags, TagSet playlistPausedTags, bool randomize = true, int maxTriesToAvoidRepeats = 3)
        {
            _name = name.ToUpperInvariant();
            Randomize = randomize;
            MaxTriesToAvoidRepeats = maxTriesToAvoidRepeats;
            PlaylistPauasedTags = playlistPausedTags;
            PlaylistValidTags = playlistValidTags;
            MusicTracks = new List<PlaylistMusicInfo>();
            PlaylistType = playlistType;
        }

        public PlaylistLoadInfo(string name, PlaylistType playlistType, List<string> validPlaylistTags, List<string> validPlaylistExclusionTags, List<string> validPlaylistRequiredTags, List<string> pausePlaylistTags, List<string> pausePlaylistExclusionTags, List<string> pausePlaylistRequiredTags, bool randomize = true, int maxTriesToAvoidRepeats = 3) : this(name, playlistType, new TagSet(validPlaylistTags, validPlaylistExclusionTags, validPlaylistRequiredTags), new TagSet(pausePlaylistTags, pausePlaylistExclusionTags, pausePlaylistRequiredTags), randomize, maxTriesToAvoidRepeats)
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

        /// <summary>
        ///     Gets or sets the type of the playlist.
        /// </summary>
        ///
        /// <value>
        ///     The type of the playlist.
        /// </value>
        public PlaylistType PlaylistType { get; set; }

        /// <summary>
        ///     The music tracks.
        /// </summary>
        public List<PlaylistMusicInfo> MusicTracks;

    }
}
