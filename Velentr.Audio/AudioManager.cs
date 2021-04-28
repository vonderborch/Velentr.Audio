using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Velentr.Audio.Categories;
using Velentr.Audio.Helpers;
using Velentr.Audio.Playlists;
using Velentr.Audio.Procedural;
using Velentr.Audio.Sounds;
using Velentr.Audio.Tagging;
using Velentr.Collections.CollectionActions;
using Velentr.Collections.Collections.Concurrent;
using IUpdateable = Velentr.Audio.Helpers.IUpdateable;

namespace Velentr.Audio
{
    /// <summary>
    ///     Manager for audioes.
    /// </summary>
    ///
    /// <seealso cref="IUpdateable"/>
    public class AudioManager : IUpdateable
    {
        /// <summary>
        ///     (Immutable) category the music belongs to.
        /// </summary>
        public const string MUSIC_CATEGORY = "MUSIC";

        /// <summary>
        ///     The randomizer.
        /// </summary>
        public Randomizer Randomizer;

        /// <summary>
        ///     The categories.
        /// </summary>
        private Dictionary<string, Category> _categories;

        /// <summary>
        ///     The current music instances.
        /// </summary>
        private Dictionary<ulong, AudioInstance> _currentMusicInstances;

        /// <summary>
        ///     The current sound instances.
        /// </summary>
        private Dictionary<ulong, AudioInstance> _currentSoundInstances;

        /// <summary>
        ///     The global volume.
        /// </summary>
        private float _globalVolume = 1f;

        /// <summary>
        ///     The music.
        /// </summary>
        private Dictionary<string, Music> _music;

        /// <summary>
        ///     Identifier for the music instance.
        /// </summary>
        private ulong _musicInstanceId;

        /// <summary>
        ///     The music pool.
        /// </summary>
        private ConcurrentPool<AudioInstance> _musicPool;

        /// <summary>
        ///     Identifier for the sound instance.
        /// </summary>
        private ulong _soundInstanceId;

        /// <summary>
        ///     The sound pool.
        /// </summary>
        private ConcurrentPool<AudioInstance> _soundPool;

        /// <summary>
        ///     The sounds.
        /// </summary>
        private Dictionary<string, Sound> _sounds;

        /// <summary>
        ///     Constructor.
        /// </summary>
        ///
        /// <param name="maxMusicInstances">
        ///     (Optional)
        ///     The maximum music instances.
        /// </param>
        /// <param name="randomizer">
        ///     (Optional)
        ///     The randomizer.
        /// </param>
        /// <param name="categories">        (Optional) The categories. </param>
        public AudioManager(int maxMusicInstances = 8, Randomizer randomizer = null, List<CategoryLoadInfo> categories = null)
        {
            TotalMaxAudioInstances = DetermineMaxSoundInstances();
            MaxMusicInstances = maxMusicInstances;
            MaxSoundInstances = TotalMaxAudioInstances - maxMusicInstances;
            CurrentMusicInstances = 0;
            CurrentSoundInstances = 0;
            _soundInstanceId = 0;
            _musicInstanceId = 0;

            Randomizer = randomizer ?? new SystemRandomizer();

            _music = new Dictionary<string, Music>();
            _sounds = new Dictionary<string, Sound>();
            _currentMusicInstances = new Dictionary<ulong, AudioInstance>();
            _currentSoundInstances = new Dictionary<ulong, AudioInstance>();

            _categories = new Dictionary<string, Category>();
            _categories.Add(MUSIC_CATEGORY, new MusicCategory(this, MUSIC_CATEGORY, true, 1f));
            if (categories != null)
            {
                for (var i = 0; i < categories.Count; i++)
                {
                    AddCategory(categories[i]);
                }
            }

            _musicPool = new ConcurrentPool<AudioInstance>(actionWhenPoolFull: PoolFullAction.IncreaseSize);
            _soundPool = new ConcurrentPool<AudioInstance>(actionWhenPoolFull: PoolFullAction.IncreaseSize);
        }

        /// <summary>
        ///     Gets or sets the last update time.
        /// </summary>
        ///
        /// <value>
        ///     The last update time.
        /// </value>
        public static TimeSpan LastUpdateTime { get; private set; }

        /// <summary>
        ///     Gets or sets the current music instances.
        /// </summary>
        ///
        /// <value>
        ///     The current music instances.
        /// </value>
        public int CurrentMusicInstances { get; internal set; }

        /// <summary>
        ///     Gets or sets the current sound instances.
        /// </summary>
        ///
        /// <value>
        ///     The current sound instances.
        /// </value>
        public int CurrentSoundInstances { get; internal set; }

        /// <summary>
        ///     Gets or sets the global volume.
        /// </summary>
        ///
        /// <value>
        ///     The global volume.
        /// </value>
        public float GlobalVolume
        {
            get => _globalVolume;
            set
            {
                _globalVolume = value;
                foreach (var category in _categories)
                {
                    category.Value.UpdateVolume(category.Value.CurrentVolume);
                }
            }
        }

        /// <summary>
        ///     Gets a value indicating whether this object has free music instances.
        /// </summary>
        ///
        /// <value>
        ///     True if this object has free music instances, false if not.
        /// </value>
        public bool HasFreeMusicInstances => MaxMusicInstances > CurrentMusicInstances;

        /// <summary>
        ///     Gets a value indicating whether this object has free sound instances.
        /// </summary>
        ///
        /// <value>
        ///     True if this object has free sound instances, false if not.
        /// </value>
        public bool HasFreeSoundInstances => MaxSoundInstances > CurrentSoundInstances;

        /// <summary>
        ///     Gets the maximum music instances.
        /// </summary>
        ///
        /// <value>
        ///     The maximum music instances.
        /// </value>
        public int MaxMusicInstances { get; }

        /// <summary>
        ///     Gets the maximum sound instances.
        /// </summary>
        ///
        /// <value>
        ///     The maximum sound instances.
        /// </value>
        public int MaxSoundInstances { get; }

        /// <summary>
        ///     Gets the total number of maximum audio instances.
        /// </summary>
        ///
        /// <value>
        ///     The total number of maximum audio instances.
        /// </value>
        public int TotalMaxAudioInstances { get; }

