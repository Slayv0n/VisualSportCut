namespace VisualSportCut.Domain.Models
{
    public class Stamp
    {
        public Stamp() { }
        public string Name { get; private set; } = null!;
        public TimeSpan StartTime { get; private set; }
        public TimeSpan EndTime { get; private set; }
        public List<TimeEvent> TimeEvents { get; private set; } = new List<TimeEvent>();
        public Tag Tag { get; private set; } = new Tag();
        public List<LabelEvent> LabelEvents { get; private set; } = new List<LabelEvent>();

        public static Stamp Create(string? name,
            string startTime,
            string endTime,
            List<TimeEvent> timeEvents,
            Tag tag,
            List<LabelEvent> labelEvents)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(name);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(startTime);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(endTime);
            ArgumentNullException.ThrowIfNull(timeEvents);
            ArgumentNullException.ThrowIfNull(tag);
            ArgumentNullException.ThrowIfNull(labelEvents);
            ArgumentOutOfRangeException.ThrowIfLessThan(endTime, startTime);

            var stamp = new Stamp
            {
                Name = name,
                StartTime = TimeSpan.ParseExact(startTime, "hh\\:mm\\:ss", null),
                EndTime = TimeSpan.ParseExact(endTime, "hh\\:mm\\:ss", null),
                TimeEvents = timeEvents,
                Tag = tag,
                LabelEvents = labelEvents,
            };

            return stamp;
        }
    }
}
