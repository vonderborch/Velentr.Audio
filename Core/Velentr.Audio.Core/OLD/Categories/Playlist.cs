using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Audio;
using Velentr.Audio.OLD.Tagging;

namespace Velentr.Audio.OLD.Categories
{
    public class Playlist
    {

        private TagSet _validTags;

        private TagSet _pauseTags;

        private TagSet _stopTags;

        private HashSet<string> _associatedAudio;

        private List<ulong> _associatedInstances;

        private string _currentMusicName;

        private Random r;

        public Playlist(AudioManager manager, string name, bool randomize, int maxTriesToAvoidRepeats = 3)
        {
            Manager = manager;
            Name = name;
            Randomize = randomize;
            MaxTriesToAvoidRepeats = maxTriesToAvoidRepeats;

            _currentMusicName = string.Empty;
            _associatedAudio = new HashSet<string>();
            _associatedInstances = new List<ulong>();
            _validTags = new TagSet();
            _pauseTags = new TagSet();
            _stopTags = new TagSet();
        }

        public string Name { get; }

        public AudioManager Manager { get; }

        public bool Randomize { get; set; }

        public int MaxTriesToAvoidRepeats { get; set; }

        public void AddAssociatedAudio(string name)
        {
            _associatedAudio.Add(name);
        }

        public bool RemoveAssociatedAudio(string name)
        {
            return _associatedAudio.Remove(name);
        }

        public void UpdatePlaylistValidTagset(TagSet newTagSet)
        {
            _validTags = newTagSet;
        }

        public void UpdatePlaylistPausedTagset(TagSet newTagSet)
        {
            _pauseTags = newTagSet;
        }

        public void UpdatePlaylistStopTagset(TagSet newTagSet)
        {
            _stopTags = newTagSet;
        }

        public void AddPlaylistValidTag(string tag, TagType tagType)
        {
            _validTags.AddTag(tag, tagType);
        }

        public void AddPlaylistValidTag(List<string> tags, TagType tagType)
        {
            _validTags.AddTag(tags, tagType);
        }

        public void AddPlaylistValidTag(List<(string, TagType)> tags)
        {
            _validTags.AddTag(tags);
        }

        public void AddPauseTag(string tag, TagType tagType)
        {
            _pauseTags.AddTag(tag, tagType);
        }

        public void AddPauseTag(List<string> tags, TagType tagType)
        {
            _pauseTags.AddTag(tags, tagType);
        }

        public void AddPauseTag(List<(string, TagType)> tags)
        {
            _pauseTags.AddTag(tags);
        }

        public void AddStopTag(string tag, TagType tagType)
        {
            _stopTags.AddTag(tag, tagType);
        }

        public void AddStopTag(List<string> tags, TagType tagType)
        {
            _stopTags.AddTag(tags, tagType);
        }

        public void AddStopTag(List<(string, TagType)> tags)
        {
            _stopTags.AddTag(tags);
        }

        public bool RemovePlaylistValidTag(string tag, TagType tagType)
        {
            return _validTags.RemoveTag(tag, tagType);
        }

        public List<bool> RemovePlaylistValidTag(List<string> tags, TagType tagType)
        {
            return _validTags.RemoveTag(tags, tagType);
        }

        public List<bool> RemovePlaylistValidTag(List<(string, TagType)> tags)
        {
            return _validTags.RemoveTag(tags);
        }

        public bool RemovePauseTag(string tag, TagType tagType)
        {
            return _pauseTags.RemoveTag(tag, tagType);
        }

        public List<bool> RemovePauseTag(List<string> tags, TagType tagType)
        {
            return _pauseTags.RemoveTag(tags, tagType);
        }

        public List<bool> RemovePauseTag(List<(string, TagType)> tags)
        {
            return _pauseTags.RemoveTag(tags);
        }

        public bool RemoveStopTag(string tag, TagType tagType)
        {
            return _stopTags.RemoveTag(tag, tagType);
        }

