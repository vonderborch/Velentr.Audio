using System;
using System.Collections.Generic;
using System.Text;

namespace Velentr.Audio
{
    public struct AudioLoadInfo
    {
        public AudioLoadInfo(string name, string categoryName, string path, float defaultPitch = 0f, bool autoLoad = false, bool loadOnlyToCreateInstance = false, List<string> tags = null, List<string> exclusionTags = null, List<string> requiredTags = null)
        {
            Name = name;
            CategoryName = categoryName;
            Path = path;
            DefaultPitch = defaultPitch;
            LoadOnlyToCreateInstance = loadOnlyToCreateInstance;
            Tags = tags;
            ExclusionTags = exclusionTags;
            RequiredTags = requiredTags;
            AutoLoad = autoLoad;
        }


        public string Name { get; set; }

        public string CategoryName { get; set; }

        public string Path { get; set; }

        public float DefaultPitch { get; set; }

        public bool LoadOnlyToCreateInstance { get; set; }

        public bool AutoLoad { get; set; }

        public List<string> Tags { get; set; }

        public List<string> ExclusionTags { get; set; }

        public List<string> RequiredTags { get; set; }
    }
}
