namespace Velentr.Audio.Internal
{
    internal class Music : Audio
    {

        public Music(string name, string path, AudioManager manager) : base(name, path, manager) { }

        public override void Load()
        {
            throw new System.NotImplementedException();
        }

        public override void Unload()
        {
            throw new System.NotImplementedException();
        }

        public override void Play()
        {
            throw new System.NotImplementedException();
        }

        public override void ChangeVolume(int newVolume)
        {
            throw new System.NotImplementedException();
        }

        public override void ChangePitch(int newPitch)
        {
            throw new System.NotImplementedException();
        }

        public override void Pause()
        {
            throw new System.NotImplementedException();
        }

        public override void Resume()
        {
            throw new System.NotImplementedException();
        }

        public override void Stop()
        {
            throw new System.NotImplementedException();
        }

    }
}
