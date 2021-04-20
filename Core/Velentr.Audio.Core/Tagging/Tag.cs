namespace Velentr.Audio.Tagging
{
    public readonly struct Tag
    {

        public Tag(string name, bool consumable, uint lifespanMilliseconds, TagPriority priority)
        {
            Name = name;
            Consumable = consumable;
            LifespanMilliseconds = lifespanMilliseconds;
            Priority = priority;
        }

        public string Name { get; }

        public bool Consumable { get; }

        public uint LifespanMilliseconds { get; }

        public TagPriority Priority { get; }
    }
}
