namespace VisualSportCut.Domain
{
    public class TimeEvent
    {
        public TimeEvent() { }
        public string Name { get; private set; } = null!;

        public static TimeEvent Create(string? name)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(name);

            var timeEvent = new TimeEvent
            {
                Name = name
            };

            return timeEvent;
        }
    }
}
