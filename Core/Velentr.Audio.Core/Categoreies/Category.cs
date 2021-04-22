using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using Velentr.Audio.Helpers;
using Velentr.Audio.Sounds;
using Velentr.Audio.Tagging;
using IUpdateable = Velentr.Audio.Helpers.IUpdateable;

namespace Velentr.Audio.Categoreies
{
    /// <summary>
    ///     An audio category.
    /// </summary>
    ///
    /// <seealso cref="IUpdateable"/>
    public class Category : IUpdateable
    {
        /// <summary>
        ///     The current tags.
        /// </summary>
        protected List<Tag> _currentTags;

        /// <summary>
        ///     The registered instances.
        /// </summary>
        protected List<ulong> _registeredInstances;

        /// <summary>
        ///     The current volume.
        /// </summary>
        private float _currentVolume;

        /// <summary>
        ///     True if is muted, false if not.
        /// </summary>
        private bool _isMuted;

        /// <summary>
        ///     Constructor.
        /// </summary>
        ///
        /// <param name="manager">         The manager. </param>
        /// <param name="name">            The name. </param>
        /// <param name="isMusicCategory"> (Optional) True if is music category, false if not. </param>
        /// <param name="volume">          (Optional) The volume. </param>
        internal Category(AudioManager manager, string name, bool isMusicCategory = false, float volume = 1.0f)
        {
            Manager = manager;
            Name = name;
            IsMusicCategory = isMusicCategory;
            _currentVolume = volume;

            _currentTags = new List<Tag>();
            _registeredInstances = new List<ulong>();
        }

        /// <summary>
        ///     Gets or sets the current volume.
        /// </summary>
        ///
        /// <value>
        ///     The current volume.
        /// </value>
        public float CurrentVolume
        {
            get => _currentVolume;
            set => UpdateVolume(value);
        }

        /// <summary>
        ///     Gets or sets the current volume int.
        /// </summary>
        ///
        /// <value>
        ///     The current volume int.
        /// </value>
        public int CurrentVolumeInt
        {
            get => (int)(_currentVolume * 100);
            set => UpdateVolume(value);
        }

