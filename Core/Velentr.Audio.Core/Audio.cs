using Microsoft.Xna.Framework.Audio;

namespace Velentr.Audio
{
    public abstract class Audio
    {

        protected Audio(string name, string path, AudioManager manager)
        {
            Name = name;
            Path = path;
            Manager = manager;
        }

        public string Name { get; protected set; }

        public string Path { get; protected set; }

        public AudioManager Manager { get; }

        public SoundEffect Sound { get; private set; }

        public float DefaultPitch { get; set; } = 0f;

        public int DefaultPitchInt
        {
            get => (int)(DefaultPitch * 100);
            set => DefaultPitch = value / 100f;
        }

        public SoundEffectInstance GetInstance()
        {

        }

    }
}
