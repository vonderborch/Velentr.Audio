using System;
using System.Collections.Generic;
using Velentr.Audio.Procedural;

namespace Velentr.Audio.DevEnv
{
    public class StandardBass : Instrument
    {

        private int counter;

        private Random r;

        private List<string> sounds;

        private int note = 0;

        private int noteSwitch = 8;

        private double bassPercent = 0.8;

        private int swing = 0;

        public StandardBass(AudioManager manager) : base(manager, "StandardBass")
        {
            counter = 0;
            r = new Random();

            sounds = new List<string>()
            {
                "bass_a2",
                "bass_b2",
                "bass_c2",
                "bass_d2",
                "bass_e2",
                "bass_f2",
                "bass_g2"
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
