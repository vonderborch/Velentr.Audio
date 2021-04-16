using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework.Audio;

namespace Velentr.Audio
{
    public class Audio : IDisposable
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        ///
        /// <param name="manager">                  The manager. </param>
        /// <param name="name">                     The name. </param>
        /// <param name="categoryName">             Name of the category. </param>
        /// <param name="path">                     Full pathname of the file. </param>
        /// <param name="defaultPitch">             (Optional) The default pitch. </param>
        /// <param name="autoLoad">                 (Optional) True to automatically load. </param>
        /// <param name="loadOnlyToCreateInstance">
        ///     (Optional) True to load only to create instance.
        /// </param>
        /// <param name="tags">
        ///     (Optional) The tags. These are effectively 'Any' tags. If any exist in the current set of
        ///     tags associated with the manager, this music is valid.
        /// </param>
        /// <param name="exclusionTags">
        ///     (Optional)
        ///     The exclusion tags. These are the opposite of 'Any' tags. If any exists in the current
        ///     set of tags associated with the manager, this music is _not_ valid.
        /// </param>
        /// <param name="requiredTags">
        ///     (Optional) The required tags. These are effectively 'All' tags. All of these tags need to
        ///     exist in the current set of tags associated with the manager for this music to be valid.
        /// </param>
        public Audio(AudioManager manager, string name, string categoryName, string path, float defaultPitch = 0f, bool autoLoad = false, bool loadOnlyToCreateInstance = false, List<string> tags = null, List<string> exclusionTags = null, List<string> requiredTags = null)
        {
            Name = name;
            CategoryName = categoryName;
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

        public SoundEffect Sound { get; private set; }

        public AudioManager Manager { get; }

        public bool LoadOnlyToCreateInstance { get; set; }

        public TagSet Tags { get; }

        public SoundEffectInstance GetInstance()
        {
            if (!Loaded && !LoadOnlyToCreateInstance)
            {
                throw new Exception($"Audio [{Name}] not loaded!");
            }

            if (
                (Category.IsMusicCategory && !Manager.HasFreeMusicInstances)
                || (!Category.IsMusicCategory && !Manager.HasFreeSoundInstances)
            )
            {
                switch (Category.ActionWhenNoFreeInstances)
                {
                    case FullInstancesAction.ThrowException:
                        throw new Exception($"No free instances available!");
                    case FullInstancesAction.FailCreation:
                        return null;
                    case FullInstancesAction.RemoveOldestInstance:
                        var instances = Manager.GetInstancesForCategory(CategoryName);
                        Manager.RemoveAudioInstance(instances[0].ID);
                        break;
                }
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
                instance.Volume = Category.CurrentVolume;

                if (Category.IsMusicCategory)
                {
                    Manager.CurrentMusicInstances++;
                }
                else
                {
                    Manager.CurrentSoundInstances++;
                }
            }

            return instance;
        }

        public bool Loaded { get; private set; }

        public string CategoryName { get; internal set; }

        public Category Category => Manager.GetCategory(CategoryName);

        public string Name { get; }

        public string Path { get; }

        public float DefaultPitch { get; set; }

        public int DefaultPitchInt
        {
            get => (int) (DefaultPitch * 100);
            set => DefaultPitch = value / 100f;
        }

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

        public bool IsValid(List<string> currentTags)
        {
            return Tags.IsValid(currentTags);
        }

        public bool IsValid(List<string> currentTags, out List<string> validTags)
        {
            return Tags.IsValid(currentTags, out validTags);
        }
    }
}
