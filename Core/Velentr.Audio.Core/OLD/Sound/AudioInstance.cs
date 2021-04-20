using System;
using Microsoft.Xna.Framework.Audio;

namespace Velentr.Audio.OLD.Sound
{
    public class AudioInstance : IDisposable
    {

        internal AudioInstance()
        {

        }

        internal AudioInstance(AudioManager manager, ulong id, string name, string categoryName, SoundEffectInstance instance)
        {
            ID = id;
            Name = name;
            CategoryName = categoryName;
            Instance = instance;
            Manager = manager;
        }

        public AudioManager Manager { get; private set; }

        public ulong ID { get; private set; }

        public string Name { get; private set; }

        public string CategoryName { get; private set; }

        public SoundEffectInstance Instance;

        public SoundState State => Instance.State;

        internal void UpdateInstance(AudioManager manager, ulong id, string name, string categoryName, SoundEffectInstance instance)
        {
            ID = id;
            Name = name;
            CategoryName = categoryName;
            Instance?.Dispose();
            Instance = null;
            Instance = instance;
            Manager = manager;
        }

        public void Pause()
        {
            if (State == SoundState.Playing)
            {
                Instance.Pause();
            }
        }

        public void Stop(bool immediate = true)
        {
            if (State != SoundState.Stopped)
            {
                Instance.Stop(immediate);
            }
        }

        public void Play(bool resumeIfPaused = true)
        {
            if (resumeIfPaused)
            {
                switch (State)
                {
                    case SoundState.Paused:
                        Instance.Resume();
                        break;
                    case SoundState.Stopped:
                        Instance.Play();
                        break;
                    default:
                        throw new Exception("Audio is already playing!");
                }
            }
            else
            {
                if (State == SoundState.Paused)
                {
                    Instance.Stop(true);
                }

                Instance.Play();
            }
        }

        public void Resume()
        {
            Instance.Resume();
        }

        public void Dispose()
        {
            Instance?.Dispose();
        }

        public void UpdateVolume(float volume)
        {
            Instance.Volume = volume;
        }

        public void UpdateVolume(int volume)
        {
            UpdateVolume(volume / 100f);
        }
    }
}
