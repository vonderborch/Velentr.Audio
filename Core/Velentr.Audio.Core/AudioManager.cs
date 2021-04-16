using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Velentr.Collections.Collections.Concurrent;

namespace Velentr.Audio
{
    public class AudioManager
    {

        public const string MUSIC_CATEGORY = "MUSIC";

        public const string DEFAULT_SOUND_CATEGORY = "SOUNDS";

        internal Dictionary<string, Category> _categories;

        private Dictionary<string, Audio> _audio;

        private Dictionary<string, Playlist> _playlists;

        private List<AudioInstance> _instances;

        private ulong _audioInstanceId;

        public AudioManager(int maxMusicInstances = 8, bool createDefaultCategories = true)
        {
            TotalMaxAudioInstances = DetermineMaxSoundInstances();
            MaxMusicInstances = maxMusicInstances;
            MaxSoundInstances = TotalMaxAudioInstances - maxMusicInstances;
            CurrentMusicInstances = 0;
            CurrentSoundInstances = 0;
            _audioInstanceId = 0;

            _playlists = new Dictionary<string, Playlist>();
            _categories = new Dictionary<string, Category>();
            if (createDefaultCategories)
            {
                _categories.Add(MUSIC_CATEGORY, new Category(MUSIC_CATEGORY, true));
                _categories.Add(DEFAULT_SOUND_CATEGORY, new Category(DEFAULT_SOUND_CATEGORY, false));
            }
        }

        public string CurrentPlaylist { get; set; }

        internal void RemoveAudioInstance(ulong id)
        {
            int index;
            var isMusicInstance = false;
            for (index = 0; index < _instances.Count; index++)
            {
                if (_instances[index].ID == id)
                {
                    isMusicInstance = _categories[_instances[index].CategoryName].IsMusicCategory;
                    break;
                }
            }

            if (index < _instances.Count)
            {
                _instances.RemoveAt(index);
                if (isMusicInstance)
                {
                    CurrentMusicInstances--;
                }
                else
                {
                    CurrentSoundInstances--;
                }
            }
        }

        public int TotalMaxAudioInstances { get; }

        public int MaxMusicInstances { get; }

        public int MaxSoundInstances { get; }

        public int CurrentMusicInstances { get; internal set; }

        public int CurrentSoundInstances { get; internal set; }

        public bool HasFreeMusicInstances => MaxMusicInstances < CurrentMusicInstances;

        public bool HasFreeSoundInstances => MaxSoundInstances < CurrentSoundInstances;

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

        public void AddAudio(List<AudioLoadInfo> loadInfos)
        {
            for (var i = 0; i < loadInfos.Count; i++)
            {
                AddAudio(loadInfos[i]);
            }
        }

        public void AddAudio(AudioLoadInfo loadInfo)
        {
            var audio = new Audio(this, loadInfo.Name, loadInfo.CategoryName, loadInfo.Path, loadInfo.DefaultPitch, loadInfo.AutoLoad, loadInfo.LoadOnlyToCreateInstance, loadInfo.Tags, loadInfo.ExclusionTags, loadInfo.RequiredTags);

            _audio.Add(loadInfo.Name, audio);
        }

        public void AddAudio(string name, string categoryName, string path, float defaultPitch = 0f, bool autoLoad = false, bool loadOnlyToCreateInstance = false, List<string> tags = null, List<string> exclusionTags = null, List<string> requiredTags = null)
        {
            AddAudio(new AudioLoadInfo(name, categoryName, path, defaultPitch, autoLoad, loadOnlyToCreateInstance, tags, exclusionTags, requiredTags));
        }

        public void AddCategory(string name, bool isMusicCategory = false, float volume = 1f)
        {
            if (_categories.ContainsKey(name))
            {
                throw new Exception($"A category with the name [{name}] already exist!");
            }

            _categories.Add(name, new Category(name, isMusicCategory, volume));
        }