        /// <summary>
        ///     Gets the category the music belongs to.
        /// </summary>
        ///
        /// <value>
        ///     The music category.
        /// </value>
        public MusicCategory MusicCategory => (MusicCategory)_categories[MUSIC_CATEGORY];

        /// <summary>
        ///     Adds a category.
        /// </summary>
        ///
        /// <param name="name">   The name. </param>
        /// <param name="volume"> The volume. </param>
        public void AddCategory(string name, float volume)
        {
            name = name.ToUpperInvariant();
            _categories.Add(name, new Category(this, name, name == MUSIC_CATEGORY, volume));
        }

        /// <summary>
        ///     Adds a category.
        /// </summary>
        ///
        /// <param name="category"> The category. </param>
        public void AddCategory(CategoryLoadInfo category)
        {
            _categories.Add(category.Name, new Category(this, category.Name, category.Name == MUSIC_CATEGORY, category.Volume));
        }

        /// <summary>
        ///     Adds a category.
        /// </summary>
        ///
        /// <param name="categories"> The categories. </param>
        public void AddCategory(List<CategoryLoadInfo> categories)
        {
            for (var i = 0; i < categories.Count; i++)
            {
                _categories.Add(categories[i].Name, new Category(this, categories[i].Name, categories[i].Name == MUSIC_CATEGORY, categories[i].Volume));
            }
        }

        /// <summary>
        ///     Adds a music.
        /// </summary>
        ///
        /// <param name="name">         The name. </param>
        /// <param name="path">         Full pathname of the file. </param>
        /// <param name="defaultPitch"> (Optional) The default pitch. </param>
        /// <param name="autoLoad">     (Optional) True to automatically load. </param>
        public void AddMusic(string name, string path, float defaultPitch = 0, bool autoLoad = false)
        {
            name = name.ToUpperInvariant();
            _music.Add(name, new Music(this, name, path, defaultPitch, autoLoad));
        }

        /// <summary>
        ///     Adds a music.
        /// </summary>
        ///
        /// <param name="loadInfo"> Information describing the load. </param>
        public void AddMusic(MusicLoadInfo loadInfo)
        {
            _music.Add(loadInfo.Name, new Music(this, loadInfo.Name, loadInfo.Path, loadInfo.DefaultPitch, loadInfo.AutoLoad));
        }

        /// <summary>
        ///     Adds a music.
        /// </summary>
        ///
        /// <param name="musicTracks"> The music tracks. </param>
        public void AddMusic(List<MusicLoadInfo> musicTracks)
        {
            for (var i = 0; i < musicTracks.Count; i++)
            {
                AddMusic(musicTracks[i]);
            }
        }

        /// <summary>
        ///     Adds a procedural music sample.
        /// </summary>
        ///
        /// <param name="name">         The name. </param>
        /// <param name="path">         Full pathname of the file. </param>
        /// <param name="defaultPitch"> The default pitch. </param>
        /// <param name="autoLoad">     (Optional) True to automatically load. </param>
        public void AddProceduralMusicSample(string name, string path, float defaultPitch, bool autoLoad = false)
        {
            name = name.ToUpperInvariant();
            _music.Add(name, new Music(this, name, path, defaultPitch, autoLoad));
        }

        /// <summary>
        ///     Adds a procedural music sample.
        /// </summary>
        ///
        /// <param name="loadInfo"> Information describing the load. </param>
        public void AddProceduralMusicSample(MusicLoadInfo loadInfo)
        {
            _music.Add(loadInfo.Name, new Music(this, loadInfo.Name, loadInfo.Path, loadInfo.DefaultPitch, loadInfo.AutoLoad));
        }

        /// <summary>
        ///     Adds a procedural music sample.
        /// </summary>
        ///
        /// <param name="proceduralMusicSamples"> The procedural music samples. </param>
        public void AddProceduralMusicSample(List<MusicLoadInfo> proceduralMusicSamples)
        {
            for (var i = 0; i < proceduralMusicSamples.Count; i++)
            {
                AddProceduralMusicSample(proceduralMusicSamples[i]);
            }
        }

        /// <summary>
        ///     Adds a procedural song.
        /// </summary>
        ///
        /// <exception cref="Exception"> Thrown when an exception error condition occurs. </exception>
        ///
        /// <param name="playlist"> The playlist. </param>
        /// <param name="name">     The name. </param>
        /// <param name="song">     The song. </param>
        public void AddProceduralSong(string playlist, string name, ProceduralSong song)
        {
            playlist = playlist.ToUpperInvariant();
            name = name.ToUpperInvariant();

            if (MusicCategory._playlists[playlist].PlaylistType == PlaylistType.Normal)
            {
                throw new Exception("Invalid playlist type to add a procedural song to!");
            }

            ((ProceduralPlaylist) MusicCategory._playlists[playlist]).AddProceduralSong(name, song);
        }

        /// <summary>
        ///     Adds a procedural song.
        /// </summary>
        ///
        /// <param name="playlist"> The playlist. </param>
        /// <param name="songs">    The songs. </param>
        public void AddProceduralSong(string playlist, List<(string, ProceduralSong)> songs)
        {
            for (var i = 0; i < songs.Count; i++)
            {
                AddProceduralSong(playlist, songs[i].Item1, songs[i].Item2);
            }
        }

        /// <summary>
        ///     Adds a procedural song.
        /// </summary>
        ///
        /// <param name="playlistsAndSongs"> The playlists and songs. </param>
        public void AddProceduralSong(Dictionary<string, List<(string, ProceduralSong)>> playlistsAndSongs)
        {
            foreach (var playlist in playlistsAndSongs)
            {
                AddProceduralSong(playlist.Key, playlist.Value);
            }
        }

        /// <summary>
        ///     Adds a music tag.
        /// </summary>
        ///
        /// <param name="tag"> The tag. </param>
        public void AddMusicTag(Tag tag)
        {
            _categories[MUSIC_CATEGORY].AddTag(tag);
        }

        /// <summary>
        ///     Adds a music tag.
        /// </summary>
        ///
        /// <param name="tags"> The tags. </param>
        public void AddMusicTag(List<Tag> tags)
        {
            _categories[MUSIC_CATEGORY].AddTag(tags);
        }

