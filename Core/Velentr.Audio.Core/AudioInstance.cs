using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Audio;

namespace Velentr.Audio
{
    public class AudioInstance : IDisposable
    {

        internal AudioInstance(ulong id, string name, string categoryName, SoundEffectInstance instance, bool isMusicInstance = false)
        {
            ID = id;
            Name = name;
            CategoryName = categoryName;
            SoundInstance = instance;
        }

        public ulong ID { get; }

        public string Name { get; }

        public string CategoryName { get; }

        public SoundEffectInstance SoundInstance;

        public SoundState State => SoundInstance.State;

        public void Pause()
        {
            if (State == SoundState.Playing)
            {
                SoundInstance.Pause();
            }
        }

        public void Stop()
        {
            if (State != SoundState.Stopped)
            {
                SoundInstance.Stop();
            }
        }

        public void Play(bool resumeIfPaused)
        {
            if (resumeIfPaused)
            {
                switch (State)
                {
                    case SoundState.Paused:
                        SoundInstance.Resume();
                        break;
                    case SoundState.Stopped:
                        SoundInstance.Play();
                        break;
                    default:
                        throw new Exception("Audio is already playing!");
                }
            }
            else
            {
                if (State == SoundState.Paused)
                {
                    SoundInstance.Stop(true);
                }

                SoundInstance.Play();
            }
        }

        public void Dispose()
        {
            SoundInstance?.Dispose();
        }

        public void UpdateVolume(float volume)
        {
            SoundInstance.Volume = volume;
        }

    }
}
