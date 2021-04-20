using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Velentr.Audio.OLD.Categories;
using Velentr.Audio.OLD.Helpers;
using Velentr.Audio.OLD.Sound;
using Velentr.Audio.OLD.Tagging;
using Velentr.Collections.CollectionActions;
using Velentr.Collections.Collections.Concurrent;

namespace Velentr.Audio.OLD
{
    public class AudioManager
    {

        public const string MUSIC_CATEGORY = "MUSIC";

        private ConcurrentPool<AudioInstance> _musicPool;

        private Dictionary<ulong, AudioInstance> _currentMusicInstances;

        private ConcurrentPool<AudioInstance> _soundPool;

        private Dictionary<ulong, AudioInstance> _currentSoundInstances;

        private ulong _audioInstanceId;

        private Category _musicCategory;

        private Dictionary<string, Category> _soundCategories;

        private Dictionary<string, Sound.Audio> _audio;

        private string _currentPlayList;

        private Dictionary<string, Playlist> _playlists;

        private float _globalVolume = 1f;

        public AudioManager(int maxMusicInstances = 8, Randomizer randomizer = null)
        {
            TotalMaxAudioInstances = DetermineMaxSoundInstances();
            MaxMusicInstances = maxMusicInstances;
            MaxSoundInstances = TotalMaxAudioInstances - maxMusicInstances;
            CurrentMusicInstances = 0;
            CurrentSoundInstances = 0;
            _audioInstanceId = 0;

            _audio = new Dictionary<string, Sound.Audio>();

            _musicPool = new ConcurrentPool<AudioInstance>(capacity: MaxMusicInstances, actionWhenPoolFull: PoolFullAction.ReturnNull);
            _currentMusicInstances = new Dictionary<ulong, AudioInstance>(MaxMusicInstances);
            _musicCategory = new Category(this, MUSIC_CATEGORY, true);

            _soundPool = new ConcurrentPool<AudioInstance>(actionWhenPoolFull: PoolFullAction.IncreaseSize);
            _currentSoundInstances = new Dictionary<ulong, AudioInstance>(MaxMusicInstances * 2);
            _soundCategories = new Dictionary<string, Category>();

            Randomizer = randomizer ?? new SystemRandomizer();

            _currentPlayList = string.Empty;
            _playlists = new Dictionary<string, Playlist>();
        }

        public Randomizer Randomizer;

        public int TotalMaxAudioInstances { get; }

        public int MaxMusicInstances { get; }

        public int MaxSoundInstances { get; }

        public int CurrentMusicInstances { get; internal set; }

        public int CurrentSoundInstances { get; internal set; }

        public bool HasFreeMusicInstances => MaxMusicInstances < CurrentMusicInstances;

        public bool HasFreeSoundInstances => MaxSoundInstances < CurrentSoundInstances;

        public float GlobalVolume
        {
            get => _globalVolume;
            set
            {
                _globalVolume = value;
                var categories = GetCategoryNames(true);
                for (var i = 0; i < categories.Count; i++)
                {
                    UpdateCategoryVolume(categories[i], _soundCategories[categories[i]].CurrentVolume);
                }
            }
        }

        internal float GetMusicVolume()
        {
            return _musicCategory != null
                ? _musicCategory.CurrentVolume
                : 0f;
        }

        public TimeSpan LastUpdateTime { get; private set; }

