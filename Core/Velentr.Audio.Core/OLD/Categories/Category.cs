using System.Collections.Generic;
using Velentr.Audio.OLD.Tagging;

namespace Velentr.Audio.OLD.Categories
{
    public class Category
    {

        private bool _isMuted = false;

        private float _currentVolume = 1.0f;

        private List<Tag> _tags;

        private List<ulong> _registeredInstances;

        private HashSet<string> _associatedAudio;

        internal Category(AudioManager manager, string name, bool isMusicCategory = false, float volume = 1.0f, FullInstancesAction actionWhenNoFreeInstances = FullInstancesAction.ThrowException)
        {
            Manager = manager;
            Name = name;
            IsMusicCategory = isMusicCategory;
            _currentVolume = volume;
            ActionWhenNoFreeInstances = actionWhenNoFreeInstances;

            _tags = new List<Tag>();
            _registeredInstances = new List<ulong>();
            _associatedAudio = new HashSet<string>();
        }

        public bool IsMusicCategory { get; }

        public string Name { get; }

        public FullInstancesAction ActionWhenNoFreeInstances { get; set; }

        public AudioManager Manager { get; }

        public bool IsMuted
        {
            get => _isMuted;
            set
            {
                _isMuted = value;
                UpdateVolume(_isMuted ? 0f : CurrentVolume);
            }
        }

        private float ActualCurrentVolume => IsMuted ? 0f : CurrentVolume * Manager.GlobalVolume;

        public float CurrentVolume
        {
            get => _currentVolume;
            set => UpdateVolume(value);
        }

        public int CurrentVolumeInt
        {
            get => (int)(_currentVolume * 100);
            set => UpdateVolume(value);
        }

        public void UpdateVolume(float volume)
        {
            _currentVolume = volume;

            // go through all sounds associated and update them with the new actual volume
            var instances = Manager.GetAudioInstancesForCategory(Name);
            for (var i = 0; i < instances.Count; i++)
            {
                instances[i].UpdateVolume(ActualCurrentVolume);
            }
        }

        public List<string> GetRegisteredAudio()
        {
            return new List<string>(_associatedAudio);
        }

        public void RegisterAudio(string name)
        {
            _associatedAudio.Add(name);
        }

        public void RegisterAudio(List<string> names)
        {
            for (var i = 0; i < _associatedAudio.Count; i++)
            {
                _associatedAudio.Add(names[i]);
            }
        }

        public bool DeRegisterAudio(string name)
        {
            return _associatedAudio.Remove(name);
        }

        public void DeRegisterAudio(List<string> names)
        {
            for (var i = 0; i < names.Count; i++)
            {
                _associatedAudio.Remove(names[i]);
            }
        }

        public void RegisterInstance(ulong id)
        {
            _registeredInstances.Add(id);
        }

        public List<ulong> GetInstanceIds()
        {
            return new List<ulong>(_registeredInstances);
        }

        public void UnRegisterInstance(ulong id)
        {
            _registeredInstances.Remove(id);
        }

        public void UpdateVolume(int volume)
        {
            UpdateVolume(volume / 100f);
        }

        public void AddTag(Tag tag)
        {
            _tags.Add(tag);
        }

        public void AddTag(List<Tag> tags)
        {
            for (var i = 0; i < tags.Count; i++)
            {
                _tags.Add(tags[i]);
            }
        }

        public List<Tag> GetTags()
        {
            return new List<Tag>(_tags);
        }

        public void RemoveTag(string name, TagRemovalStrategy removalStrategy = TagRemovalStrategy.RemoveOnlyFirst)
        {
            var indexesToRemove = new List<int>();
            for (var i = 0; i < _tags.Count; i++)
            {
                if (_tags[i].Name.Equals(name))
                {
                    indexesToRemove.Add(i);
                    if (removalStrategy == TagRemovalStrategy.RemoveOnlyFirst)
                    {
                        break;
                    }
                    else if (removalStrategy == TagRemovalStrategy.RemoveOnlyLast && indexesToRemove.Count > 1)
                    {
                        indexesToRemove.RemoveAt(0);
                    }
                }
            }

            for (var i = indexesToRemove.Count - 1; i >= 0; i--)
            {
                _tags.RemoveAt(indexesToRemove[i]);
            }
        }

        public void RemoveTag(int index)
        {
            _tags.RemoveAt(index);
        }

        public void RemoveTag(List<string> names, TagRemovalStrategy removalStrategy)
        {
            for (var i = 0; i < names.Count; i++)
            {
                RemoveTag(names[i], removalStrategy);
            }
        }

        public virtual void Update()
        {
            // go through all of our tags and generate new audio instances as we can
            var audio = Manager.GetAudioForCategory(Name);

            for (var i = 0; i < audio.Count; i++)
            {
                if (audio[i].IsValid(_tags, out var validTags))
                {
                    // consume each tag (if required)
                    for (var j = 0; j < validTags.Count; j++)
                    {
                        if (validTags[j].Consumable)
                        {
                            RemoveTag(validTags[j].Name, TagRemovalStrategy.RemoveOnlyFirst);
                        }
                    }

                    // create a new instance
                    var id = Manager.GenerateNewAudioInstance(audio[i].Name, Name);
                    RegisterInstance(id);
                }
            }
        }
    }
}
