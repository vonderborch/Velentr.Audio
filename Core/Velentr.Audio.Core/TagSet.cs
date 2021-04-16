using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Velentr.Audio
{
    /// <summary>
    ///     A tag set.
    /// </summary>
    public struct TagSet
    {
        /// <summary>
        ///     The exclusions.
        /// </summary>
        private HashSet<string> _exclusions;

        /// <summary>
        ///     The required.
        /// </summary>
        private HashSet<string> _required;

        /// <summary>
        ///     The tags.
        /// </summary>
        private HashSet<string> _tags;

        /// <summary>
        ///     Constructor.
        /// </summary>
        ///
        /// <param name="tags">
        ///     (Optional) The tags. These are effectively 'Any' tags. If any exist in the current set of
        ///     tags associated with the manager, this music is valid.
        /// </param>
        /// <param name="exclusionTags">
        ///     (Optional)
        ///     The exclusion tags. These are the opposite of 'Any' tags. If any exists in the current
        ///     set of tags associated with the manager, this music is _not_ valid.
        /// </param>
        /// <param name="requiredTags">
        ///     (Optional) The required tags. These are effectively 'All' tags. All of these tags need to
        ///     exist in the current set of tags associated with the manager for this music to be valid.
        /// </param>
        public TagSet(List<string> tags = null, List<string> exclusionTags = null, List<string> requiredTags = null)
        {
            _tags = new HashSet<string>(tags ?? new List<string>());
            _exclusions = new HashSet<string>(exclusionTags ?? new List<string>());
            _required = new HashSet<string>(requiredTags ?? new List<string>());

            TotalTagCount = _tags.Count + _exclusions.Count + _required.Count;
        }

        /// <summary>
        ///     Gets the number of exclusion tags.
        /// </summary>
        ///
        /// <value>
        ///     The number of exclusion tags.
        /// </value>
        public int ExclusionTagCount => _exclusions.Count;

        /// <summary>
        ///     Gets the number of required tags.
        /// </summary>
        ///
        /// <value>
        ///     The number of required tags.
        /// </value>
        public int RequiredTagCount => _required.Count;

        /// <summary>
        ///     Gets the number of tags.
        /// </summary>
        ///
        /// <value>
        ///     The number of tags.
        /// </value>
        public int TagCount => _tags.Count;

        /// <summary>
        ///     Gets or sets the number of total tags.
        /// </summary>
        ///
        /// <value>
        ///     The total number of tag count.
        /// </value>
        public int TotalTagCount { get; private set; }


        /// <summary>
        ///     Gets or sets the exclusion tags.
        /// </summary>
        ///
        /// <value>
        ///     The exclusion tags.
        /// </value>
        public List<string> ExclusionTags
        {
            get => _exclusions.ToList();
            set
            {
                _exclusions = new HashSet<string>(value);
                TotalTagCount = _tags.Count + _exclusions.Count + _required.Count;
            }
        }

        /// <summary>
        ///     Gets or sets the tags.
        /// </summary>
        ///
        /// <value>
        ///     The tags.
        /// </value>
        public List<string> Tags
        {
            get => _tags.ToList();
            set
            {
                _tags = new HashSet<string>(value);
                TotalTagCount = _tags.Count + _exclusions.Count + _required.Count;
            }
        }

        /// <summary>
        ///     Gets or sets the required tags.
        /// </summary>
        ///
        /// <value>
        ///     The required tags.
        /// </value>
        public List<string> RequiredTags
        {
            get => _required.ToList();
            set
            {
                _required = new HashSet<string>(value);
                TotalTagCount = _tags.Count + _exclusions.Count + _required.Count;
            }
        }

        /// <summary>
        ///     Adds a tag.
        /// </summary>
        ///
        /// <param name="tag"> The tag. </param>
        public void AddTag(string tag)
        {
            _tags.Add(tag);
            TotalTagCount++;
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

            TotalTagCount += tags.Count;
        }

        /// <summary>
        ///     Adds an exclusion tag.
        /// </summary>
        ///
        /// <param name="tag"> The tag. </param>
        public void AddExclusionTag(string tag)
        {
            _exclusions.Add(tag);
            TotalTagCount++;
        }

        /// <summary>
        ///     Adds an exclusion tags.
        /// </summary>
        ///
        /// <param name="tags"> The tags. </param>
        public void AddExclusionTags(List<string> tags)
        {
            for (var i = 0; i < tags.Count; i++)
            {
                _exclusions.Add(tags[i]);
            }

            TotalTagCount += tags.Count;
        }

        /// <summary>
        ///     Adds a required tag.
        /// </summary>
        ///
        /// <param name="tag"> The tag. </param>
        public void AddRequiredTag(string tag)
        {
            _required.Add(tag);
            TotalTagCount++;
        }

        /// <summary>
        ///     Adds a required tags.
        /// </summary>
        ///
        /// <param name="tags"> The tags. </param>
        public void AddRequiredTags(List<string> tags)
        {
            for (var i = 0; i < tags.Count; i++)
            {
                _required.Add(tags[i]);
            }

            TotalTagCount += tags.Count;
        }

        /// <summary>
        ///     Determine if the music is valid to be played based on the current tags.
        /// </summary>
        /// <param name="currentTags"> The current tags. </param>
        /// <returns>
        ///     True if valid, false if not.
        /// </returns>
        public bool IsValid(List<string> currentTags)
        {
            if (TotalTagCount == 0 && currentTags.Count == 0)
            {
                return true;
            }

            var requiredTags = 0;
            var tags = 0;
            for (var i = 0; i < currentTags.Count; i++)
            {
                // if we run into a tag this piece of music is excluded from being played when it is there, return false
                if (_exclusions.Contains(currentTags[i]))
                {
                    return false;
                }

                if (_required.Contains(currentTags[i]))
                {
                    requiredTags++;
                }

                if (_tags.Contains(currentTags[i]))
                {
                    tags++;
                }
            }

            // return true if
            //   we have at least one required tag and all required tags are in the list of current tags
            //   at least one of our normal tags exists
            return RequiredTagCount > 0 && requiredTags == RequiredTagCount || tags > 0;
        }

        public bool IsValid(List<string> currentTags, out List<string> validTags)
        {
            if (TotalTagCount == 0 && currentTags.Count == 0)
            {
                validTags = new List<string>();
                return true;
            }

            var tags = 0;
            var requiredTags = 0;
            var valid = new List<string>();
            for (var i = 0; i < currentTags.Count; i++)
            {
                // if we run into a tag this piece of music is excluded from being played when it is there, return false
                if (_exclusions.Contains(currentTags[i]))
                {
                    validTags = new List<string>();
                    return false;
                }

                if (_required.Contains(currentTags[i]))
                {
                    requiredTags++;
                    valid.Add(currentTags[i]);
                }

                if (_tags.Contains(currentTags[i]))
                {
                    tags++;
                    valid.Add(currentTags[i]);
                }
            }

            // return true if
            //   we have at least one required tag and all required tags are in the list of current tags
            //   at least one of our normal tags exists
            var isValid = RequiredTagCount > 0 && requiredTags == RequiredTagCount || tags > 0;
            validTags = isValid
                ? valid
                : new List<string>();
            return isValid;
        }

        /// <summary>
        ///     Gets valid tags.
        /// </summary>
        ///
        /// <param name="currentTags"> The current tags. </param>
        ///
        /// <returns>
        ///     The valid tags.
        /// </returns>
        public List<string> GetValidTags(List<string> currentTags)
        {
            if (TotalTagCount == 0 && currentTags.Count == 0)
            {
                return currentTags;
            }

            var valid = new List<string>();
            var requiredTags = 0;
            var tags = 0;
            for (var i = 0; i < currentTags.Count; i++)
            {
                // if we run into a tag this piece of music is excluded from being played when it is there, return false
                if (_exclusions.Contains(currentTags[i]))
                {
                    return new List<string>();
                }

                if (_required.Contains(currentTags[i]))
                {
                    requiredTags++;
                    valid.Add(currentTags[i]);
                }

                if (_tags.Contains(currentTags[i]))
                {
                    tags++;
                    valid.Add(currentTags[i]);
                }
            }
            return RequiredTagCount > 0 && requiredTags == RequiredTagCount
                ? valid
                : tags > 0
                    ? valid
                    : new List<string>();
        }
    }
}