        public void Update(GameTime gameTime)
        {
            LastUpdateTime = gameTime.TotalGameTime;

            // go through each sound instance and see if they're done playing
            var soundIdsToRemove = new List<ulong>();
            foreach (var sound in _currentSoundInstances)
            {
                if (sound.Value.State == SoundState.Stopped)
                {
                    soundIdsToRemove.Add(sound.Key);
                }
            }
            RemoveSoundInstances(soundIdsToRemove);

            // go through each category and create new sound instances as we can/should
            foreach (var category in _soundCategories)
            {
                category.Value.Update();
            }

            // update our current music/playlist
            if (GetMusicVolume() > 0f)
            {
                var currentMusicTags = _musicCategory.GetTags();
                var playNewPlaylist = true;
                if (!string.IsNullOrEmpty(_currentPlayList))
                {
                    _playlists[_currentPlayList].Update(currentMusicTags);
                    var play = _playlists[_currentPlayList].ShouldPlaylistBePlayed(currentMusicTags);
                    playNewPlaylist = !play;
                }

                if (playNewPlaylist)
                {
                    var validPlaylists = new List<string>();
                    foreach (var playlist in _playlists)
                    {
                        if (playlist.Value.ShouldPlaylistBePlayed(currentMusicTags))
                        {
                            validPlaylists.Add(playlist.Key);
                        }
                    }

                    if (validPlaylists.Count > 0)
                    {
                        _currentPlayList = validPlaylists[0];
                        _playlists[_currentPlayList].Update(currentMusicTags);
                    }
                }
            }
        }

        internal void RemoveSoundInstances(List<ulong> ids)
        {
            for (var i = 0; i < ids.Count; i++)
            {
                _currentSoundInstances.Remove(ids[i]);
                CurrentSoundInstances--;
            }
        }

        internal List<Sound.Audio> GetAudio(List<string> names)
        {
            var output = new List<Sound.Audio>(names.Count);

            for (var i = 0; i < names.Count; i++)
            {
                output.Add(_audio[names[i]]);
            }

            return output;
        }

        internal void RemoveMusicInstances(List<ulong> ids)
        {
            for (var i = 0; i < ids.Count; i++)
            {
                _currentMusicInstances.Remove(ids[i]);
                CurrentMusicInstances--;
            }
        }

        public void AddPlaylist(string name, bool randomize, int maxTriesToAvoidRepeats = 3, List<string> associatedAudio = null, TagSet? playingTags = null, TagSet? pauseTags = null, TagSet? stopTags = null)
        {
            var newPlaylist = new Playlist(this, name, randomize, maxTriesToAvoidRepeats);

            if (associatedAudio != null)
            {
                for (var i = 0; i < associatedAudio.Count; i++)
                {
                    newPlaylist.AddAssociatedAudio(associatedAudio[i]);
                }
            }

            if (playingTags != null)
            {
                newPlaylist.UpdatePlaylistValidTagset((TagSet)playingTags);
            }

            if (pauseTags != null)
            {
                newPlaylist.UpdatePlaylistPausedTagset((TagSet)pauseTags);
            }

            if (stopTags != null)
            {
                newPlaylist.UpdatePlaylistStopTagset((TagSet)stopTags);
            }

            _playlists.Add(name, newPlaylist);
        }

        public Playlist GetPlaylist(string name)
        {
            return _playlists[name];
        }

        public void AddAudio(string name, string path, float defaultPitch = 0f, bool autoLoad = false, bool loadOnlyToCreateInstance = false, List<string> tags = null, List<string> exclusionTags = null, List<string> requiredTags = null)
        {
            _audio.Add(name, new Sound.Audio(this, name, path, defaultPitch, autoLoad, loadOnlyToCreateInstance, tags, exclusionTags, requiredTags));
        }

        public void AddAudio(AudioLoadInfo loadInfo)
        {
            _audio.Add(loadInfo.Name, new Sound.Audio(this, loadInfo.Name, loadInfo.Path, loadInfo.DefaultPitch, loadInfo.AutoLoad, loadInfo.LoadOnlyToCreateInstance, loadInfo.Tags, loadInfo.ExclusionTags, loadInfo.RequiredTags));
        }

        public void AddAudio(List<AudioLoadInfo> loadInfo)
        {
            for (var i = 0; i < loadInfo.Count; i++)
            {
                AddAudio(loadInfo[i]);
            }
        }

        public void RemoveAudio(string name)
        {
            _audio.Remove(name);

            _musicCategory.DeRegisterAudio(name);
            foreach (var category in _soundCategories)
            {
                category.Value.DeRegisterAudio(name);
            }

            foreach (var playlist in _playlists)
            {
                playlist.Value.RemoveAssociatedAudio(name);
            }
        }