        /// <summary>
        ///     Gets a value indicating whether this object is music category.
        /// </summary>
        ///
        /// <value>
        ///     True if this object is music category, false if not.
        /// </value>
        public bool IsMusicCategory { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether this object is muted.
        /// </summary>
        ///
        /// <value>
        ///     True if this object is muted, false if not.
        /// </value>
        public bool IsMuted
        {
            get => _isMuted;
            set
            {
                _isMuted = value;
                UpdateVolume(CurrentVolume);
            }
        }

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
        ///     Gets the actual current volume.
        /// </summary>
        ///
        /// <value>
        ///     The actual current volume.
        /// </value>
        private float ActualCurrentVolume => IsMuted ? 0f : CurrentVolume * Manager.GlobalVolume;

        /// <summary>
        ///     Adds a tag.
        /// </summary>
        ///
        /// <param name="tag"> The tag. </param>
        public void AddTag(Tag tag)
        {
            _currentTags.Add(tag);
        }

        /// <summary>
        ///     Adds a tag.
        /// </summary>
        ///
        /// <param name="tags"> The tags. </param>
        public void AddTag(List<Tag> tags)
        {
            _currentTags.AddRange(tags);
        }

        /// <summary>
        ///     Adds a tag.
        /// </summary>
        ///
        /// <param name="name">                 The name. </param>
        /// <param name="consumable">           True if consumable. </param>
        /// <param name="lifespanMilliseconds"> The lifespan in milliseconds. </param>
        public void AddTag(string name, bool consumable, uint lifespanMilliseconds)
        {
            _currentTags.Add(new Tag(name, consumable, lifespanMilliseconds));
        }

        /// <summary>
        ///     Gets current tags.
        /// </summary>
        ///
        /// <returns>
        ///     The current tags.
        /// </returns>
        public List<Tag> GetCurrentTags()
        {
            return new List<Tag>(_currentTags);
        }

        /// <summary>
        ///     Gets instance identifiers.
        /// </summary>
        ///
        /// <returns>
        ///     The instance identifiers.
        /// </returns>
        public List<ulong> GetInstanceIds()
        {
            return new List<ulong>(_registeredInstances);
        }

        /// <summary>
        ///     Registers the instance described by ID.
        /// </summary>
        ///
        /// <param name="id"> The identifier. </param>
        public virtual void RegisterInstance(ulong id)
        {
            _registeredInstances.Add(id);
        }

        /// <summary>
        ///     Removes the tag described by tags.
        /// </summary>
        ///
        /// <param name="name">                    The name. </param>
        /// <param name="removeOnlyFirstinstance"> (Optional) True to remove only firstinstance. </param>
        ///
        /// <returns>
        ///     A List&lt;bool&gt;
        /// </returns>
        public bool RemoveTag(string name, bool removeOnlyFirstinstance = true)
        {
            var indexesToRemove = new List<int>();
            for (var i = 0; i < _currentTags.Count; i++)
            {
                if (_currentTags[i].Name.Equals(name))
                {
                    indexesToRemove.Add(i);
                    if (removeOnlyFirstinstance)
                    {
                        return true;
                    }
                }
            }

            for (var i = indexesToRemove.Count - 1; i >= 0; i--)
            {
                _currentTags.RemoveAt(indexesToRemove[i]);
            }

            return indexesToRemove.Count != 0;
        }

        /// <summary>
        ///     Removes the tag described by tags.
        /// </summary>
        ///
        /// <param name="names">                   The names. </param>
        /// <param name="removeOnlyFirstinstance"> (Optional) True to remove only firstinstance. </param>
        ///
        /// <returns>
        ///     A List&lt;bool&gt;
        /// </returns>
        public List<bool> RemoveTag(List<string> names, bool removeOnlyFirstinstance = true)
        {
            var output = new List<bool>();
            for (var i = 0; i < names.Count; i++)
            {
                output.Add(RemoveTag(names[i], removeOnlyFirstinstance));
            }

            return output;
        }

        /// <summary>
        ///     Removes the tag described by tags.
        /// </summary>
        ///
        /// <param name="tags"> The tags. </param>
        ///
        /// <returns>
        ///     A List&lt;bool&gt;
        /// </returns>
        public List<bool> RemoveTag(List<(string, bool)> tags)
        {
            var output = new List<bool>();
            for (var i = 0; i < tags.Count; i++)
            {
                output.Add(RemoveTag(tags[i].Item1, tags[i].Item2));
            }

            return output;
        }

        /// <summary>
        ///     Un register instance.
        /// </summary>
        ///
        /// <param name="id"> The identifier. </param>
        public virtual void UnRegisterInstance(ulong id)
        {
            _registeredInstances.Remove(id);
        }

        /// <summary>
        ///     Un register instance.
        /// </summary>
        ///
        /// <param name="ids"> The identifiers. </param>
        public virtual void UnRegisterInstance(List<ulong> ids)
        {
            for (var i = 0; i < ids.Count; i++)
            {
                _registeredInstances.Remove(ids[i]);
            }
        }

        /// <summary>
        ///     Updates the given gameTime.
        /// </summary>
        ///
        /// <param name="gameTime"> The game time. </param>
        public virtual void Update(GameTime gameTime)
        {
            // clean up existing sounds as possible
            var instances = Manager.GetAudioInstances(_registeredInstances, IsMusicCategory);
            var instancesToKill = new List<ulong>();
            for (var i = 0; i < instances.Count; i++)
            {
                switch (instances[i].State)
                {
                    case SoundState.Stopped:
                        instancesToKill.Add(instances[i].ID);
                        break;
                }
            }
            Manager.RemoveAudioInstances(instancesToKill, Name);
            UnRegisterInstance(instancesToKill);

            // create new sounds
            var audio = Manager.GetAssociatedAudio(Name);
            for (var i = 0; i < audio.Count; i++)
            {
                if (((Sound)audio[i]).Tags.IsValid(_currentTags, out var validTags))
                {
                    // consume each tag as required
                    for (var j = 0; j < validTags.Count; j++)
                    {
                        if (validTags[j].Consumable)
                        {
                            _currentTags.Remove(validTags[j]);
                        }
                    }

                    // create a new instance
                    var newId = Manager.GenerateNewAudioInstance(audio[i].Name, Name, true);
                    RegisterInstance(newId);
                }
            }

            // go through all tags and clean them up as required
            var tagsToRemove = new List<int>();
            for (var i = 0; i < _currentTags.Count; i++)
            {
                if (TimeHelpers.ElapsedMilliSeconds(_currentTags[i].CreationTime, gameTime) > _currentTags[i].LifespanMilliseconds)
                {
                    tagsToRemove.Add(i);
                }
            }
            for (var i = tagsToRemove.Count - 1; i >= 0; i--)
            {
                _currentTags.RemoveAt(tagsToRemove[i]);
            }
        }

        /// <summary>
        ///     Updates the volume described by volume.
        /// </summary>
        ///
        /// <param name="volume"> The volume. </param>
        public void UpdateVolume(float volume)
        {
            _currentVolume = volume;

            // go through all sounds associated and update them with the new actual volume
            var instances = Manager.GetAudioInstances(_registeredInstances, IsMusicCategory);
            for (var i = 0; i < instances.Count; i++)
            {
                instances[i].UpdateVolume(ActualCurrentVolume);
            }
        }
    }
}