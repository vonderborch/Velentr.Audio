using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using System.Linq;
using Velentr.Audio.Sounds;
using Velentr.Audio.Tagging;
using IUpdateable = Velentr.Audio.Helpers.IUpdateable;

namespace Velentr.Audio.Playlists
{
    /// <summary>
    ///     List of plays.
    /// </summary>
    ///
    /// <seealso cref="Velentr.Audio.Helpers.IUpdateable"/>
    public class Playlist : IUpdateable
    {
        /// <summary>
        ///     The music.
        /// </summary>
        public Dictionary<string, PlaylistMusic> _music;
        public TagSet PlaylistPausedTags;
        public TagSet PlaylistValidTags;
        private string _currentMusicName;

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
        internal Playlist(AudioManager manager, string name, bool randomize = true, int maxTriesToAvoidRepeats = 3, TagSet? playlistValidTags = null, TagSet? playlistPausedTags = null)
        {
            Manager = manager;
            Name = name;
            Randomize = randomize;
            MaxTriesToAvoidRepeats = maxTriesToAvoidRepeats;

            _currentMusicName = string.Empty;
            PlaylistValidTags = playlistValidTags ?? new TagSet();
            PlaylistPausedTags = playlistPausedTags ?? new TagSet();
            _music = new Dictionary<string, PlaylistMusic>();
            IsPaused = false;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this object is paused.
        /// </summary>
        ///
        /// <value>
        ///     True if this object is paused, false if not.
        /// </value>
        public bool IsPaused { get; internal set; }

        /// <summary>
        ///     Gets the manager.
        /// </summary>
        ///
        /// <value>
        ///     The manager.
        /// </value>
        public AudioManager Manager { get; }

        /// <summary>
        ///     Gets or sets the maximum tries to avoid repeats.
        /// </summary>
        ///
        /// <value>
        ///     The maximum tries to avoid repeats.
        /// </value>
        public int MaxTriesToAvoidRepeats { get; set; }

        /// <summary>
        ///     Gets the name.
        /// </summary>
        ///
        /// <value>
        ///     The name.
        /// </value>
        public string Name { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether the randomize.
        /// </summary>
        ///
        /// <value>
        ///     True if randomize, false if not.
        /// </value>
        public bool Randomize { get; set; }

        /// <summary>
        ///     Adds a playlist music to 'tags'.
        /// </summary>
        ///
        /// <param name="name"> The name. </param>
        /// <param name="tags"> The tags. </param>
        public void AddPlaylistMusic(string name, TagSet tags)
        {
            AddPlaylistMusic(new PlaylistMusic(name, tags));
        }

        /// <summary>
        ///     Adds a playlist music to 'tags'.
        /// </summary>
        ///
        /// <param name="name">          The name. </param>
        /// <param name="tags">          (Optional) The tags. </param>
        /// <param name="exclusionTags"> (Optional) The exclusion tags. </param>
        /// <param name="requiredTags">  (Optional) The required tags. </param>
        public void AddPlaylistMusic(string name, List<string> tags = null, List<string> exclusionTags = null, List<string> requiredTags = null)
        {
            AddPlaylistMusic(new PlaylistMusic(name, tags, exclusionTags, requiredTags));
        }

        /// <summary>
        ///     Adds a playlist music to 'tags'.
        /// </summary>
        ///
        /// <param name="music"> The music. </param>
        public void AddPlaylistMusic(PlaylistMusic music)
        {
            _music.Add(music.Name, music);
        }

        /// <summary>
        ///     Adds a playlist paused tag.
        /// </summary>
        ///
        /// <param name="tag">     The tag. </param>
        /// <param name="tagType"> Type of the tag. </param>
        public void AddPlaylistPausedTag(string tag, TagType tagType)
        {
            PlaylistPausedTags.AddTag(tag, tagType);
        }

        /// <summary>
        ///     Adds a playlist paused tag.
        /// </summary>
        ///
        /// <param name="tags">    The tags. </param>
        /// <param name="tagType"> Type of the tag. </param>
        public void AddPlaylistPausedTag(List<string> tags, TagType tagType)
        {
            PlaylistPausedTags.AddTag(tags, tagType);
        }

        /// <summary>
        ///     Adds a playlist paused tag.
        /// </summary>
        ///
        /// <param name="tags"> The tags. </param>
        public void AddPlaylistPausedTag(List<(string, TagType)> tags)
        {
            PlaylistPausedTags.AddTag(tags);
        }

        /// <summary>
        ///     Adds a playlist valid tag.
        /// </summary>
        ///
        /// <param name="tag">     The tag. </param>
        /// <param name="tagType"> Type of the tag. </param>
        public void AddPlaylistValidTag(string tag, TagType tagType)
        {
            PlaylistValidTags.AddTag(tag, tagType);
        }

        /// <summary>
        ///     Adds a playlist valid tag.
        /// </summary>
        ///
        /// <param name="tags">    The tags. </param>
        /// <param name="tagType"> Type of the tag. </param>
        public void AddPlaylistValidTag(List<string> tags, TagType tagType)
        {
            PlaylistValidTags.AddTag(tags, tagType);
        }

        /// <summary>
        ///     Adds a playlist valid tag.
        /// </summary>
        ///
        /// <param name="tags"> The tags. </param>
        public void AddPlaylistValidTag(List<(string, TagType)> tags)
        {
            PlaylistValidTags.AddTag(tags);
        }

        /// <summary>
        ///     Pauses this object.
        /// </summary>
        public void Pause()
        {
            var instances = Manager.GetAudioInstances(Manager.MusicCategory.GetPlaylistAudioInstances(Name), true);
            for (var i = 0; i < instances.Count; i++)
            {
                instances[i].Pause();
            }

            IsPaused = true;
        }

        /// <summary>
        ///     Removes the playlist music described by name.
        /// </summary>
        ///
        /// <param name="name"> The name. </param>
        ///
        /// <returns>
        ///     True if it succeeds, false if it fails.
        /// </returns>
        public bool RemovePlaylistMusic(string name)
        {
            return _music.Remove(name);
        }

        /// <summary>
        ///     Removes the playlist paused tag described by tags.
        /// </summary>
        ///
        /// <param name="tag">     The tag. </param>
        /// <param name="tagType"> Type of the tag. </param>
        ///
        /// <returns>
        ///     A List&lt;bool&gt;
        /// </returns>
        public bool RemovePlaylistPausedTag(string tag, TagType tagType)
        {
            return PlaylistPausedTags.RemoveTag(tag, tagType);
        }

        /// <summary>
        ///     Removes the playlist paused tag described by tags.
        /// </summary>
        ///
        /// <param name="tags">    The tags. </param>
        /// <param name="tagType"> Type of the tag. </param>
        ///
        /// <returns>
        ///     A List&lt;bool&gt;
        /// </returns>
        public List<bool> RemovePlaylistPausedTag(List<string> tags, TagType tagType)
        {
            return PlaylistPausedTags.RemoveTag(tags, tagType);
        }

        /// <summary>
        ///     Removes the playlist paused tag described by tags.
        /// </summary>
        ///
        /// <param name="tags"> The tags. </param>
        ///
        /// <returns>
        ///     A List&lt;bool&gt;
        /// </returns>
        public List<bool> RemovePlaylistPausedTag(List<(string, TagType)> tags)
        {
            return PlaylistPausedTags.RemoveTag(tags);
        }

        /// <summary>
        ///     Removes the playlist valid tag described by tags.
        /// </summary>
        ///
        /// <param name="tag">     The tag. </param>
        /// <param name="tagType"> Type of the tag. </param>
        ///
        /// <returns>
        ///     A List&lt;bool&gt;
        /// </returns>
        public bool RemovePlaylistValidTag(string tag, TagType tagType)
        {
            return PlaylistValidTags.RemoveTag(tag, tagType);
        }

        /// <summary>
        ///     Removes the playlist valid tag described by tags.
        /// </summary>
        ///
        /// <param name="tags">    The tags. </param>
        /// <param name="tagType"> Type of the tag. </param>
        ///
        /// <returns>
        ///     A List&lt;bool&gt;
        /// </returns>
        public List<bool> RemovePlaylistValidTag(List<string> tags, TagType tagType)
        {
            return PlaylistValidTags.RemoveTag(tags, tagType);
        }

        /// <summary>
        ///     Removes the playlist valid tag described by tags.
        /// </summary>
        ///
        /// <param name="tags"> The tags. </param>
        ///
        /// <returns>
        ///     A List&lt;bool&gt;
        /// </returns>
        public List<bool> RemovePlaylistValidTag(List<(string, TagType)> tags)
        {
            return PlaylistValidTags.RemoveTag(tags);
        }

        /// <summary>
        ///     Determine if we should playlist be paused.
        /// </summary>
        ///
        /// <param name="currentTags"> The current tags. </param>
        ///
        /// <returns>
        ///     True if it succeeds, false if it fails.
        /// </returns>
        public bool ShouldPlaylistBePaused(List<Tag> currentTags)
        {
            return PlaylistPausedTags.IsValid(currentTags);
        }

        /// <summary>
        ///     Determine if we should playlist be played.
        /// </summary>
        ///
        /// <param name="currentTags"> The current tags. </param>
        ///
        /// <returns>
        ///     True if it succeeds, false if it fails.
        /// </returns>
        public bool ShouldPlaylistBePlayed(List<Tag> currentTags)
        {
            return PlaylistValidTags.IsValid(currentTags);
        }

        /// <summary>
        ///     Determine if we should playlist be stopped.
        /// </summary>
        ///
        /// <param name="currentTags"> The current tags. </param>
        ///
        /// <returns>
        ///     True if it succeeds, false if it fails.
        /// </returns>
        public bool ShouldPlaylistBeStopped(List<Tag> currentTags)
        {
            return !(ShouldPlaylistBePlayed(currentTags) || ShouldPlaylistBePaused(currentTags));
        }

        /// <summary>
        ///     Stops this object.
        /// </summary>
        public void Stop()
        {
            var instances = Manager.GetAudioInstances(Manager.MusicCategory.GetPlaylistAudioInstances(Name), true);
            for (var i = 0; i < instances.Count; i++)
            {
                instances[i].Stop(true);
            }
            Manager.RemoveAudioInstances(instances.Select(x => x.ID).ToList(), AudioManager.MUSIC_CATEGORY);

            IsPaused = false;
        }

        /// <summary>
        ///     Updates the class.
        /// </summary>
        ///
        /// <param name="gameTime"> The game time. </param>
        ///
        /// <seealso cref="IUpdateable.Update(GameTime)"/>
        public virtual void Update(GameTime gameTime)
        {
            var instances = Manager.GetAudioInstancesForCategory(AudioManager.MUSIC_CATEGORY);
            var newMusic = instances.Count == 0;
            var instancestoKill = new List<ulong>();
            for (var i = 0; i < instances.Count; i++)
            {
                if (instances[i].State == SoundState.Stopped)
                {
                    instancestoKill.Add(instances[i].ID);
                    if (instances[i].Name.Equals(_currentMusicName))
                    {
                        _currentMusicName = string.Empty;
                        newMusic = true;
                    }
                }
                else if (instances[i].State == SoundState.Paused)
                {
                    instances[i].Resume();
                }
            }

            Manager.RemoveAudioInstances(instancestoKill, AudioManager.MUSIC_CATEGORY);
            Manager.MusicCategory.UnRegisterInstance(instancestoKill);

            if (newMusic)
            {
                var categoryTags = Manager.MusicCategory.GetCurrentTags();
                var associatedMusic = Manager.GetPlaylistMusic(_music.Keys.ToList());
                if (associatedMusic.Count == 0)
                {
                    _currentMusicName = string.Empty;
                }
                else
                {
                    var validMusic = new List<(Music, List<Tag>)>();
                    for (var i = 0; i < associatedMusic.Count; i++)
                    {
                        if (_music[associatedMusic[i].Name].Tags.IsValid(Manager.MusicCategory.GetCurrentTags(), out var validTags))
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
                        var id = Manager.GenerateNewAudioInstance(_currentMusicName, AudioManager.MUSIC_CATEGORY, true);
                        Manager.MusicCategory.RegisterInstance(id);
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
                        var id = Manager.GenerateNewAudioInstance(associatedMusic[itemId].Name, AudioManager.MUSIC_CATEGORY);
                        Manager.MusicCategory.RegisterInstance(id);
                    }
                }
            }
        }
    }
}