        /// <summary>
        ///     Adds a music tag.
        /// </summary>
        ///
        /// <param name="name">                 The name. </param>
        /// <param name="consumable">           (Optional) True if consumable. </param>
        /// <param name="lifespanMilliseconds"> (Optional) The lifespan in milliseconds. </param>
        public void AddMusicTag(string name, bool consumable = false, uint lifespanMilliseconds = UInt32.MaxValue)
        {
            _categories[MUSIC_CATEGORY].AddTag(new Tag(name, consumable, lifespanMilliseconds));
        }

        /// <summary>
        ///     Adds a music to playlist.
        /// </summary>
        ///
        /// <param name="playlist"> The playlist. </param>
        /// <param name="name">     The name. </param>
        /// <param name="tags">     The tags. </param>
        public void AddMusicToPlaylist(string playlist, string name, TagSet tags)
        {
            AddMusicToPlaylist(playlist, new PlaylistMusicInfo(name, tags));
        }

        /// <summary>
        ///     Adds a music to playlist.
        /// </summary>
        ///
        /// <param name="playlist">      The playlist. </param>
        /// <param name="name">          The name. </param>
        /// <param name="tags">          (Optional) The tags. </param>
        /// <param name="exclusionTags"> (Optional) The exclusion tags. </param>
        /// <param name="requiredTags">  (Optional) The required tags. </param>
        public void AddMusicToPlaylist(string playlist, string name, List<string> tags = null, List<string> exclusionTags = null, List<string> requiredTags = null)
        {
            AddMusicToPlaylist(playlist, new PlaylistMusicInfo(name, tags, exclusionTags, requiredTags));
        }

        /// <summary>
        ///     Adds a music to playlist.
        /// </summary>
        ///
        /// <param name="playlist">  The playlist. </param>
        /// <param name="trackInfo"> Information describing the track. </param>
        public void AddMusicToPlaylist(string playlist, PlaylistMusicInfo trackInfo)
        {
            MusicCategory._playlists[playlist].AddPlaylistMusic(trackInfo);
        }

        /// <summary>
        ///     Adds a music to playlist.
        /// </summary>
        ///
        /// <param name="playlist"> The playlist. </param>
        /// <param name="tracks">   The tracks. </param>
        public void AddMusicToPlaylist(string playlist, List<PlaylistMusicInfo> tracks)
        {
            for (var i = 0; i < tracks.Count; i++)
            {
                AddMusicToPlaylist(playlist, tracks[i]);
            }
        }

        /// <summary>
        ///     Adds a music to playlist.
        /// </summary>
        ///
        /// <param name="playlistsAndTracks"> The playlists and tracks. </param>
        public void AddMusicToPlaylist(Dictionary<string, List<PlaylistMusicInfo>> playlistsAndTracks)
        {
            foreach (var playlist in playlistsAndTracks)
            {
                AddMusicToPlaylist(playlist.Key, playlist.Value);
            }
        }

        /// <summary>
        ///     Adds a playlist.
        /// </summary>
        ///
        /// <param name="playlist"> The playlist. </param>
        public void AddPlaylist(Playlist playlist)
        {
            MusicCategory.AddPlaylist(playlist);
        }

        /// <summary>
        ///     Adds a playlist.
        /// </summary>
        ///
        /// <param name="playlist"> The playlist. </param>
        public void AddPlaylist(PlaylistLoadInfo playlist)
        {
            Playlist playlistInstance;
            switch (playlist.PlaylistType)
            {
                case PlaylistType.Normal:
                    playlistInstance = new Playlist(this, playlist.Name, playlist.Randomize, playlist.MaxTriesToAvoidRepeats, playlist.PlaylistValidTags, playlist.PlaylistPauasedTags);
                    break;
                case PlaylistType.Procedural:
                    playlistInstance = new ProceduralPlaylist(this, playlist.Name, playlist.Randomize, playlist.MaxTriesToAvoidRepeats, playlist.PlaylistValidTags, playlist.PlaylistPauasedTags);
                    break;
                default:
                    throw new Exception("Unsupported playlist type!");
            }

            for (var i = 0; i < playlist.MusicTracks.Count; i++)
            {
                playlistInstance.AddPlaylistMusic(playlist.MusicTracks[i]);
            }

            MusicCategory.AddPlaylist(playlistInstance);
        }

        /// <summary>
        ///     Adds a playlist.
        /// </summary>
        ///
        /// <param name="playlists"> The playlists. </param>
        public void AddPlaylist(List<PlaylistLoadInfo> playlists)
        {
            for (var i = 0; i < playlists.Count; i++)
            {
                AddPlaylist(playlists[i]);
            }
        }

        /// <summary>
        ///     Adds a playlist.
        /// </summary>
        ///
        /// <param name="name">                   The name. </param>
        /// <param name="playlistType">           Type of the playlist. </param>
        /// <param name="randomize">              (Optional) True to randomize. </param>
        /// <param name="maxTriesToAvoidRepeats"> (Optional) The maximum tries to avoid repeats. </param>
        public void AddPlaylist(string name, PlaylistType playlistType, bool randomize = true, int maxTriesToAvoidRepeats = 3)
        {
            AddPlaylist(new PlaylistLoadInfo(name, playlistType, randomize, maxTriesToAvoidRepeats));
        }

        /// <summary>
        ///     Adds a playlist.
        /// </summary>
        ///
        /// <param name="name">                   The name. </param>
        /// <param name="playlistType">           Type of the playlist. </param>
        /// <param name="playlistValidTags">      The playlist valid tags. </param>
        /// <param name="playlistPausedTags">     The playlist paused tags. </param>
        /// <param name="randomize">              (Optional) True to randomize. </param>
        /// <param name="maxTriesToAvoidRepeats"> (Optional) The maximum tries to avoid repeats. </param>
        public void AddPlaylist(string name, PlaylistType playlistType, TagSet playlistValidTags, TagSet playlistPausedTags, bool randomize = true, int maxTriesToAvoidRepeats = 3)
        {
            AddPlaylist(new PlaylistLoadInfo(name, playlistType, playlistValidTags, playlistPausedTags, randomize, maxTriesToAvoidRepeats));
        }

