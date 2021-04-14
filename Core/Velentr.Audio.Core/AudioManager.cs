using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Velentr.Audio.Internal;
using Velentr.Collections.CollectionActions;
using Velentr.Collections.Collections.Concurrent;

namespace Velentr.Audio
{

    public class AudioManager
    {
        private Dictionary<string, Audio> _music;

        private Dictionary<string, Audio> _sounds;

        private ConcurrentPool<SoundEffectInstance> _musicPool;
        private ConcurrentPool<SoundEffectInstance> _soundPool;

        private Dictionary<string, AudioChannel> _channels;

        public int MaxTotalAudioInstances { get; }

        public int MusicReservedAudioInstanceCount { get; set; }

        public int SoundsReservedAudioInstanceCount => MaxTotalAudioInstances - MusicReservedAudioInstanceCount;

        public AudioManager(int maxTotalAudioInstances = -1, int musicReservedAudioInstanceCount = 3)
        {
            _music = new Dictionary<string, Audio>();
            _sounds = new Dictionary<string, Audio>();

            MaxTotalAudioInstances = maxTotalAudioInstances >= -1
                ? maxTotalAudioInstances
                : int.MaxValue;

            MusicReservedAudioInstanceCount = musicReservedAudioInstanceCount;
            _musicPool = new ConcurrentPool<SoundEffectInstance>(actionWhenPoolFull: PoolFullAction.ReturnNull, capacity: MusicReservedAudioInstanceCount);
            _soundPool = new ConcurrentPool<SoundEffectInstance>(capacity: 16);
            _channels = new Dictionary<string, AudioChannel>()
            {
                {"MUSIC", new AudioChannel("MUSIC", true, 1.0f)},
                {"SOUNDS", new AudioChannel("SOUNDS", false, 1.0f)}
            };
        }

        public void AddSoundChannel(string name)
        {

        }

        public void RemoveSoundChannel(string name)
        {

        }

        public void AdjustSoundChannelName(string name, int newVolume)
        {

        }

        public void AdjustSoundChannelName(string name, float newVolume)
        {

        }

        public void Update()
        {

        }

        public void UpdateMasterVolume(int newVolume)
        {

        }

        public void UpdateMasterVolume(float newVolume)
        {

        }



        public void AddMusic(string name, string path, bool autoLoad = true)
        {
            var music = new Music(name, path, this);
            if (autoLoad)
            {
                music.Load();
            }
            _music.Add(name, music);
        }

        public void AddSound(string name, string path, bool autoLoad = true)
        {
            var sound = new Sound(name, path, this);
            if (autoLoad)
            {
                sound.Load();
            }
            _sounds.Add(name, sound);
        }

        
    }
}
