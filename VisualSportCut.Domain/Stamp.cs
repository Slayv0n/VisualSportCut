namespace VisualSportCut.Domain
{
    public class Stamp
    {
        public Stamp() { }
        public string Name { get; private set; } = null!;
        public DateTime StartTime { get; private set; }
        public DateTime EndTime { get; private set; }
        public List<TimeEvent> TimeEvents { get; private set; } = new List<TimeEvent>();
        public Tag Tag { get; private set; } = new Tag();
        public List<LabelEvent> LabelEvents { get; private set; } = new List<LabelEvent>();

        public static Stamp Create(string? name,
            DateTime startTime,
            DateTime endTime,
            List<TimeEvent> timeEvents,
            Tag tag,
            List<LabelEvent> labelEvents)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(name);
            ArgumentNullException.ThrowIfNull(timeEvents);
            ArgumentNullException.ThrowIfNull(tag);
            ArgumentNullException.ThrowIfNull(labelEvents);
            ArgumentOutOfRangeException.ThrowIfLessThan(endTime, startTime);

            var stamp = new Stamp
            {
                Name = name,
                StartTime = startTime,
                EndTime = endTime,
                Tag = tag,
                LabelEvents = labelEvents,

            };

            return stamp;
        }
    }
}
