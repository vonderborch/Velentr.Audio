using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Velentr.Audio.Audio;
using Velentr.Audio.Helpers;

namespace Velentr.Audio
{
    public class AudioManager
    {

        public Randomizer Randomizer;

        private float _globalVolume = 1f;

        private ulong _audioInstanceId;

        private Dictionary<string, Music> _music;

        private Dictionary<string, Sound> _sounds;

        public AudioManager(int maxMusicInstances = 8, Randomizer randomizer = null)
        {
            TotalMaxAudioInstances = DetermineMaxSoundInstances();
            MaxMusicInstances = maxMusicInstances;
            MaxSoundInstances = TotalMaxAudioInstances - maxMusicInstances;
            CurrentMusicInstances = 0;
            CurrentSoundInstances = 0;
            _audioInstanceId = 0;

            Randomizer = randomizer ?? new SystemRandomizer();

            _music = new Dictionary<string, Music>();
            _sounds = new Dictionary<string, Sound>();
        }

        public int TotalMaxAudioInstances { get; }

        public int MaxMusicInstances { get; }

        public int MaxSoundInstances { get; }

        public int CurrentMusicInstances { get; internal set; }

        public int CurrentSoundInstances { get; internal set; }

        public bool HasFreeMusicInstances => MaxMusicInstances < CurrentMusicInstances;

        public bool HasFreeSoundInstances => MaxSoundInstances < CurrentSoundInstances;

        public TimeSpan LastUpdateTime { get; private set; }

        public void AddMusic(string name, string path, float defaultPitch = 0, bool autoLoad = false, bool loadOnlyToCreateInstance = false)
        {
            _music.Add(name, new Music(this, name, path, defaultPitch, autoLoad, loadOnlyToCreateInstance));
        }

        public void AddMusic(MusicLoadInfo loadInfo)
        {
            _music.Add(loadInfo.Name, new Music(this, loadInfo.Name, loadInfo.Path, loadInfo.DefaultPitch, loadInfo.AutoLoad, loadInfo.LoadOnlyToCreateInstance));
        }

        public void AddMusic(List<MusicLoadInfo> musicTracks)
        {
            for (var i = 0; i < musicTracks.Count; i++)
            {
                AddMusic(musicTracks[i]);
            }
        }

        public void AddSound(string name, string path, List<string> categories, float defaultPitch = 0, bool autoLoad = false, bool loadOnlyToCreateInstance = false, List<string> tags = null, List<string> exclusionTags = null, List<string> requiredTags = null)
        {
            _sounds.Add(name, new Sound(this, name, path, categories, defaultPitch, autoLoad, loadOnlyToCreateInstance, tags, exclusionTags, requiredTags));
        }

        public void AddSound(SoundLoadInfo loadInfo)
        {
            _sounds.Add(loadInfo.Name, new Sound(this, loadInfo.Name, loadInfo.Path, loadInfo.Categories, loadInfo.DefaultPitch, loadInfo.AutoLoad, loadInfo.LoadOnlyToCreateInstance, loadInfo.Tags.Tags, loadInfo.Tags.ExclusionTags, loadInfo.Tags.RequiredTags));
        }

        public void AddSound(List<SoundLoadInfo> sounds)
        {
            for (var i = 0; i < sounds.Count; i++)
            {
                AddSound(sounds[i]);
            }
        }

        public void RemoveSound(string name)
        {
            _sounds.Remove(name);

            // stop the sound from playing and remove it from everywhere it is associated with
            
        }

        public void RemoveSound(List<string> names)
        {
            for (var i = 0; i < names.Count; i++)
            {
                RemoveSound(names[i]);
            }
        }

        public void RemoveMusic(string name)
        {
            _music.Remove(name);

            // stop the music from playing (if it is) and remove it from everywhere it is associated with
        }

        public void RemoveMusic(List<string> names)
        {
            for (var i = 0; i < names.Count; i++)
            {
                RemoveMusic(names[i]);
            }
        }

        public void Update(GameTime gameTime)
        {
            LastUpdateTime = gameTime.TotalGameTime;
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
