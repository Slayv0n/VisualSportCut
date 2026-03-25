using System;
using System.Collections.Generic;
using System.Text;
using VisualSportCut.Domain.Interfaces;
using VisualSportCut.Domain.Models;

namespace VisualSportCut.Domain.Services
{
    public class StatisticService : IStatisticService
    {
        private List<Stamp> _stamps = new();

        public void SetStamps(List<Stamp> stamps) => _stamps = stamps;

        public IEnumerable<StatItem> GetStatsByGroup(string tagName)
        => _stamps
            .Where(s => s.Tag.Group.Contains(tagName))
            .GroupBy(s => s.Tag?.Name)
            .Select(g => StatItem.Create(g.Key!, g.Count(), g.First().Tag.Color));

        public IEnumerable<StatItem> GetStatsByPeriod(string periodName)
            => _stamps
                .Where(s => s.TimeEvents.Any(te => te.Name == periodName))
                .GroupBy(s => s.Tag?.Name)
                .Select(g => StatItem.Create(g.Key!, g.Count(), g.First().Tag.Color));

        public IEnumerable<StatItem> GetStatsByLabel(string labelType, string labelValue)
        => _stamps
                .Where(s => s.LabelEvents.Any(le => le.Group == le.Group && le.Name == labelValue))
                .GroupBy(s => s.Tag.Name) //В работе
                .Select(g => StatItem.Create(g.Key!, g.Count(), g.First().Tag.Color));
    }
}
