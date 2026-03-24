using System;
using System.Collections.Generic;
using System.Text;

namespace VisualSportCut.Domain.Models
{
    public class StatItem
    {
        public StatItem() { }
        public string Category { get; private set; } = null!;
        public int Count { get; private set; }
        public string Color { get; private set; } = null!;

        public static StatItem Create(string category, int count, string color)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(category);
            ArgumentException.ThrowIfNullOrWhiteSpace(color);

            var statItem = new StatItem
            {
                Category = category,
                Count = count,
                Color = color
            };

            return statItem;
        }

    }
}
