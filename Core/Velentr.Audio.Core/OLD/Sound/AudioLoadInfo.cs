using System.Collections.Generic;

namespace Velentr.Audio.OLD.Sound
{
    public struct AudioLoadInfo
    {

        public AudioLoadInfo(string name, string path, float defaultPitch = 0f, bool autoLoad = false, bool loadOnlyToCreateInstance = false, List<string> tags = null, List<string> exclusionTags = null, List<string> requiredTags = null)
        {
            Name = name;
            Path = path;
            DefaultPitch = defaultPitch;
            AutoLoad = autoLoad;
            LoadOnlyToCreateInstance = loadOnlyToCreateInstance;
            Tags = tags == null ? new List<string>() : new List<string>(tags);
            ExclusionTags = exclusionTags == null ? new List<string>() : new List<string>(exclusionTags);
            RequiredTags = requiredTags == null ? new List<string>() : new List<string>(requiredTags);
        }

        public string Name { get; }

        public string Path { get; }

        public float DefaultPitch { get; }

        public bool AutoLoad { get; }

        public bool LoadOnlyToCreateInstance { get; }

        public List<string> Tags { get; }

        public List<string> ExclusionTags { get; }

        public List<string> RequiredTags { get; }
    }
}
