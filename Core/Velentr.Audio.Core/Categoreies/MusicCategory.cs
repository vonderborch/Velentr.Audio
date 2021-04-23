using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Velentr.Audio.Helpers;
using Velentr.Audio.Playlists;
using Velentr.Audio.Tagging;

namespace Velentr.Audio.Categoreies
{
    /// <summary>
    ///     A music category.
    /// </summary>
    ///
    /// <seealso cref="Category"/>
    public class MusicCategory : Category
    {
        /// <summary>
        ///     The current playlist.
        /// </summary>
        private string _currentPlaylist;

        /// <summary>
        ///     The playlists.
        /// </summary>
        internal Dictionary<string, Playlist> _playlists;

        /// <summary>
        ///     The playlist tags.
        /// </summary>
        private List<Tag> _playlistTags;

        /// <summary>
        ///     The registered playlist instances.
        /// </summary>
        private Dictionary<string, List<ulong>> _registeredPlaylistInstances;

        /// <summary>
        ///     Constructor.
        /// </summary>
        ///
        /// <param name="manager">         The manager. </param>
        /// <param name="name">            The name. </param>
        /// <param name="isMusicCategory"> (Optional) True if is music category, false if not. </param>
        /// <param name="volume">          (Optional) The volume. </param>
        internal MusicCategory(AudioManager manager, string name, bool isMusicCategory = false, float volume = 1) : base(manager, name, isMusicCategory, volume)
        {
            _currentPlaylist = string.Empty;
            _playlists = new Dictionary<string, Playlist>();
            _registeredPlaylistInstances = new Dictionary<string, List<ulong>>();
            _playlistTags = new List<Tag>();
        }

        /// <summary>
        ///     Gets or sets the current playlist.
        /// </summary>
        ///
        /// <value>
        ///     The current playlist.
        /// </value>
        public string CurrentPlaylist
        {
            get => _currentPlaylist;
            set => ChangePlaylist(value);
        }

        /// <summary>
        ///     Adds a playlist.
        /// </summary>
        ///
        /// <param name="playlist"> The playlist. </param>
        public void AddPlaylist(Playlist playlist)
        {
            _playlists.Add(playlist.Name, playlist);
            _registeredPlaylistInstances.Add(playlist.Name, new List<ulong>());
        }

        /// <summary>
        ///     Adds a playlist tag.
        /// </summary>
        ///
        /// <param name="tag"> The tag. </param>
        public void AddPlaylistChoiceTag(Tag tag)
        {
            _playlistTags.Add(tag);
        }

        /// <summary>
        ///     Adds a playlist tag.
        /// </summary>
        ///
        /// <param name="tags"> The tags. </param>
        public void AddPlaylistChoiceTag(List<Tag> tags)
        {
            _playlistTags.AddRange(tags);
        }

        /// <summary>
        ///     Adds a playlist tag.
        /// </summary>
        ///
        /// <param name="name">                 The name. </param>
        /// <param name="consumable">           True if consumable. </param>
        /// <param name="lifespanMilliseconds"> The lifespan in milliseconds. </param>
        public void AddPlaylistChoiceTag(string name, bool consumable, uint lifespanMilliseconds)
        {
            _playlistTags.Add(new Tag(name, consumable, lifespanMilliseconds));
        }

        /// <summary>
        ///     Change playlist.
        /// </summary>
        ///
        /// <exception cref="Exception"> Thrown when an exception error condition occurs. </exception>
        ///
        /// <param name="newPlaylist"> The new playlist. </param>
        public void ChangePlaylist(string newPlaylist)
        {
            if (!string.IsNullOrEmpty(newPlaylist))
            {
                if (!_playlists.ContainsKey(newPlaylist))
                {
                    throw new Exception("Invalid playlist!");
                }

                // pause or stop the current playlist depending on whether the playlist should be stopped or paused...
                if (!string.IsNullOrEmpty(_currentPlaylist))
                {
                    if (_playlists[_currentPlaylist].ShouldPlaylistBeStopped(GetCurrentPlaylistTags()))
                    {
                        _playlists[_currentPlaylist].Stop();
                    }
                    else
                    {
                        _playlists[_currentPlaylist].Pause();
                    }
                }

                _currentPlaylist = newPlaylist;
            }
        }

