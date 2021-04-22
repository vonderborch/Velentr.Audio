using Microsoft.Xna.Framework.Audio;
using System;

namespace Velentr.Audio.Sounds
{
    /// <summary>
    ///     An audio instance.
    /// </summary>
    public class AudioInstance
    {
        /// <summary>
        ///     The instance.
        /// </summary>
        public SoundEffectInstance Instance;

        /// <summary>
        ///     Default constructor.
        /// </summary>
        public AudioInstance()
        {
        }

        /// <summary>
        ///     Gets or sets the name of the category.
        /// </summary>
        ///
        /// <value>
        ///     The name of the category.
        /// </value>
        public string CategoryName { get; private set; }

        /// <summary>
        ///     Gets or sets the identifier.
        /// </summary>
        ///
        /// <value>
        ///     The identifier.
        /// </value>
        public ulong ID { get; private set; }

        /// <summary>
        ///     Gets or sets the manager.
        /// </summary>
        ///
        /// <value>
        ///     The manager.
        /// </value>
        public AudioManager Manager { get; private set; }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        ///
        /// <value>
        ///     The name.
        /// </value>
        public string Name { get; private set; }

        /// <summary>
        ///     Gets the state.
        /// </summary>
        ///
        /// <value>
        ///     The state.
        /// </value>
        public SoundState State => Instance.State;

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting
        ///     unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Instance?.Dispose();
        }

        /// <summary>
        ///     Pauses this object.
        /// </summary>
        public void Pause()
        {
            if (State == SoundState.Playing)
            {
                Instance.Pause();
            }
        }

        /// <summary>
        ///     Plays the given resume if paused.
        /// </summary>
        ///
        /// <exception cref="Exception"> Thrown when an exception error condition occurs. </exception>
        ///
        /// <param name="resumeIfPaused"> (Optional) True if resume if paused. </param>
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

        /// <summary>
        ///     Resumes this object.
        /// </summary>
        public void Resume()
        {
            Instance.Resume();
        }

        /// <summary>
        ///     Stops the given immediate.
        /// </summary>
        ///
        /// <param name="immediate"> (Optional) True to immediate. </param>
        public void Stop(bool immediate = true)
        {
            if (State != SoundState.Stopped)
            {
                Instance.Stop(immediate);
            }
        }

        /// <summary>
        ///     Updates the volume described by volume.
        /// </summary>
        ///
        /// <param name="volume"> The volume. </param>
        public void UpdateVolume(float volume)
        {
            Instance.Volume = volume;
        }

        /// <summary>
        ///     Updates the volume described by volume.
        /// </summary>
        ///
        /// <param name="volume"> The volume. </param>
        public void UpdateVolume(int volume)
        {
            UpdateVolume(volume / 100f);
        }

        /// <summary>
        ///     Updates the instance.
        /// </summary>
        ///
        /// <param name="manager">      The manager. </param>
        /// <param name="id">           The identifier. </param>
        /// <param name="name">         The name. </param>
        /// <param name="categoryName"> The name of the category. </param>
        /// <param name="instance">     The instance. </param>
        internal void UpdateInstance(AudioManager manager, ulong id, string name, string categoryName, SoundEffectInstance instance)
        {
            Manager = manager;
            ID = id;
            Name = name;
            CategoryName = categoryName;
            Instance?.Dispose();
            Instance = null;
            Instance = instance;
        }
    }
}