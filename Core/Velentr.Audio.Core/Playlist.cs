using System;
using System.Collections.Generic;
using System.Text;

namespace Velentr.Audio
{
    public class Playlist
    {

        private Dictionary<int, ulong> _instanceIds;

        private Dictionary<string, PlaylistMusic> _music;

        public Playlist(string name, List<PlaylistMusic> music, bool randomize)
        {
            Name = name;
            Randomize = randomize;
            _music = new Dictionary<string, PlaylistMusic>();
            _instanceIds = new Dictionary<int, ulong>();
            AddMusic(music);
        }

        public string Name { get; }

        public bool Randomize { get; set; }

        public void AddMusic(PlaylistMusic music)
        {
            _music.Add(music.Name, music);
        }

        public void AddMusic(List<PlaylistMusic> music)
        {
            for (var i = 0; i < music.Count; i++)
            {
                _music.Add(music[i].Name, music[i]);
            }
        }

        public bool RemoveMusic(string name)
        {
            return _music.Remove(name);
        }

        public void RemoveMusic(List<string> names)
        {
            for (var i = 0; i < names.Count; i++)
            {
                _music.Remove(names[i]);
            }
        }

        public void PauseMusic()
        {

        }

        public void StopMusic()
        {

        }

        public void ResumeMusic()
        {

        }

        public void Update()
        {

        }
    }
}