        /// <summary>
        ///     Adds a playlist.
        /// </summary>
        ///
        /// <param name="name">                       The name. </param>
        /// <param name="playlistType">           Type of the playlist. </param>
        /// <param name="validPlaylistTags">          The valid playlist tags. </param>
        /// <param name="validPlaylistExclusionTags"> The valid playlist exclusion tags. </param>
        /// <param name="validPlaylistRequiredTags">  The valid playlist required tags. </param>
        /// <param name="pausePlaylistTags">          The pause playlist tags. </param>
        /// <param name="pausePlaylistExclusionTags"> The pause playlist exclusion tags. </param>
        /// <param name="pausePlaylistRequiredTags">  The pause playlist required tags. </param>
        /// <param name="randomize">                  (Optional) True to randomize. </param>
        /// <param name="maxTriesToAvoidRepeats">
        ///     (Optional) The maximum tries to avoid repeats.
        /// </param>
        public void AddPlaylist(string name, PlaylistType playlistType, List<string> validPlaylistTags, List<string> validPlaylistExclusionTags, List<string> validPlaylistRequiredTags, List<string> pausePlaylistTags, List<string> pausePlaylistExclusionTags, List<string> pausePlaylistRequiredTags, bool randomize = true, int maxTriesToAvoidRepeats = 3)
        {
            AddPlaylist(new PlaylistLoadInfo(name, playlistType, validPlaylistTags, validPlaylistExclusionTags, validPlaylistRequiredTags, pausePlaylistTags, pausePlaylistExclusionTags, pausePlaylistRequiredTags, randomize, maxTriesToAvoidRepeats));
        }

        /// <summary>
        ///     Adds a playlist tag.
        /// </summary>
        ///
        /// <param name="tag"> The tag. </param>
        public void AddPlaylistChoiceTag(Tag tag)
        {
            MusicCategory.AddPlaylistChoiceTag(tag);
        }

        /// <summary>
        ///     Adds a playlist tag.
        /// </summary>
        ///
        /// <param name="tags"> The tags. </param>
        public void AddPlaylistChoiceTag(List<Tag> tags)
        {
            MusicCategory.AddPlaylistChoiceTag(tags);
        }

        /// <summary>
        ///     Adds a playlist tag.
        /// </summary>
        ///
        /// <param name="name">                 The name. </param>
        /// <param name="consumable">           (Optional) True if consumable. </param>
        /// <param name="lifespanMilliseconds"> (Optional) The lifespan in milliseconds. </param>
        public void AddPlaylistChoiceTag(string name, bool consumable = false, uint lifespanMilliseconds = UInt32.MaxValue)
        {
            MusicCategory.AddPlaylistChoiceTag(new Tag(name, consumable, lifespanMilliseconds));
        }

        /// <summary>
        ///     Adds a sound.
        /// </summary>
        ///
        /// <param name="name">          The name. </param>
        /// <param name="path">          Full pathname of the file. </param>
        /// <param name="categories">    The categories. </param>
        /// <param name="defaultPitch">  (Optional) The default pitch. </param>
        /// <param name="autoLoad">      (Optional) True to automatically load. </param>
        /// <param name="tags">          (Optional) The tags. </param>
        /// <param name="exclusionTags"> (Optional) The exclusion tags. </param>
        /// <param name="requiredTags">  (Optional) The required tags. </param>
        public void AddSound(string name, string path, List<string> categories, float defaultPitch = 0, bool autoLoad = false, List<string> tags = null, List<string> exclusionTags = null, List<string> requiredTags = null)
        {
            name = name.ToUpperInvariant();
            _sounds.Add(name, new Sound(this, name, path, categories, defaultPitch, autoLoad, tags, exclusionTags, requiredTags));
        }

        /// <summary>
        ///     Adds a sound.
        /// </summary>
        ///
        /// <param name="loadInfo"> Information describing the load. </param>
        public void AddSound(SoundLoadInfo loadInfo)
        {
            _sounds.Add(loadInfo.Name, new Sound(this, loadInfo.Name, loadInfo.Path, loadInfo.Categories, loadInfo.DefaultPitch, loadInfo.AutoLoad, loadInfo.Tags.Tags, loadInfo.Tags.ExclusionTags, loadInfo.Tags.RequiredTags));
        }

        /// <summary>
        ///     Adds a sound.
        /// </summary>
        ///
        /// <param name="sounds"> The sounds. </param>
        public void AddSound(List<SoundLoadInfo> sounds)
        {
            for (var i = 0; i < sounds.Count; i++)
            {
                AddSound(sounds[i]);
            }
        }

        /// <summary>
        ///     Adds a sound tag.
        /// </summary>
        ///
        /// <exception cref="Exception"> Thrown when an exception error condition occurs. </exception>
        ///
        /// <param name="category"> The category. </param>
        /// <param name="tag">      The tag. </param>
        public void AddSoundTag(string category, Tag tag)
        {
            category = category.ToUpperInvariant();
            if (category == MUSIC_CATEGORY)
            {
                throw new Exception("Can't add a sound tag to the Music Category!");
            }
            _categories[category].AddTag(tag);
        }

        /// <summary>
        ///     Adds a sound tag.
        /// </summary>
        ///
        /// <exception cref="Exception"> Thrown when an exception error condition occurs. </exception>
        ///
        /// <param name="category"> The category. </param>
        /// <param name="tags">     The tags. </param>
        public void AddSoundTag(string category, List<Tag> tags)
        {
            category = category.ToUpperInvariant();
            if (category == MUSIC_CATEGORY)
            {
                throw new Exception("Can't add a sound tag to the Music Category!");
            }
            _categories[category].AddTag(tags);
        }

        /// <summary>
        ///     Adds a sound tag.
        /// </summary>
        ///
        /// <exception cref="Exception"> Thrown when an exception error condition occurs. </exception>
        ///
        /// <param name="category">             The category. </param>
        /// <param name="name">                 The name. </param>
        /// <param name="consumable">           True if consumable. </param>
        /// <param name="lifespanMilliseconds"> The lifespan in milliseconds. </param>
        public void AddSoundTag(string category, string name, bool consumable, uint lifespanMilliseconds)
        {
            category = category.ToUpperInvariant();
            if (category == MUSIC_CATEGORY)
            {
                throw new Exception("Can't add a sound tag to the Music Category!");
            }
            _categories[category].AddTag(new Tag(name, consumable, lifespanMilliseconds));
        }

