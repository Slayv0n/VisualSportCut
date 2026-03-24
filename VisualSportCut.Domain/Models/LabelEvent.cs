using System.Drawing;

namespace VisualSportCut.Domain.Models
{
    public class LabelEvent
    {
        public LabelEvent() { }
        public string Name { get; private set; } = null!;
        public string Group { get; private set; } = null!;

        public static LabelEvent Create(string? name, string? group)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(name);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(group);

            var labelEvent = new LabelEvent
            {
                Name = name,
                Group = group
            };

            return labelEvent;
        } 
    }
}