        public List<bool> RemoveStopTag(List<string> tags, TagType tagType)
        {
            return _stopTags.RemoveTag(tags, tagType);
        }

        public List<bool> RemoveStopTag(List<(string, TagType)> tags)
        {
            return _stopTags.RemoveTag(tags);
        }

        public bool ShouldPlaylistBePlayed(List<Tag> currentTags)
        {
            return _validTags.IsValid(currentTags);
        }

        public bool ShouldPlaylistBePaused(List<Tag> currentTags)
        {
            return _pauseTags.IsValid(currentTags);
        }

        public bool ShouldPlaylistBeStopped(List<Tag> currentTags)
        {
            return _stopTags.IsValid(currentTags);
        }

        public void Update(List<Tag> currentTags)
        {
            // check if the we need to pause the current music that is playing
            var shouldPause = ShouldPlaylistBePaused(currentTags);

            // check if we need to stop the current music that is playing
            var shouldStop = ShouldPlaylistBeStopped(currentTags);

            // get all currently playing instances...
            var instances = Manager.GetMusicInstances(_associatedInstances);
            var newMusic = instances.Count == 0;
            var instancestoKill = new List<ulong>();
            for (var i = 0; i < instances.Count; i++)
            {
                if (shouldPause)  
                {
                    instances[i].Pause();
                    newMusic = false;
                }
                else if (shouldStop)
                {
                    instances[i].Stop(true);
                    instancestoKill.Add(instances[i].ID);
                    _currentMusicName = string.Empty;
                    newMusic = false;
                }
                else
                {
                    switch (instances[i].State)
                    {
                        case SoundState.Stopped:
                            instancestoKill.Add(instances[i].ID);
                            newMusic = true;
                            break;
                        case SoundState.Paused:
                            instances[i].Resume();
                            newMusic = false;
                            break;
                    }
                }
            }

            // play new music if we need to
            if (newMusic)
            {
                var audio = GetTaggedAudio(currentTags);
                if (audio.Count == 1)
                {
                    // if only one song is valid, that is definitely the next one
                    _currentMusicName = audio[0].Name;
                    Manager.GenerateNewAudioInstance(audio[0].Name, AudioManager.MUSIC_CATEGORY);
                }
                else if (audio.Count == 0)
                {
                    // no audio!
                    _currentMusicName = string.Empty;
                }
                else
                {
                    if (Randomize)
                    {
                        var itemId = 0;
                        for (var i = 0; i < MaxTriesToAvoidRepeats; i++)
                        {
                            itemId = Manager.Randomizer.GetNextRandomInt(0, audio.Count);
                            if (!_currentMusicName.Equals(audio[itemId].Name))
                            {
                                break;
                            }
                        }

                        _currentMusicName = audio[itemId].Name;
                        Manager.GenerateNewAudioInstance(audio[itemId].Name, AudioManager.MUSIC_CATEGORY);
                    }
                    else
                    {
                        var currentIndex = -1;
                        if (!string.IsNullOrEmpty(_currentMusicName))
                        {
                            currentIndex = audio.FindIndex(x => x.Name == _currentMusicName);
                        }

                        if (currentIndex == -1 || currentIndex >= audio.Count - 1)
                        {
                            currentIndex = 0;
                        }
                        else
                        {
                            currentIndex++;
                        }
                        _currentMusicName = audio[currentIndex].Name;
                        Manager.GenerateNewAudioInstance(audio[currentIndex].Name, AudioManager.MUSIC_CATEGORY);
                    }
                }
            }
        }

        public List<Sound.Audio> GetTaggedAudio(List<Tag> currentTags)
        {
            var audio = Manager.GetAudio(_associatedAudio.ToList());
            var output = new List<Sound.Audio>();

            for (var i = 0; i < audio.Count; i++)
            {
                if (audio[i].IsValid(currentTags))
                {
                    output.Add(audio[i]);
                }
            }

            return output;
        }

    }
}