        /// <summary>
        ///     Adds a tag.
        /// </summary>
        ///
        /// <param name="category"> The category. </param>
        /// <param name="tag">      The tag. </param>
        public void AddTag(string category, Tag tag)
        {
            _categories[category.ToUpperInvariant()].AddTag(tag);
        }

        /// <summary>
        ///     Adds a tag.
        /// </summary>
        ///
        /// <param name="category"> The category. </param>
        /// <param name="tags">     The tags. </param>
        public void AddTag(string category, List<Tag> tags)
        {
            _categories[category.ToUpperInvariant()].AddTag(tags);
        }

        /// <summary>
        ///     Adds a tag.
        /// </summary>
        ///
        /// <param name="category">             The category. </param>
        /// <param name="name">                 The name. </param>
        /// <param name="consumable">           True if consumable. </param>
        /// <param name="lifespanMilliseconds"> The lifespan in milliseconds. </param>
        public void AddTag(string category, string name, bool consumable, uint lifespanMilliseconds)
        {
            _categories[category.ToUpperInvariant()].AddTag(new Tag(name, consumable, lifespanMilliseconds));
        }

        /// <summary>
        ///     Change playlist.
        /// </summary>
        ///
        /// <param name="newPlaylist"> The new playlist. </param>
        public void ChangePlaylist(string newPlaylist)
        {
            MusicCategory.ChangePlaylist(newPlaylist);
        }

        /// <summary>
        ///     Generates a new audio instance.
        /// </summary>
        ///
        /// <exception cref="Exception"> Thrown when an exception error condition occurs. </exception>
        ///
        /// <param name="name">     The name. </param>
        /// <param name="category"> The category. </param>
        /// <param name="autoPlay"> (Optional) True to automatically play. </param>
        ///
        /// <returns>
        ///     The new audio instance.
        /// </returns>
        public ulong GenerateNewAudioInstance(string name, string category, bool autoPlay = true, bool throwOnError = true)
        {
            name = name.ToUpperInvariant();
            category = category.ToUpperInvariant();

            if (category == MUSIC_CATEGORY && !HasFreeMusicInstances && throwOnError)
            {
                throw new Exception("No free music instances available!");
            }
            if (category != MUSIC_CATEGORY && !HasFreeSoundInstances && throwOnError)
            {
                throw new Exception("No free sound instances available!");
            }

            AudioInstance newAudioInstance = null;
            if (category == MUSIC_CATEGORY)
            {
                var instance = _music[name].GetInstance();
                if (instance == null && throwOnError)
                {
                    throw new Exception("Failed to create a new music instance!");
                }

                newAudioInstance = _musicPool.Get();
                newAudioInstance.UpdateInstance(this, _musicInstanceId++, name, MUSIC_CATEGORY, instance);
                _currentMusicInstances.Add(newAudioInstance.ID, newAudioInstance);
                CurrentMusicInstances++;
                MusicCategory.RegisterInstance(newAudioInstance.ID);
            }
            else
            {
                var instance = _sounds[name].GetInstance();
                if (instance == null && throwOnError)
                {
                    throw new Exception("Failed to create a new music instance!");
                }

                newAudioInstance = _soundPool.Get();
                newAudioInstance.UpdateInstance(this, _soundInstanceId++, name, category, instance);
                _currentSoundInstances.Add(newAudioInstance.ID, newAudioInstance);
                CurrentSoundInstances++;
                _categories[category].RegisterInstance(newAudioInstance.ID);
            }

            if (autoPlay)
            {
                newAudioInstance?.Play(true);
            }
            return newAudioInstance?.ID ?? (ulong)0;
        }

        /// <summary>
        ///     Gets associated audio.
        /// </summary>
        ///
        /// <param name="category"> The category. </param>
        ///
        /// <returns>
        ///     The associated audio.
        /// </returns>
        public List<Sounds.Audio> GetAssociatedAudio(string category)
        {
            category = category.ToUpperInvariant();
            var output = new List<Sounds.Audio>();
            if (category == MUSIC_CATEGORY)
            {
                foreach (var music in _music)
                {
                    output.Add(music.Value);
                }
            }
            else
            {
                foreach (var sound in _sounds)
                {
                    if (sound.Value.Categories.Contains(category))
                    {
                        output.Add(sound.Value);
                    }
                }
            }

            return output;
        }

        /// <summary>
        ///     Gets audio instances.
        /// </summary>
        ///
        /// <param name="ids">               The identifiers. </param>
        /// <param name="getMusicInstances"> True to get music instances. </param>
        ///
        /// <returns>
        ///     The audio instances.
        /// </returns>
        public List<AudioInstance> GetAudioInstances(List<ulong> ids, bool getMusicInstances)
        {
            var output = new List<AudioInstance>();

            if (getMusicInstances)
            {
                foreach (var instance in _currentMusicInstances)
                {
                    output.Add(instance.Value);
                }
            }
            else
            {
                for (var i = 0; i < ids.Count; i++)
                {
                    output.Add(_currentSoundInstances[ids[i]]);
                }
            }

            return output;
        }

        /// <summary>
        ///     Gets a category.
        /// </summary>
        ///
        /// <param name="category"> The category. </param>
        ///
        /// <returns>
        ///     The category.
        /// </returns>
        public Category GetCategory(string category)
        {
            category = category.ToUpperInvariant();
            if (_categories.TryGetValue(category, out var output))
            {
                return output;
            }

            return null;
        }

        /// <summary>
        ///     Gets category names.
        /// </summary>
        ///
        /// <returns>
        ///     The category names.
        /// </returns>
        public List<string> GetCategoryNames()
        {
            return _categories.Keys.ToList();
        }

        /// <summary>
        ///     Gets current playlist.
        /// </summary>
        ///
        /// <returns>
        ///     The current playlist.
        /// </returns>
        public string GetCurrentPlaylist()
        {
            return MusicCategory.CurrentPlaylist;
        }