        internal ulong GenerateNewAudioInstance(string name, string category)
        {
            if (category == MUSIC_CATEGORY && !HasFreeMusicInstances)
            {
                throw new Exception("No free music instances available!");
            }
            if (category != MUSIC_CATEGORY && !HasFreeSoundInstances)
            {
                throw new Exception("No free sound instances available!");
            }

            var instance = _audio[name].GetInstance();
            AudioInstance newAudioInstance;

            if (category == MUSIC_CATEGORY)
            {
                newAudioInstance = _musicPool.Get();
                if (newAudioInstance == null)
                {
                    throw new Exception("No free music instances available!");
                }

                newAudioInstance.UpdateInstance(this, _audioInstanceId++, name, category, instance);
                _currentMusicInstances.Add(newAudioInstance.ID, newAudioInstance);
                CurrentMusicInstances++;
            }
            else
            {
                newAudioInstance = _soundPool.Get();
                if (newAudioInstance == null)
                {
                    throw new Exception("No free sound instances available!");
                }

                newAudioInstance.UpdateInstance(this, _audioInstanceId++, name, category, instance);
                _currentSoundInstances.Add(newAudioInstance.ID, newAudioInstance);
                CurrentSoundInstances++;
            }

            newAudioInstance.Play(false);
            return newAudioInstance.ID;
        }

        public List<string> GetCategoryNames(bool includeMusicCategory = false)
        {
            var output = new List<string>();
            if (includeMusicCategory && _musicCategory != null)
            {
                output.Add(MUSIC_CATEGORY);
            }

            foreach (var category in _soundCategories)
            {
                output.Add(category.Key);
            }

            return output;
        }

        internal List<Sound.Audio> GetAudioForCategory(string category)
        {
            var output = new List<Sound.Audio>();
            List<string> audioNames;
            switch (category)
            {
                case MUSIC_CATEGORY:
                    audioNames = _musicCategory.GetRegisteredAudio();
                    break;
                default:
                    audioNames = _soundCategories[category].GetRegisteredAudio();
                    break;
            }

            for (var i = 0; i < audioNames.Count; i++)
            {
                output.Add(_audio[audioNames[i]]);
            }

            return output;
        }

        internal List<AudioInstance> GetMusicInstances(List<ulong> ids)
        {
            var output = new List<AudioInstance>();
            for (var i = 0; i < ids.Count; i++)
            {
                output.Add(_currentMusicInstances[ids[i]]);
            }

            return output;
        }

        internal List<AudioInstance> GetAudioInstancesForCategory(string category)
        {
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
                    var ids = _soundCategories[category].GetInstanceIds();
                    for (var i = 0; i < ids.Count; i++)
                    {
                        output.Add(_currentSoundInstances[ids[i]]);
                    }

                    break;
            }

            return output;
        }

        public void UpdateMusicVolume(float volume)
        {
            UpdateCategoryVolume(MUSIC_CATEGORY, volume);
        }

        public void UpdateMusicVolume(int volume)
        {
            UpdateCategoryVolume(MUSIC_CATEGORY, volume);
        }

        public void UpdateCategoryVolume(string category, int volume)
        {
            UpdateCategoryVolume(category, volume / 100f);
        }

        public void UpdateCategoryVolume(string category, float volume)
        {
            switch (category)
            {
                case MUSIC_CATEGORY:
                    _musicCategory.UpdateVolume(volume);
                    break;
                default:
                    _soundCategories[category].UpdateVolume(volume);
                    break;
            }
        }

