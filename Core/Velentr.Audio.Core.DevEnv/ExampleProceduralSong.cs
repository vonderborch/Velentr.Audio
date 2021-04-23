using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Velentr.Audio.Helpers;
using Velentr.Audio.Procedural;

namespace Velentr.Audio.Core.DevEnv
{
    public class ExampleProceduralSong : ProceduralSong
    {

        private Randomizer r;

        private Instrument bass;

        private Instrument piano;

        private uint timer;

        private int counter = 31;

        private int swit = 32;

        private long bassSeed = 0;

        private long pianoSeed = 0;

        public ExampleProceduralSong(AudioManager manager, long defaultSeed) : base(manager, "ExampleSong", defaultSeed)
        {
            bass = new StandardBass(manager);
            piano = new StandardPiano(manager);
            BeatsPerMinute = 120;
            timer = 0;
            Reset();
            DefaultSeed = manager.Randomizer.GetNextRandomLong();

        }

        public override Randomizer Randomizer
        {
            get => r;
            set => r = value;
        }

        public override void Reset(long seed)
        {
            r = new SystemRandomizer(seed);
            bassSeed = r.GetNextRandomLong();
            pianoSeed = r.GetNextRandomLong();
            bass.Reset(bassSeed);
            piano.Reset(pianoSeed);

            counter = 31;

        }

        public override void Update(GameTime gameTime)
        {
            timer += (uint)gameTime.ElapsedGameTime.Milliseconds;

            if (timer >= (59000 + r.GetNextRandomInt(0, 2000)) / BeatsPerMinute / 4)
            {
                counter++;
                // if the counter is equal to the switch, 
                //then reset the song.
                if (counter == swit)
                {
                    piano.Reset(pianoSeed);
                    bass.Reset(bassSeed);
                    counter = 0;
                }
                timer = 0;
            }

            piano.Update(timer); //update the
            bass.Update(timer);  //instruments
        }

    }
}