        public void ChooseNewPlaylist()
        {
            // first let's look through all of the playlists and find all the possible valid playlists...
            var validPlaylists = new List<string>();
            var pausedValidPlaylists = new List<string>();
            var playlistsToStop = new List<string>();
            foreach (var playlist in _playlists)
            {
                if (playlist.Value.ShouldPlaylistBePlayed(GetCurrentPlaylistTags()))
                {
                    validPlaylists.Add(playlist.Key);

                    if (playlist.Value.IsPaused)
                    {
                        pausedValidPlaylists.Add(playlist.Key);
                    }
                }
                if (playlist.Value.IsPaused && playlist.Value.ShouldPlaylistBeStopped(GetCurrentPlaylistTags()))
                {
                    playlistsToStop.Add(playlist.Key);
                }
            }

            // stop any playlists that are paused and are no longer valid but were paused
            for (var i = 0; i < playlistsToStop.Count; i++)
            {
                _playlists[playlistsToStop[i]].Stop();
            }

            // determine the next playlist to play...
            var newPlaylist = _currentPlaylist;
            switch (validPlaylists.Count)
            {
                case 0:
                    break;

                case 1:
                    newPlaylist = validPlaylists[0];
                    break;

                default:
                    // check if any are paused...
                    if (pausedValidPlaylists.Count == 1)
                    {
                        newPlaylist = pausedValidPlaylists[0];
                    }
                    else if (pausedValidPlaylists.Count > 1)
                    {
                        newPlaylist = pausedValidPlaylists[Manager.Randomizer.GetNextRandomInt(0, validPlaylists.Count, false)];
                    }
                    else
                    {
                        newPlaylist = validPlaylists[Manager.Randomizer.GetNextRandomInt(0, validPlaylists.Count, false)];
                    }
                    break;
            }

            ChangePlaylist(newPlaylist);
        }

        /// <summary>
        ///     Gets current playlist tags.
        /// </summary>
        ///
        /// <returns>
        ///     The current playlist tags.
        /// </returns>
        public List<Tag> GetCurrentPlaylistTags()
        {
            return new List<Tag>(_playlistTags);
        }

        /// <summary>
        ///     Gets playlist audio instances.
        /// </summary>
        ///
        /// <param name="playlist"> The playlist. </param>
        ///
        /// <returns>
        ///     The playlist audio instances.
        /// </returns>
        public List<ulong> GetPlaylistAudioInstances(string playlist)
        {
            return new List<ulong>(_registeredPlaylistInstances[_currentPlaylist]);
        }

        /// <summary>
        ///     Registers the instance described by ID.
        /// </summary>
        ///
        /// <param name="id"> The identifier. </param>
        ///
        /// <seealso cref="Velentr.Audio.Categoreies.Category.RegisterInstance(ulong)"/>
        public override void RegisterInstance(ulong id)
        {
            base.RegisterInstance(id);
            _registeredPlaylistInstances[_currentPlaylist].Add(id);
        }

        public bool RemovePlaylist(string name)
        {
            var result = _playlists.Remove(name);
            if (CurrentPlaylist == name)
            {
                _currentPlaylist = string.Empty;
                ChooseNewPlaylist();
            }

            return result;
        }

        /// <summary>
        ///     Removes the playlist tag.
        /// </summary>
        ///
        /// <param name="name">                    The name. </param>
        /// <param name="removeOnlyFirstinstance"> (Optional) True to remove only firstinstance. </param>
        ///
        /// <returns>
        ///     True if it succeeds, false if it fails.
        /// </returns>
        public bool RemovePlaylistChoiceTag(string name, bool removeOnlyFirstinstance = true)
        {
            var indexesToRemove = new List<int>();
            for (var i = 0; i < _playlistTags.Count; i++)
            {
                if (_playlistTags[i].Name.Equals(name))
                {
                    indexesToRemove.Add(i);
                    if (removeOnlyFirstinstance)
                    {
                        return true;
                    }
                }
            }

            for (var i = indexesToRemove.Count - 1; i >= 0; i--)
            {
                _playlistTags.RemoveAt(indexesToRemove[i]);
            }

            return indexesToRemove.Count != 0;
        }

