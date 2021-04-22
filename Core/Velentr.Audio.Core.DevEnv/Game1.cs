﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Velentr.Audio;
using Velentr.Audio.Categoreies;
using Velentr.Audio.Helpers;
using Velentr.Audio.Playlists;
using Velentr.Audio.Tagging;

namespace Velentr.Input.DevEnv
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private AudioManager manager;

        private string _soundsCategory = "SOUNDS";


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // create an Audio Manager with the SOUNDS category (this could also be done by creating a new AudioManager without the categories parameter and then calling manager.AddCategory() with our categor(y/ies) instead)
            var categories = new List<CategoryLoadInfo>
            {
                new CategoryLoadInfo(_soundsCategory, 1f)
            };
            manager = new AudioManager(categories: categories);

            // Register some music with our audio manager...
            manager.AddMusic("Drums", "Content\\Music\\Drums-of-the-Deep-153-Loop-Full.wav", 0f, false);
            manager.AddMusic("Lord", "Content\\Music\\Lord-of-the-Land-130-Full.wav", 0f, false);
            manager.AddMusic("Master", "Content\\Music\\Master-of-the-Feast-122-Full.wav", 0f, false);
            manager.AddMusic("Liuto", "Content\\Music\\Suonatore-di-Liuto-Full.wav", 0f, false);
            manager.AddMusic("Tales", "Content\\Music\\Teller-of-the-Tales-130-Full.wav", 0f, false);

            // register some sounds with our manager. List them as belonging to the SOUNDS category
            var soundCategories = new List<string>() { _soundsCategory };
            manager.AddSound("beep", "Content\\Sounds\\33775__jobro__1-beep-a.wav", soundCategories, 0f, false, new List<string>() { "beep" });
            manager.AddSound("blaster", "Content\\Sounds\\27568__suonho__memorymoon-space-blaster-plays.wav", soundCategories, 0f, false, new List<string>() { "blast" });

            // create some playlists and say what tags should play or pause them...
            var playlist1 = new PlaylistLoadInfo("playlist1");
            playlist1.PlaylistValidTags.AddTag("play", TagType.Normal);
            playlist1.PlaylistPauasedTags.AddTag("pause", TagType.Normal);

            var playlist2 = new PlaylistLoadInfo("playlist2");
            playlist2.PlaylistValidTags.AddTag("pause", TagType.Normal);
            playlist2.PlaylistPauasedTags.AddTag("play", TagType.Normal);

            // Add some music tracks to the playlists...
            playlist1.MusicTracks.Add(new PlaylistMusic("Drums", new List<string>() { "normal", "stress" }));
            playlist1.MusicTracks.Add(new PlaylistMusic("Lord", new List<string>() { "normal" }, new List<string>() { "stress" }));
            playlist1.MusicTracks.Add(new PlaylistMusic("Tales", new List<string>() { "normal" }, new List<string>() { "stress" }));

            playlist2.MusicTracks.Add(new PlaylistMusic("Master", new List<string>() { "normal", "stress" }));
            playlist2.MusicTracks.Add(new PlaylistMusic("Liuto", new List<string>() { "normal" }, new List<string>() { "stress" }));

            // add the playlists to the Audio Manager...
            manager.AddPlaylist(new List<PlaylistLoadInfo>() { playlist1, playlist2 });

            // Setup some tags for what playlist and music we should play...
            manager.AddPlaylistChoiceTag("play");
            manager.AddMusicTag("normal");
        }

        private TimeSpan _lastBeepSoundTime = TimeSpan.MinValue;
        private TimeSpan _lastBlasterSoundTime = TimeSpan.MinValue;

        protected override void Update(GameTime gameTime)
        {
            // update the manager
            manager.Update(gameTime);

            // every 2.5 seconds, play a beep sound
            if (TimeHelpers.ElapsedMilliSeconds(_lastBeepSoundTime, gameTime) > 2500)
            {
                _lastBeepSoundTime = gameTime.TotalGameTime;
                manager.AddTag(_soundsCategory, "beep", true, 500);
            }

            // every 6 seconds, play a blaster sound
            if (TimeHelpers.ElapsedMilliSeconds(_lastBlasterSoundTime, gameTime) > 6000)
            {
                _lastBlasterSoundTime = gameTime.TotalGameTime;
                manager.AddSoundTag(_soundsCategory, "blast", true, 500);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
