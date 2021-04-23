using Microsoft.Xna.Framework.Audio;
using System;
using System.IO;

namespace Velentr.Audio.Sounds
{
    /// <summary>
    ///     An audio.
    /// </summary>
    ///
    /// <seealso cref="IDisposable"/>
    public abstract class Audio : IDisposable
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        ///
        /// <param name="manager">                  The manager. </param>
        /// <param name="name">                     The name. </param>
        /// <param name="path">                     Full pathname of the file. </param>
        /// <param name="defaultPitch">             (Optional) The default pitch. </param>
        /// <param name="autoLoad">                 (Optional) True to automatically load. </param>
        protected Audio(AudioManager manager, string name, string path, float defaultPitch = 0f, bool autoLoad = false)
        {
            if (System.IO.Path.GetExtension(path) != ".wav")
            {
                throw new Exception("Only .wav files are supported!");
            }

            Name = name;
            Path = path;
            DefaultPitch = defaultPitch;
            Loaded = false;
            Manager = manager;

            if (autoLoad)
            {
                Load();
            }
        }

        /// <summary>
        ///     Gets or sets the default pitch.
        /// </summary>
        ///
        /// <value>
        ///     The default pitch.
        /// </value>
        public float DefaultPitch { get; set; }

        /// <summary>
        ///     Gets or sets the default pitch int.
        /// </summary>
        ///
        /// <value>
        ///     The default pitch int.
        /// </value>
        public int DefaultPitchInt
        {
            get => (int)(DefaultPitch * 100);
            set => DefaultPitch = value / 100f;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the loaded.
        /// </summary>
        ///
        /// <value>
        ///     True if loaded, false if not.
        /// </value>
        public bool Loaded { get; private set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the only to create instance should be loaded.
        /// </summary>
        ///
        /// <value>
        ///     True if load only to create instance, false if not.
        /// </value>
        public bool LoadOnlyToCreateInstance { get; set; }

        /// <summary>
        ///     Gets the manager.
        /// </summary>
        ///
        /// <value>
        ///     The manager.
        /// </value>
        public AudioManager Manager { get; }

        /// <summary>
        ///     Gets the name.
        /// </summary>
        ///
        /// <value>
        ///     The name.
        /// </value>
        public string Name { get; }

        /// <summary>
        ///     Gets the full pathname of the file.
        /// </summary>
        ///
        /// <value>
        ///     The full pathname of the file.
        /// </value>
        public string Path { get; }

        /// <summary>
        ///     Gets or sets the sound.
        /// </summary>
        ///
        /// <value>
        ///     The sound.
        /// </value>
        public SoundEffect Sound { get; private set; }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting
        ///     unmanaged resources.
        /// </summary>
        ///
        /// <seealso cref="IDisposable.Dispose()"/>
        public void Dispose()
        {
            Sound?.Dispose();
            Loaded = false;
        }

        /// <summary>
        ///     Gets the instance.
        /// </summary>
        ///
        /// <exception cref="Exception"> Thrown when an exception error condition occurs. </exception>
        ///
        /// <returns>
        ///     The instance.
        /// </returns>
        public SoundEffectInstance GetInstance()
        {
            SoundEffectInstance instance = null;
            if (!Loaded)
            {
                using (var stream = File.OpenRead(Path))
                {
                    Sound = SoundEffect.FromStream(stream);
                    instance = Sound.CreateInstance();
                }
                Loaded = true;
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

        /// <summary>
        ///     Loads this object.
        /// </summary>
        ///
        /// <exception cref="Exception"> Thrown when an exception error condition occurs. </exception>
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
    }
}