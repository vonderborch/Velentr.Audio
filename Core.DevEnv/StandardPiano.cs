using System;
using System.Collections.Generic;
using Velentr.Audio.Procedural;

namespace Velentr.Audio.DevEnv
{
    public class StandardPiano : Instrument
    {

        private int counter;

        private Random r;

        private List<string> sounds;

        private int note = 0;

        private int noteSwitch = 16;

        private double bassPercent = 0.4;

        private int swing = 0;

        public StandardPiano(AudioManager manager) : base(manager, "StandardPiano")
        {
            counter = 0;
            r = new Random();

            sounds = new List<string>()
            {
                "elecP_A",
                "elecP_B",
                "elecP_C",
                "elecP_D",
                "elecP_E",
                "elecP_F",
                "elecP_G"
            };

            swing = 10000;
        }

        public override void Reset(long seed)
        {
            swing = 10000;

            r = new Random((int) seed);
            counter = 0;
            note = r.Next(0, sounds.Count);
        }

        public override void Update(uint millisecondsSinceLastTick)
        {
            if (millisecondsSinceLastTick == 0)
            {
                swing = r.Next(0, 30);
                counter += 1;
                GC.Collect();
            }
            else if (millisecondsSinceLastTick > swing)
            {
                swing = 10000;
                if (counter % noteSwitch == 0)
                {
                    note = r.Next(0, sounds.Count);
                }

                if (r.NextDouble() < bassPercent)
                {
                    var id = Manager.GenerateNewAudioInstance(sounds[note], AudioManager.MUSIC_CATEGORY, true, false);
                }
            }
        }

    }
}