        /// <summary>
        ///     Removes the playlist tag.
        /// </summary>
        ///
        /// <param name="names">                   The names. </param>
        /// <param name="removeOnlyFirstinstance"> (Optional) True to remove only firstinstance. </param>
        ///
        /// <returns>
        ///     True if it succeeds, false if it fails.
        /// </returns>
        public List<bool> RemovePlaylistChoiceTag(List<string> names, bool removeOnlyFirstinstance = true)
        {
            var output = new List<bool>();
            for (var i = 0; i < names.Count; i++)
            {
                output.Add(RemoveTag(names[i], removeOnlyFirstinstance));
            }

            return output;
        }

        /// <summary>
        ///     Removes the playlist tag.
        /// </summary>
        ///
        /// <param name="tags"> The tags. </param>
        ///
        /// <returns>
        ///     True if it succeeds, false if it fails.
        /// </returns>
        public List<bool> RemovePlaylistChoiceTag(List<(string, bool)> tags)
        {
            var output = new List<bool>();
            for (var i = 0; i < tags.Count; i++)
            {
                output.Add(RemoveTag(tags[i].Item1, tags[i].Item2));
            }

            return output;
        }

        /// <summary>
        ///     Un register instance.
        /// </summary>
        ///
        /// <param name="id"> The identifier. </param>
        ///
        /// <seealso cref="Velentr.Audio.Categoreies.Category.UnRegisterInstance(ulong)"/>
        public override void UnRegisterInstance(ulong id)
        {
            base.UnRegisterInstance(id);
            _registeredPlaylistInstances[_currentPlaylist].Remove(id);
        }

        /// <summary>
        ///     Updates the given gameTime.
        /// </summary>
        ///
        /// <param name="gameTime"> The game time. </param>
        ///
        /// <seealso cref="Velentr.Audio.Categoreies.Category.Update(GameTime)"/>
        public override void Update(GameTime gameTime)
        {
            // first let's check if we need to play a new playlist...
            bool playNewPlaylist = string.IsNullOrEmpty(_currentPlaylist);
            if (!playNewPlaylist)
            {
                playNewPlaylist = !_playlists[_currentPlaylist].ShouldPlaylistBePlayed(GetCurrentPlaylistTags());
            }

            if (playNewPlaylist)
            {
                ChooseNewPlaylist();
            }

            // next, let's update the current playlist
            if (_currentPlaylist != string.Empty)
            {
                _playlists[_currentPlaylist].Update(gameTime);
            }

            // go through all tags and clean them up as required
            var tagsToRemove = new List<int>();
            for (var i = 0; i < _currentTags.Count; i++)
            {
                if (TimeHelpers.ElapsedMilliSeconds(_currentTags[i].CreationTime, gameTime) > _currentTags[i].LifespanMilliseconds)
                {
                    tagsToRemove.Add(i);
                }
            }

            for (var i = tagsToRemove.Count - 1; i >= 0; i--)
            {
                _currentTags.RemoveAt(tagsToRemove[i]);
            }

            // go through all playlist tags and clean them up as required
            tagsToRemove = new List<int>();
            for (var i = 0; i < _playlistTags.Count; i++)
            {
                if (TimeHelpers.ElapsedMilliSeconds(_playlistTags[i].CreationTime, gameTime) > _playlistTags[i].LifespanMilliseconds)
                {
                    tagsToRemove.Add(i);
                }
            }

            for (var i = tagsToRemove.Count - 1; i >= 0; i--)
            {
                _playlistTags.RemoveAt(tagsToRemove[i]);
            }
        }
    }
}