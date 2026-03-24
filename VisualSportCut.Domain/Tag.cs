namespace VisualSportCut.Domain
{
    public class Tag
    {
        public Tag() { }

        public string Name { get; private set; } = null!;
        public string Group { get; private set; } = null!;
        public string Color { get; private set;  } = null!;

        public static Tag Create(string? name, string? group, string? color)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(name);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(group);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(color);

            var tag = new Tag
            {
                Name = name,
                Group = group,
                Color = color
            };

            return tag;
        }
    }
}
