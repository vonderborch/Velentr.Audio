using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Velentr.Audio.Playlists;
using Velentr.Audio.Tagging;

namespace Velentr.Audio.Procedural
{
    /// <summary>
    ///     A playlist that uses Procedural songs instead of regular songs.
    /// </summary>
    ///
    /// <seealso cref="Playlist"/>
    public class ProceduralPlaylist : Playlist
    {
        /// <summary>
        ///     The songs.
        /// </summary>
        private Dictionary<string, ProceduralSong> _songs;

        /// <summary>
        ///     Constructor.
        /// </summary>
        ///
        /// <param name="manager">                The manager. </param>
        /// <param name="name">                   The name. </param>
        /// <param name="randomize">              (Optional) True to randomize. </param>
        /// <param name="maxTriesToAvoidRepeats"> (Optional) The maximum tries to avoid repeats. </param>
        /// <param name="playlistValidTags">      (Optional) The playlist valid tags. </param>
        /// <param name="playlistPausedTags">     (Optional) The playlist paused tags. </param>
        internal ProceduralPlaylist(AudioManager manager, string name, bool randomize = true, int maxTriesToAvoidRepeats = 3, TagSet? playlistValidTags = null, TagSet? playlistPausedTags = null) : base(manager, name, randomize, maxTriesToAvoidRepeats, playlistValidTags, playlistPausedTags)
        {
            PlaylistType = PlaylistType.Procedural;
            _songs = new Dictionary<string, ProceduralSong>();
        }

        /// <summary>
        ///     Adds a procedural song to 'song'.
        /// </summary>
        ///
        /// <param name="name"> The name. </param>
        /// <param name="song"> The song. </param>
        public void AddProceduralSong(string name, ProceduralSong song)
        {
            _songs.Add(name.ToUpperInvariant(), song);
        }

        /// <summary>
        ///     Removes the procedural song described by name.
        /// </summary>
        ///
        /// <param name="name"> The name. </param>
        ///
        /// <returns>
        ///     True if it succeeds, false if it fails.
        /// </returns>
        public bool RemoveProceduralSong(string name)
        {
            name = name.ToUpperInvariant();
            if (_currentMusicName == name)
            {
                Stop();
            }

            return _songs.Remove(name);
        }

        /// <summary>
        ///     Updates the class.
        /// </summary>
        ///
        /// <param name="gameTime"> The game time. </param>
        ///
        /// <seealso cref="Velentr.Audio.Playlists.Playlist.Update(GameTime)"/>
        /// <seealso cref="IUpdateable.Update(GameTime)"/>
        public override void Update(GameTime gameTime)
        {
            var instances = Manager.GetAudioInstancesForCategory(AudioManager.MUSIC_CATEGORY);
            var instancestoKill = new List<ulong>();
            for (var i = 0; i < instances.Count; i++)
            {
                if (instances[i].State == SoundState.Stopped)
                {
                    instancestoKill.Add(instances[i].ID);
                    if (instances[i].Name.Equals(_currentMusicName))
                    {
                        _currentMusicName = string.Empty;
                    }
                }
                else if (instances[i].State == SoundState.Paused)
                {
                    instances[i].Resume();
                }
            }

            if (string.IsNullOrEmpty(_currentMusicName))
            {
                var categoryTags = Manager.MusicCategory.GetCurrentTags();
                var associatedMusic = Music.Values.ToList();
                if (_songs.Count == 0)
                {
                    _currentMusicName = string.Empty;
                }
                else
                {
                    var validMusic = new List<(PlaylistMusicInfo, List<Tag>)>();
                    for (var i = 0; i < associatedMusic.Count; i++)
                    {
                        if (Music[associatedMusic[i].Name].Tags.IsValid(categoryTags, out var validTags))
                        {
                            validMusic.Add((associatedMusic[i], validTags));
                        }
                    }

                    if (validMusic.Count == 0)
                    {
                        _currentMusicName = string.Empty;
                    }
                    else if (validMusic.Count == 1)
                    {
                        _currentMusicName = associatedMusic[0].Name;
                        _songs[_currentMusicName].Reset(Manager.Randomizer.GetNextRandomLong());
                    }
                    else
                    {
                        var itemId = 0;
                        if (Randomize)
                        {
                            for (var i = 0; i < MaxTriesToAvoidRepeats; i++)
                            {
                                itemId = Manager.Randomizer.GetNextRandomInt(0, associatedMusic.Count, true);
                                if (!_currentMusicName.Equals(associatedMusic[itemId].Name))
                                {
                                    break;
                                }
                            }
                        }
                        else
                        {
                            itemId = -1;
                            if (!string.IsNullOrEmpty(_currentMusicName))
                            {
                                itemId = associatedMusic.FindIndex(x => x.Name == _currentMusicName);
                            }

                            if (itemId == -1 || itemId >= associatedMusic.Count - 1)
                            {
                                itemId = 0;
                            }
                            else
                            {
                                itemId++;
                            }
                        }
                        _currentMusicName = associatedMusic[itemId].Name;
                        _songs[_currentMusicName].Reset(Manager.Randomizer.GetNextRandomLong());
                    }
                }
            }

            if (!string.IsNullOrEmpty(_currentMusicName))
            {
                _songs[_currentMusicName].Update(gameTime);
            }
        }

        /// <summary>
        ///     Play new procedural song.
        /// </summary>
        ///
        /// <param name="name"> The name. </param>
        public void PlayNewProceduralSong(string name)
        {
            _currentMusicName = name.ToUpperInvariant();
            _songs[_currentMusicName].Reset(Manager.Randomizer.GetNextRandomLong());
        }
    }
}