        /// <summary>
        ///     Gets music tags.
        /// </summary>
        ///
        /// <returns>
        ///     The music tags.
        /// </returns>
        public List<Tag> GetMusicTags()
        {
            return _categories[MUSIC_CATEGORY].GetCurrentTags();
        }

        /// <summary>
        ///     Gets playlist music.
        /// </summary>
        ///
        /// <param name="trackNames"> List of names of the tracks. </param>
        ///
        /// <returns>
        ///     The playlist music.
        /// </returns>
        public List<Music> GetPlaylistMusic(List<string> trackNames)
        {
            return _music.Where(x => trackNames.Contains(x.Key)).Select(x => (Music)x.Value).ToList();
        }

        /// <summary>
        ///     Gets playlist tags.
        /// </summary>
        ///
        /// <returns>
        ///     The playlist tags.
        /// </returns>
        public List<Tag> GetPlaylistTags()
        {
            return MusicCategory.GetCurrentPlaylistTags();
        }

        /// <summary>
        ///     Gets the tags.
        /// </summary>
        ///
        /// <param name="category"> The category. </param>
        ///
        /// <returns>
        ///     The tags.
        /// </returns>
        public List<Tag> GetTags(string category)
        {
            return _categories[category.ToUpperInvariant()].GetCurrentTags();
        }

        /// <summary>
        ///     Removes the audio instances.
        /// </summary>
        ///
        /// <param name="ids">      The identifiers. </param>
        /// <param name="category"> The category. </param>
        public void RemoveAudioInstances(List<ulong> ids, string category)
        {
            category = category.ToUpperInvariant();
            if (category == MUSIC_CATEGORY)
            {
                for (var i = 0; i < ids.Count; i++)
                {
                    var instance = _currentMusicInstances[ids[i]];
                    _currentMusicInstances.Remove(ids[i]);
                    instance.Stop(true);
                    _musicPool.Return(instance);
                    CurrentMusicInstances--;
                }
            }
            else
            {
                for (var i = 0; i < ids.Count; i++)
                {
                    var instance = _currentSoundInstances[ids[i]];
                    _currentSoundInstances.Remove(ids[i]);
                    instance.Stop(true);
                    _soundPool.Return(instance);
                    CurrentSoundInstances--;
                }
            }
        }

        /// <summary>
        ///     Removes the music described by names.
        /// </summary>
        ///
        /// <param name="name"> The name. </param>
        public void RemoveMusic(string name)
        {
            _music.Remove(name.ToUpperInvariant());

            // stop the music from playing (if it is) and remove it from everywhere it is associated with 1
            var instances = _currentMusicInstances.Where(x => x.Value.Name == name);
            var instancesToKill = instances.Select(x => x.Key).ToList();
            RemoveAudioInstances(instancesToKill, MUSIC_CATEGORY);
        }

        /// <summary>
        ///     Removes the music described by names.
        /// </summary>
        ///
        /// <param name="names"> The names. </param>
        public void RemoveMusic(List<string> names)
        {
            for (var i = 0; i < names.Count; i++)
            {
                RemoveMusic(names[i].ToUpperInvariant());
            }
        }

        /// <summary>
        ///     Removes the procedural music sample described by name.
        /// </summary>
        ///
        /// <param name="name"> The name. </param>
        public void RemoveProceduralMusicSample(string name)
        {
            _music.Remove(name.ToUpperInvariant());

            // stop the music from playing (if it is) and remove it from everywhere it is associated with 1
            var instances = _currentMusicInstances.Where(x => x.Value.Name == name);
            var instancesToKill = instances.Select(x => x.Key).ToList();
            RemoveAudioInstances(instancesToKill, MUSIC_CATEGORY);
        }

        /// <summary>
        ///     Removes the procedural music sample described by name.
        /// </summary>
        ///
        /// <param name="names"> The names. </param>
        public void RemoveProceduralMusicSample(List<string> names)
        {
            for (var i = 0; i < names.Count; i++)
            {
                RemoveMusic(names[i].ToUpperInvariant());
            }
        }

        /// <summary>
        ///     Removes the music from playlist described by playlistsAndTracks.
        /// </summary>
        ///
        /// <param name="playlist"> The playlist. </param>
        /// <param name="name">     The name. </param>
        public void RemoveMusicFromPlaylist(string playlist, string name)
        {
            MusicCategory._playlists[playlist.ToUpperInvariant()].RemovePlaylistMusic(name.ToUpperInvariant());
        }

        /// <summary>
        ///     Removes the music from playlist described by playlistsAndTracks.
        /// </summary>
        ///
        /// <param name="playlist"> The playlist. </param>
        /// <param name="names">    The names. </param>
        public void RemoveMusicFromPlaylist(string playlist, List<string> names)
        {
            for (var i = 0; i < names.Count; i++)
            {
                RemoveMusicFromPlaylist(playlist, names[i]);
            }
        }

        /// <summary>
        ///     Removes the music from playlist described by playlistsAndTracks.
        /// </summary>
        ///
        /// <param name="playlistsAndTracks"> The playlists and tracks. </param>
        public void RemoveMusicFromPlaylist(Dictionary<string, List<string>> playlistsAndTracks)
        {
            foreach (var playlist in playlistsAndTracks)
            {
                RemoveMusicFromPlaylist(playlist.Key, playlist.Value);
            }
        }

        /// <summary>
        ///     Removes the music tag described by tags.
        /// </summary>
        ///
        /// <param name="name">                    The name. </param>
        /// <param name="removeOnlyFirstinstance"> (Optional) True to remove only firstinstance. </param>
        ///
        /// <returns>
        ///     A List&lt;bool&gt;
        /// </returns>
        public bool RemoveMusicTag(string name, bool removeOnlyFirstinstance = true)
        {
            return _categories[MUSIC_CATEGORY].RemoveTag(name, removeOnlyFirstinstance);
        }

        /// <summary>
        ///     Removes the music tag described by tags.
        /// </summary>
        ///
        /// <param name="names">                   The names. </param>
        /// <param name="removeOnlyFirstinstance"> (Optional) True to remove only firstinstance. </param>
        ///
        /// <returns>
        ///     A List&lt;bool&gt;
        /// </returns>
        public List<bool> RemoveMusicTag(List<string> names, bool removeOnlyFirstinstance = true)
        {
            return _categories[MUSIC_CATEGORY].RemoveTag(names, removeOnlyFirstinstance);
        }

