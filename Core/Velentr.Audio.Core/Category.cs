using System.Collections.Generic;
using System.Linq;

namespace Velentr.Audio
{
    /// <summary>
    ///     A category.
    /// </summary>
    public class Category
    {
        /// <summary>
        ///     The current volume.
        /// </summary>
        private float _currentVolume = 1.0f;

        /// <summary>
        ///     The tags.
        /// </summary>
        private HashSet<string> _tags;


        public Category(string name, bool isMusicCategory = false, float volume = 1.0f, FullInstancesAction actionWhenNoFreeInstances = FullInstancesAction.ThrowException)
        {
            Name = name;
            IsMusicCategory = isMusicCategory;
            ConsumeValidTagsWhenCreatingInstance = !isMusicCategory;
            _currentVolume = volume;
            _tags = new HashSet<string>();
            ActionWhenNoFreeInstances = actionWhenNoFreeInstances;
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
        ///     Gets the name.
        /// </summary>
        ///
        /// <value>
        ///     The name.
        /// </value>
        public string Name { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether the consume valid tags when creating instance.
        /// </summary>
        ///
        /// <value>
        ///     True if consume valid tags when creating instance, false if not.
        /// </value>
        public bool ConsumeValidTagsWhenCreatingInstance { get; set; }

        /// <summary>
        ///     Gets or sets the action when no free instances.
        /// </summary>
        ///
        /// <value>
        ///     The action when no free instances.
        /// </value>
        public FullInstancesAction ActionWhenNoFreeInstances { get; set; }

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
            set => ChangeVolume(value);
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
            get => (int) (_currentVolume * 100);
            set => ChangeVolume(value);
        }

        /// <summary>
        ///     Change volume.
        /// </summary>
        ///
        /// <param name="newVolume"> The new volume. </param>
        public void ChangeVolume(float newVolume)
        {
            _currentVolume = newVolume;
            // go through all associated sounds and update them to the new volume

        }

        /// <summary>
        ///     Change volume.
        /// </summary>
        ///
        /// <param name="newVolume"> The new volume. </param>
        public void ChangeVolume(int newVolume)
        {
            ChangeVolume(newVolume / 100f);
        }

        /// <summary>
        ///     Adds a tag.
        /// </summary>
        ///
        /// <param name="tag"> The tag. </param>
        public void AddTag(string tag)
        {
            _tags.Add(tag);
        }

        /// <summary>
        ///     Adds the tags.
        /// </summary>
        ///
        /// <param name="tags"> The tags. </param>
        public void AddTags(List<string> tags)
        {
            for (var i = 0; i < tags.Count; i++)
            {
                _tags.Add(tags[i]);
            }
        }

        /// <summary>
        ///     Removes the tag described by tag.
        /// </summary>
        ///
        /// <param name="tag"> The tag. </param>
        ///
        /// <returns>
        ///     True if it succeeds, false if it fails.
        /// </returns>
        public bool RemoveTag(string tag)
        {
            return _tags.Remove(tag);
        }

        /// <summary>
        ///     Removes the tags described by tags.
        /// </summary>
        ///
        /// <param name="tags"> The tags. </param>
        public void RemoveTags(List<string> tags)
        {
            for (var i = 0; i < tags.Count; i++)
            {
                _tags.Remove(tags[i]);
            }
        }

        public List<string> GetTags()
        {
            return _tags.ToList();
        }
    }
}
