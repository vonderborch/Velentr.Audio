using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Audio;
using Velentr.Audio.OLD.Tagging;

namespace Velentr.Audio.OLD.Sound
{
    public class Audio
    {

        public Audio(AudioManager manager, string name, string path, float defaultPitch = 0f, bool autoLoad = false, bool loadOnlyToCreateInstance = false, List<string> tags = null, List<string> exclusionTags = null, List<string> requiredTags = null)
        {
            Name = name;
            Path = path;
            DefaultPitch = defaultPitch;
            Loaded = false;
            Manager = manager;
            LoadOnlyToCreateInstance = loadOnlyToCreateInstance;
            Tags = new TagSet(tags, exclusionTags, requiredTags);

            if (autoLoad)
            {
                Load();
            }
        }

        public string Name { get; }

        public string Path { get; }

        public float DefaultPitch { get; set; }

        public int DefaultPitchInt
        {
            get => (int)(DefaultPitch * 100);
            set => DefaultPitch = value / 100f;
        }

        public SoundEffect Sound { get; private set; }

        public AudioManager Manager { get; }

        public bool LoadOnlyToCreateInstance { get; set; }

        public TagSet Tags;

        public bool Loaded { get; private set; }

        public void Load()
        {
            if (string.IsNullOrEmpty(Path) || !File.Exists(Path))
            {
                throw new Exception("Path must contain a valid path to a file!");
            }

            using (var stream = File.OpenRead(Path))
            {
                Sound = SoundEffect.FromStream(stream);
                Loaded = true;
            }
        }

        public void Dispose()
        {
            Sound?.Dispose();
            Loaded = false;
        }

        public bool IsValid(List<Tag> currentTags)
        {
            return Tags.IsValid(currentTags);
        }

        public bool IsValid(List<Tag> currentTags, out List<Tag> validTags)
        {
            return Tags.IsValid(currentTags, out validTags);
        }

        public SoundEffectInstance GetInstance()
        {
            if (!Loaded && !LoadOnlyToCreateInstance)
            {
                throw new Exception($"Audio [{Name}] not loaded!");
            }

            SoundEffectInstance instance = null;
            if (!Loaded)
            {
                using (var stream = File.OpenRead(Path))
                {
                    Sound = SoundEffect.FromStream(stream);
                    instance = Sound.CreateInstance();
                }
                Sound.Dispose();
            }
            else
            {
                instance = Sound.CreateInstance();
            }

            if (instance != null)
            {
                instance.Pitch = DefaultPitch;
            }

            return instance;
        }
    }
}