        public bool RemoveCategory(string name, string newAudioCategory = null)
        {
            var relatedAudio = _audio.Where(x => x.Value.CategoryName == name).ToList();
            if (!string.IsNullOrEmpty(newAudioCategory))
            {
                // update all associated audio to the new category
                foreach (var audio in relatedAudio)
                {
                    audio.Value.CategoryName = newAudioCategory;
                }

                // make sure that our audio volumes are correct for the new channel
                _categories[newAudioCategory].ChangeVolume(_categories[newAudioCategory].CurrentVolume);
            }
            else
            {
                // dispose of the associated audio!
                foreach (var audio in relatedAudio)
                {
                    _audio.Remove(audio.Key);
                    audio.Value.Dispose();
                }
            }

            // remove the category 
            return _categories.Remove(name);
        }

        public void AddTagToCategory(string categoryName, string tag)
        {
            _categories[categoryName].AddTag(tag);
        }

        public void AddTagsToCategory(string categoryName, List<string> tags)
        {
            _categories[categoryName].AddTags(tags);
        }

        public bool RemoveTagFromCategory(string categoryName, string tag)
        {
            return _categories[categoryName].RemoveTag(tag);
        }

        public void RemoveTagsFromCategory(string categoryName, List<string> tags)
        {
            _categories[categoryName].RemoveTags(tags);
        }

        public Category GetCategory(string categoryName)
        {
            return _categories[categoryName];
        }

        public void UpdateCategoryVolume(string categoryName, float volume)
        {
            var instances = GetInstancesForCategory(categoryName);

            for (var i = 0; i < instances.Count; i++)
            {
                instances[i].UpdateVolume(volume);
            }

        }

        public void UpdateCategoryVolume(string categoryName, int volume)
        {
            UpdateCategoryVolume(categoryName, volume / 100f);
        }

        public void Update(GameTime gameTime)
        {
            // create instances based on tags in categories
            foreach (var category in _categories)
            {
                if (category.Key != MUSIC_CATEGORY)
                {
                    UpdateCategory(category.Key);
                }
            }

            // update music

        }

        public void AddPlaylist(string name, Playlist playlist)
        {
            _playlists.Add(name, playlist);
        }

        public void AddPlaylist(Dictionary<string, Playlist> playlists)
        {
            foreach (var playlist in playlists)
            {
                _playlists.Add(playlist.Key, playlist.Value);
            }
        }

        public bool RemovePlaylist(string name)
        {
            return _playlists.Remove(name);
        }

        public void RemovePlaylist(List<string> names)
        {
            for (var i = 0; i < names[i].Length; i++)
            {
                _playlists.Remove(names[i]);
            }
        }

        internal List<AudioInstance> GetInstancesForCategory(string name)
        {
            return _instances.Where(x => x.CategoryName == name).ToList();
        }

        internal List<KeyValuePair<string, Audio>> GetAudioForCategory(string name)
        {
            return _audio.Where(x => x.Value.CategoryName == name).ToList();
        }

        private void UpdateCategory(string categoryName)
        {
            var category = _categories[categoryName];
            var instances = _instances.Where(x => x.CategoryName == categoryName).ToList();

            for (var i = 0; i < instances.Count; i++)
            {
                // remove any instances that are currently stopped
                if (instances[i].State == SoundState.Stopped)
                {
                    RemoveAudioInstance(instances[i].ID);
                }
            }

            // create new instances as required
            var validAudio = GetAudioForCategory(categoryName);
            foreach (var audio in validAudio)
            {
                if (audio.Value.IsValid(category.GetTags(), out var validTags))
                {
                    if (category.ConsumeValidTagsWhenCreatingInstance)
                    {
                        category.RemoveTags(validTags);
                    }

                    var instance = audio.Value.GetInstance();
                    instance.Play();
                    _instances.Add(new AudioInstance(_audioInstanceId++, audio.Key, categoryName, instance, category.IsMusicCategory));
                }
            }
        }
    }
}