        /// <summary>
        ///     Removes the music tag described by tags.
        /// </summary>
        ///
        /// <param name="tags"> The tags. </param>
        ///
        /// <returns>
        ///     A List&lt;bool&gt;
        /// </returns>
        public List<bool> RemoveMusicTag(List<(string, bool)> tags)
        {
            return _categories[MUSIC_CATEGORY].RemoveTag(tags);
        }

        /// <summary>
        ///     Removes the playlist described by names.
        /// </summary>
        ///
        /// <param name="name"> The name. </param>
        ///
        /// <returns>
        ///     A List&lt;bool&gt;
        /// </returns>
        public bool RemovePlaylist(string name)
        {
            return MusicCategory.RemovePlaylist(name);
        }

        /// <summary>
        ///     Removes the playlist described by names.
        /// </summary>
        ///
        /// <param name="names"> The names. </param>
        ///
        /// <returns>
        ///     A List&lt;bool&gt;
        /// </returns>
        public List<bool> RemovePlaylist(List<string> names)
        {
            var output = new List<bool>();
            for (var i = 0; i < names.Count; i++)
            {
                output.Add(MusicCategory.RemovePlaylist(names[i]));
            }

            return output;
        }

        /// <summary>
        ///     Removes the playlist tag described by tags.
        /// </summary>
        ///
        /// <param name="name">                    The name. </param>
        /// <param name="removeOnlyFirstinstance"> (Optional) True to remove only firstinstance. </param>
        ///
        /// <returns>
        ///     A List&lt;bool&gt;
        /// </returns>
        public bool RemovePlaylistChoiceTag(string name, bool removeOnlyFirstinstance = true)
        {
            return MusicCategory.RemovePlaylistChoiceTag(name, removeOnlyFirstinstance);
        }

        /// <summary>
        ///     Removes the playlist tag described by tags.
        /// </summary>
        ///
        /// <param name="names">                   The names. </param>
        /// <param name="removeOnlyFirstinstance"> (Optional) True to remove only firstinstance. </param>
        ///
        /// <returns>
        ///     A List&lt;bool&gt;
        /// </returns>
        public List<bool> RemovePlaylistChoiceTag(List<string> names, bool removeOnlyFirstinstance = true)
        {
            return MusicCategory.RemovePlaylistChoiceTag(names, removeOnlyFirstinstance);
        }

        /// <summary>
        ///     Removes the playlist tag described by tags.
        /// </summary>
        ///
        /// <param name="tags"> The tags. </param>
        ///
        /// <returns>
        ///     A List&lt;bool&gt;
        /// </returns>
        public List<bool> RemovePlaylistChoiceTag(List<(string, bool)> tags)
        {
            return MusicCategory.RemovePlaylistChoiceTag(tags);
        }

        /// <summary>
        ///     Removes the procedural song.
        /// </summary>
        ///
        /// <exception cref="Exception"> Thrown when an exception error condition occurs. </exception>
        ///
        /// <param name="playlist"> The playlist. </param>
        /// <param name="name">     The name. </param>
        ///
        /// <returns>
        ///     True if it succeeds, false if it fails.
        /// </returns>
        public bool RemoveProceduralSong(string playlist, string name)
        {
            playlist = playlist.ToUpperInvariant();
            name = name.ToUpperInvariant();

            if (MusicCategory._playlists[playlist].PlaylistType == PlaylistType.Normal)
            {
                throw new Exception("Invalid playlist type to add a procedural song to!");
            }

            return ((ProceduralPlaylist)MusicCategory._playlists[playlist]).RemoveProceduralSong(name);
        }

        /// <summary>
        ///     Removes the procedural song.
        /// </summary>
        ///
        /// <param name="playlist"> The playlist. </param>
        /// <param name="names">    The names. </param>
        ///
        /// <returns>
        ///     True if it succeeds, false if it fails.
        /// </returns>
        public List<bool> RemoveProceduralSong(string playlist, List<string> names)
        {
            var output = new List<bool>();
            for (var i = 0; i < names.Count; i++)
            {
                output.Add(RemoveProceduralSong(playlist, names[i]));
            }

            return output;
        }

        /// <summary>
        ///     Removes the sound described by names.
        /// </summary>
        ///
        /// <param name="name"> The name. </param>
        public void RemoveSound(string name)
        {
            _sounds.Remove(name.ToUpperInvariant());

            // stop the sound from playing and remove it from everywhere it is associated with
            var instances = _currentSoundInstances.Where(x => x.Value.Name == name);
            var instancesToKill = new Dictionary<string, List<ulong>>();
            foreach (var instance in instances)
            {
                if (!instancesToKill.ContainsKey(instance.Value.CategoryName))
                {
                    instancesToKill.Add(instance.Value.CategoryName, new List<ulong>());
                }
                instancesToKill[instance.Value.CategoryName].Add(instance.Key);
            }

            foreach (var killCategory in instancesToKill)
            {
                RemoveAudioInstances(killCategory.Value, killCategory.Key);
            }
        }

        /// <summary>
        ///     Removes the sound described by names.
        /// </summary>
        ///
        /// <param name="names"> The names. </param>
        public void RemoveSound(List<string> names)
        {
            for (var i = 0; i < names.Count; i++)
            {
                RemoveSound(names[i].ToUpperInvariant());
            }
        }

        /// <summary>
        ///     Removes the sound tag.
        /// </summary>
        ///
        /// <exception cref="Exception"> Thrown when an exception error condition occurs. </exception>
        ///
        /// <param name="category">                The category. </param>
        /// <param name="name">                    The name. </param>
        /// <param name="removeOnlyFirstinstance"> (Optional) True to remove only firstinstance. </param>
        ///
        /// <returns>
        ///     True if it succeeds, false if it fails.
        /// </returns>
        public bool RemoveSoundTag(string category, string name, bool removeOnlyFirstinstance = true)
        {
            category = category.ToUpperInvariant();
            if (category == MUSIC_CATEGORY)
            {
                throw new Exception("Can't remove a sound tag from the Music Category!");
            }
            return _categories[category].RemoveTag(name, removeOnlyFirstinstance);
        }