        public void AddCategory(string name, float volume = 1.0f, FullInstancesAction actionWhenNoFreeInstances = FullInstancesAction.ThrowException, bool overrideExistingCategory = true)
        {
            switch (name)
            {
                case MUSIC_CATEGORY:
                    if (!overrideExistingCategory && _musicCategory != null)
                    {
                        throw new Exception("Category already exists!");
                    }

                    _musicCategory = new Category(this, name, true, volume, actionWhenNoFreeInstances);
                    break;
                default:
                    _soundCategories.Add(name, new Category(this, name, false, volume, actionWhenNoFreeInstances));
                    break;
            }
        }

        public bool RemoveCategory(string name)
        {
            switch (name)
            {
                case MUSIC_CATEGORY:
                    if (_musicCategory == null)
                    {
                        return false;
                    }
                    else
                    {
                        _musicCategory = null;
                        return true;
                    }
                default:
                    return _soundCategories.Remove(name);
            }
        }

        public void AddMusicTag(string tag, bool consumable = true, uint lifespanMilliseconds = uint.MaxValue, TagPriority priority = TagPriority.Normal)
        {
            AddSoundTag(MUSIC_CATEGORY, tag, consumable, lifespanMilliseconds, priority);
        }

        public void AddMusicTag(Tag tag)
        {
            AddSoundTag(MUSIC_CATEGORY, tag);
        }

        public void AddMusicTag(List<Tag> tags)
        {
            AddSoundTag(MUSIC_CATEGORY, tags);
        }

        public void AddSoundTag(string category, string tag, bool consumable = true, uint lifespanMilliseconds = 100, TagPriority priority = TagPriority.Normal)
        {
            AddSoundTag(category, new Tag(tag, consumable, lifespanMilliseconds, priority));
        }

        public void AddSoundTag(string category, Tag tag)
        {
            switch (category)
            {
                case MUSIC_CATEGORY:
                    _musicCategory.AddTag(tag);
                    break;
                default:
                    _soundCategories[category].AddTag(tag);
                    break;
            }
        }

        public void AddSoundTag(string category, List<Tag> tags)
        {
            for (var i = 0; i < tags.Count; i++)
            {
                AddSoundTag(category, tags[i]);
            }
        }

        public void AddSoundTag(Dictionary<string, List<Tag>> categoriesAndTags)
        {
            foreach (var category in categoriesAndTags)
            {
                for (var i = 0; i < category.Value.Count; i++)
                {
                    AddSoundTag(category.Key, category.Value[i]);
                }
            }
        }

        public List<Tag> GetMusicTags()
        {
            return GetSoundTags(MUSIC_CATEGORY);
        }

        public List<Tag> GetSoundTags(string category)
        {
            switch (category)
            {
                case MUSIC_CATEGORY:
                    return _musicCategory.GetTags();
                default:
                    return _soundCategories[category].GetTags();
            }
        }

        public Dictionary<string, List<Tag>> GetSoundTags(List<string> categories)
        {
            var output = new Dictionary<string, List<Tag>>();
            for (var i = 0; i < categories.Count; i++)
            {
                output.Add(categories[i], GetSoundTags(categories[i]));
            }

            return output;
        }

        public void RemoveMusicTag(string tag)
        {
            RemoveSoundTag(MUSIC_CATEGORY, tag);
        }

        public void RemoveMusicTag(List<string> tags)
        {
            RemoveSoundTag(MUSIC_CATEGORY, tags);
        }

        public void RemoveSoundTag(string category, string tag)
        {
            switch (category)
            {
                case MUSIC_CATEGORY:
                    _musicCategory.RemoveTag(tag);
                    break;
                default:
                    _soundCategories[category].RemoveTag(tag);
                    break;
            }
        }

        public void RemoveSoundTag(string category, List<string> tags)
        {
            for (var i = 0; i < tags.Count; i++)
            {
                RemoveSoundTag(category, tags[i]);
            }
        }

        public void RemoveSoundTag(Dictionary<string, List<string>> categoriesAndTags)
        {
            foreach (var category in categoriesAndTags)
            {
                for (var i = 0; i < category.Value.Count; i++)
                {
                    RemoveSoundTag(category.Key, category.Value[i]);
                }
            }
        }

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
