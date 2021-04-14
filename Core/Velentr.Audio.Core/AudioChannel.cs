using System;
using System.Collections.Generic;
using System.Text;

namespace Velentr.Audio
{
    internal class AudioChannel
    {

        public AudioChannel(string name, bool useMusicInstances, float volume)
        {
            Name = name;
            UseMusicInstances = useMusicInstances;
            Volume = volume;
        }

        public string Name;

        public bool UseMusicInstances;

        public float Volume;

    }
}