        /// <summary>
        ///     Removes the sound tag.
        /// </summary>
        ///
        /// <exception cref="Exception"> Thrown when an exception error condition occurs. </exception>
        ///
        /// <param name="category">                The category. </param>
        /// <param name="names">                   The names. </param>
        /// <param name="removeOnlyFirstinstance"> (Optional) True to remove only firstinstance. </param>
        ///
        /// <returns>
        ///     True if it succeeds, false if it fails.
        /// </returns>
        public List<bool> RemoveSoundTag(string category, List<string> names, bool removeOnlyFirstinstance = true)
        {
            category = category.ToUpperInvariant();
            if (category == MUSIC_CATEGORY)
            {
                throw new Exception("Can't remove a sound tag from the Music Category!");
            }
            return _categories[category].RemoveTag(names, removeOnlyFirstinstance);
        }

        /// <summary>
        ///     Removes the sound tag.
        /// </summary>
        ///
        /// <exception cref="Exception"> Thrown when an exception error condition occurs. </exception>
        ///
        /// <param name="category"> The category. </param>
        /// <param name="tags">     The tags. </param>
        ///
        /// <returns>
        ///     True if it succeeds, false if it fails.
        /// </returns>
        public List<bool> RemoveSoundTag(string category, List<(string, bool)> tags)
        {
            category = category.ToUpperInvariant();
            if (category == MUSIC_CATEGORY)
            {
                throw new Exception("Can't remove a sound tag from the Music Category!");
            }
            return _categories[category].RemoveTag(tags);
        }

        /// <summary>
        ///     Removes the tag.
        /// </summary>
        ///
        /// <param name="category">                The category. </param>
        /// <param name="name">                    The name. </param>
        /// <param name="removeOnlyFirstinstance"> (Optional) True to remove only firstinstance. </param>
        ///
        /// <returns>
        ///     A List&lt;bool&gt;
        /// </returns>
        public bool RemoveTag(string category, string name, bool removeOnlyFirstinstance = true)
        {
            return _categories[category.ToUpperInvariant()].RemoveTag(name, removeOnlyFirstinstance);
        }

        /// <summary>
        ///     Removes the tag.
        /// </summary>
        ///
        /// <param name="category">                The category. </param>
        /// <param name="names">                   The names. </param>
        /// <param name="removeOnlyFirstinstance"> (Optional) True to remove only firstinstance. </param>
        ///
        /// <returns>
        ///     A List&lt;bool&gt;
        /// </returns>
        public List<bool> RemoveTag(string category, List<string> names, bool removeOnlyFirstinstance = true)
        {
            return _categories[category.ToUpperInvariant()].RemoveTag(names, removeOnlyFirstinstance);
        }

        /// <summary>
        ///     Removes the tag.
        /// </summary>
        ///
        /// <param name="category"> The category. </param>
        /// <param name="tags">     The tags. </param>
        ///
        /// <returns>
        ///     A List&lt;bool&gt;
        /// </returns>
        public List<bool> RemoveTag(string category, List<(string, bool)> tags)
        {
            return _categories[category.ToUpperInvariant()].RemoveTag(tags);
        }

        /// <summary>
        ///     Sets category mute status.
        /// </summary>
        ///
        /// <param name="category"> The category. </param>
        /// <param name="isMuted">  True if is muted, false if not. </param>
        public void SetCategoryMuteStatus(string category, bool isMuted)
        {
            category = category.ToUpperInvariant();
            _categories[category].IsMuted = isMuted;
        }

        /// <summary>
        ///     Sets category volume.
        /// </summary>
        ///
        /// <param name="category"> The category. </param>
        /// <param name="volume">   The volume. </param>
        public void SetCategoryVolume(string category, float volume)
        {
            category = category.ToUpperInvariant();
            _categories[category].UpdateVolume(volume);
        }

        /// <summary>
        ///     Sets category volume.
        /// </summary>
        ///
        /// <param name="category"> The category. </param>
        /// <param name="volume">   The volume. </param>
        public void SetCategoryVolume(string category, int volume)
        {
            SetCategoryVolume(category, volume / 100f);
        }

        /// <summary>
        ///     Sets global volume.
        /// </summary>
        ///
        /// <param name="volume"> The volume. </param>
        public void SetGlobalVolume(float volume)
        {
            GlobalVolume = volume;
        }

        /// <summary>
        ///     Sets global volume.
        /// </summary>
        ///
        /// <param name="volume"> The volume. </param>
        public void SetGlobalVolume(int volume)
        {
            GlobalVolume = volume / 100f;
        }

        /// <summary>
        ///     Updates the class.
        /// </summary>
        ///
        /// <param name="gameTime"> The game time. </param>
        ///
        /// <seealso cref="IUpdateable.Update(GameTime)"/>
        public void Update(GameTime gameTime)
        {
            LastUpdateTime = gameTime.TotalGameTime;

            // loop through all sound categories and update them as required...
            foreach (var category in _categories)
            {
                category.Value.Update(gameTime);
            }
        }

        /// <summary>
        ///     Gets audio instances for category.
        /// </summary>
        ///
        /// <param name="category"> The category. </param>
        ///
        /// <returns>
        ///     The audio instances for category.
        /// </returns>
        internal List<AudioInstance> GetAudioInstancesForCategory(string category)
        {
            category = category.ToUpperInvariant();
            var output = new List<AudioInstance>();
            switch (category)
            {
                case MUSIC_CATEGORY:
                    foreach (var instance in _currentMusicInstances)
                    {
                        output.Add(instance.Value);
                    }

                    break;

                default:
                    var ids = _categories[category].GetInstanceIds();
                    for (var i = 0; i < ids.Count; i++)
                    {
                        output.Add(_currentSoundInstances[ids[i]]);
                    }

                    break;
            }

            return output;
        }

        /// <summary>
        ///     Determine maximum sound instances.
        /// </summary>
        ///
        /// <returns>
        ///     An int.
        /// </returns>
        private int DetermineMaxSoundInstances()
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.MacOSX:
                    return 256;

                case PlatformID.Unix:
                    return 32;

                case PlatformID.Xbox:
                    return 300;

                default:
                    return int.MaxValue;
            }
        }
    }
}