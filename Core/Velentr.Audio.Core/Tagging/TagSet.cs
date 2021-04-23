using System;
using System.Collections.Generic;
using System.Linq;

namespace Velentr.Audio.Tagging
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
        /// <param name="fake"> (Optional) True to fake. </param>
        public TagSet(bool fake = false)
        {
            _tags = new HashSet<string>();
            _exclusions = new HashSet<string>();
            _required = new HashSet<string>();

            TotalTagCount = _tags.Count + _exclusions.Count + _required.Count;
        }

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
        public TagSet(List<string> tags, List<string> exclusionTags, List<string> requiredTags)
        {
            _tags = tags == null
                ? new HashSet<string>()
                : new HashSet<string>(tags);
            _exclusions = exclusionTags == null
                ? new HashSet<string>()
                : new HashSet<string>(tags);
            _required = requiredTags == null
                ? new HashSet<string>()
                : new HashSet<string>(tags);

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
        ///     Gets the number of required tags.
        /// </summary>
        ///
        /// <value>
        ///     The number of required tags.
        /// </value>
        public int RequiredTagCount => _required.Count;

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
        ///     Gets the number of tags.
        /// </summary>
        ///
        /// <value>
        ///     The number of tags.
        /// </value>
        public int TagCount => _tags.Count;

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
        ///     Gets or sets the number of total tags.
        /// </summary>
        ///
        /// <value>
        ///     The total number of tag count.
        /// </value>
        public int TotalTagCount { get; private set; }

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
        public void AddExclusionTag(List<string> tags)
        {
            for (var i = 0; i < tags.Count; i++)
            {
                _exclusions.Add(tags[i]);
            }

            TotalTagCount += tags.Count;
        }

        /// <summary>
        ///     Adds a tag.
        /// </summary>
        ///
        /// <param name="tag"> The tag. </param>
        public void AddNormalTag(string tag)
        {
            _tags.Add(tag);
            TotalTagCount++;
        }

        /// <summary>
        ///     Adds the tags.
        /// </summary>
        ///
        /// <param name="tags"> The tags. </param>
        public void AddNormalTag(List<string> tags)
        {
            for (var i = 0; i < tags.Count; i++)
            {
                _tags.Add(tags[i]);
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
        public void AddRequiredTag(List<string> tags)
        {
            for (var i = 0; i < tags.Count; i++)
            {
                _required.Add(tags[i]);
            }

            TotalTagCount += tags.Count;
        }

        /// <summary>
        ///     Adds a tag.
        /// </summary>
        ///
        /// <exception cref="Exception"> Thrown when an exception error condition occurs. </exception>
        ///
        /// <param name="tag">     The tag. </param>
        /// <param name="tagType"> Type of the tag. </param>
        public void AddTag(string tag, TagType tagType)
        {
            switch (tagType)
            {
                case TagType.Normal:
                    AddNormalTag(tag);
                    break;

                case TagType.Exclusion:
                    AddExclusionTag(tag);
                    break;

                case TagType.Required:
                    AddRequiredTag(tag);
                    break;

                default:
                    throw new Exception("Unsupported tag type!");
            }
        }

        /// <summary>
        ///     Adds a tag.
        /// </summary>
        ///
        /// <exception cref="Exception"> Thrown when an exception error condition occurs. </exception>
        ///
        /// <param name="tags">    The tags. </param>
        /// <param name="tagType"> Type of the tag. </param>
        public void AddTag(List<string> tags, TagType tagType)
        {
            switch (tagType)
            {
                case TagType.Normal:
                    AddNormalTag(tags);
                    break;

                case TagType.Exclusion:
                    AddExclusionTag(tags);
                    break;

                case TagType.Required:
                    AddRequiredTag(tags);
                    break;

                default:
                    throw new Exception("Unsupported tag type!");
            }
        }

        /// <summary>
        ///     Adds a tag.
        /// </summary>
        ///
        /// <exception cref="Exception"> Thrown when an exception error condition occurs. </exception>
        ///
        /// <param name="tags"> The tags. </param>
        public void AddTag(List<(string, TagType)> tags)
        {
            for (var i = 0; i < tags.Count; i++)
            {
                switch (tags[i].Item2)
                {
                    case TagType.Normal:
                        AddNormalTag(tags[i].Item1);
                        break;

                    case TagType.Exclusion:
                        AddExclusionTag(tags[i].Item1);
                        break;

                    case TagType.Required:
                        AddRequiredTag(tags[i].Item1);
                        break;

                    default:
                        throw new Exception("Unsupported tag type!");
                }
            }
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
        public List<Tag> GetValidTags(List<Tag> currentTags)
        {
            if (TotalTagCount == 0 && currentTags.Count == 0)
            {
                return currentTags;
            }

            var valid = new List<Tag>();
            var requiredTags = 0;
            var tags = 0;
            for (var i = 0; i < currentTags.Count; i++)
            {
                // if we run into a tag this piece of music is excluded from being played when it is there, return false
                if (_exclusions.Contains(currentTags[i].Name))
                {
                    return new List<Tag>();
                }

                if (_required.Contains(currentTags[i].Name))
                {
                    requiredTags++;
                    valid.Add(currentTags[i]);
                }

                if (_tags.Contains(currentTags[i].Name))
                {
                    tags++;
                    valid.Add(currentTags[i]);
                }
            }
            return RequiredTagCount > 0 && requiredTags == RequiredTagCount
                ? valid
                : tags > 0
                    ? valid
                    : new List<Tag>();
        }

        /// <summary>
        ///     Determine if the music is valid to be played based on the current tags.
        /// </summary>
        /// <param name="currentTags"> The current tags. </param>
        /// <returns>
        ///     True if valid, false if not.
        /// </returns>
        public bool IsValid(List<Tag> currentTags)
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
                if (_exclusions.Contains(currentTags[i].Name))
                {
                    return false;
                }

                if (_required.Contains(currentTags[i].Name))
                {
                    requiredTags++;
                }

                if (_tags.Contains(currentTags[i].Name))
                {
                    tags++;
                }
            }

            // return true if
            //   we have at least one required tag and all required tags are in the list of current tags
            //   at least one of our normal tags exists
            return RequiredTagCount > 0 && requiredTags == RequiredTagCount || tags > 0;
        }

        public bool IsValid(List<Tag> currentTags, out List<Tag> validTags)
        {
            if (TotalTagCount == 0 && currentTags.Count == 0)
            {
                validTags = new List<Tag>();
                return true;
            }

            var tags = 0;
            var requiredTags = 0;
            var valid = new List<Tag>();
            for (var i = 0; i < currentTags.Count; i++)
            {
                // if we run into a tag this piece of music is excluded from being played when it is there, return false
                if (_exclusions.Contains(currentTags[i].Name))
                {
                    validTags = new List<Tag>();
                    return false;
                }

                if (_required.Contains(currentTags[i].Name))
                {
                    requiredTags++;
                    valid.Add(currentTags[i]);
                }

                if (_tags.Contains(currentTags[i].Name))
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
                : new List<Tag>();
            return isValid;
        }

        /// <summary>
        ///     Removes the exclusion tag described by tags.
        /// </summary>
        ///
        /// <param name="tag"> The tag. </param>
        ///
        /// <returns>
        ///     A List&lt;bool&gt;
        /// </returns>
        public bool RemoveExclusionTag(string tag)
        {
            return _exclusions.Remove(tag);
        }

        /// <summary>
        ///     Removes the exclusion tag described by tags.
        /// </summary>
        ///
        /// <param name="tags"> The tags. </param>
        ///
        /// <returns>
        ///     A List&lt;bool&gt;
        /// </returns>
        public List<bool> RemoveExclusionTag(List<string> tags)
        {
            var output = new List<bool>(tags.Count);
            for (var i = 0; i < tags.Count; i++)
            {
                output.Add(_exclusions.Remove(tags[i]));
            }

            return output;
        }

        /// <summary>
        ///     Removes the normal tag described by tags.
        /// </summary>
        ///
        /// <param name="tag"> The tag. </param>
        ///
        /// <returns>
        ///     A List&lt;bool&gt;
        /// </returns>
        public bool RemoveNormalTag(string tag)
        {
            return _tags.Remove(tag);
        }

        /// <summary>
        ///     Removes the normal tag described by tags.
        /// </summary>
        ///
        /// <param name="tags"> The tags. </param>
        ///
        /// <returns>
        ///     A List&lt;bool&gt;
        /// </returns>
        public List<bool> RemoveNormalTag(List<string> tags)
        {
            var output = new List<bool>(tags.Count);
            for (var i = 0; i < tags.Count; i++)
            {
                output.Add(_tags.Remove(tags[i]));
            }

            return output;
        }

        /// <summary>
        ///     Removes the required tag described by tags.
        /// </summary>
        ///
        /// <param name="tag"> The tag. </param>
        ///
        /// <returns>
        ///     A List&lt;bool&gt;
        /// </returns>
        public bool RemoveRequiredTag(string tag)
        {
            return _required.Remove(tag);
        }

        /// <summary>
        ///     Removes the required tag described by tags.
        /// </summary>
        ///
        /// <param name="tags"> The tags. </param>
        ///
        /// <returns>
        ///     A List&lt;bool&gt;
        /// </returns>
        public List<bool> RemoveRequiredTag(List<string> tags)
        {
            var output = new List<bool>(tags.Count);
            for (var i = 0; i < tags.Count; i++)
            {
                output.Add(_required.Remove(tags[i]));
            }

            return output;
        }

        /// <summary>
        ///     Removes the tag described by tags.
        /// </summary>
        ///
        /// <exception cref="Exception"> Thrown when an exception error condition occurs. </exception>
        ///
        /// <param name="tag">     The tag. </param>
        /// <param name="tagType"> Type of the tag. </param>
        ///
        /// <returns>
        ///     A List&lt;bool&gt;
        /// </returns>
        public bool RemoveTag(string tag, TagType tagType)
        {
            switch (tagType)
            {
                case TagType.Normal:
                    return RemoveNormalTag(tag);

                case TagType.Exclusion:
                    return RemoveExclusionTag(tag);

                case TagType.Required:
                    return RemoveRequiredTag(tag);

                default:
                    throw new Exception("Unsupported tag type!");
            }
        }

        /// <summary>
        ///     Removes the tag described by tags.
        /// </summary>
        ///
        /// <exception cref="Exception"> Thrown when an exception error condition occurs. </exception>
        ///
        /// <param name="tags">    The tags. </param>
        /// <param name="tagType"> Type of the tag. </param>
        ///
        /// <returns>
        ///     A List&lt;bool&gt;
        /// </returns>
        public List<bool> RemoveTag(List<string> tags, TagType tagType)
        {
            switch (tagType)
            {
                case TagType.Normal:
                    return RemoveNormalTag(tags);

                case TagType.Exclusion:
                    return RemoveExclusionTag(tags);

                case TagType.Required:
                    return RemoveRequiredTag(tags);

                default:
                    throw new Exception("Unsupported tag type!");
            }
        }

        /// <summary>
        ///     Removes the tag described by tags.
        /// </summary>
        ///
        /// <exception cref="Exception"> Thrown when an exception error condition occurs. </exception>
        ///
        /// <param name="tags"> The tags. </param>
        public List<bool> RemoveTag(List<(string, TagType)> tags)
        {
            var output = new List<bool>(tags.Count);
            for (var i = 0; i < tags.Count; i++)
            {
                switch (tags[i].Item2)
                {
                    case TagType.Normal:
                        output.Add(RemoveNormalTag(tags[i].Item1));
                        break;

                    case TagType.Exclusion:
                        output.Add(RemoveExclusionTag(tags[i].Item1));
                        break;

                    case TagType.Required:
                        output.Add(RemoveRequiredTag(tags[i].Item1));
                        break;

                    default:
                        throw new Exception("Unsupported tag type!");
                }
            }

            return output;
        }
    }
}