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

        public IEnumerable<StatItem> GetStatsByGroup(string groupName)
        => _stamps
            .Where(s => s.Tag.Group.Contains(groupName))
            .GroupBy(s => s.Tag?.Name)
            .Select(g => StatItem.Create(g.Key!, g.Count(), g.First().Tag.Color));

        public IEnumerable<StatItem> GetStatsByPeriod(string periodName)
            => _stamps
                .Where(s => s.TimeEvents.Any(te => te.Name.Contains(periodName)))
                .GroupBy(s => s.Tag?.Name)
                .Select(g => StatItem.Create(g.Key!, g.Count(), g.First().Tag.Color));

        public IEnumerable<StatItem> GetStatsByLabel(string labelGroup, string labelName)
        => _stamps
                .Where(s => s.LabelEvents.Any(le => le.Group.Contains(labelGroup) && le.Name.Contains(labelName)))
                .GroupBy(s => s.Tag?.Name)
                .Select(g => StatItem.Create(g.Key!, g.Count(), g.First().Tag.Color));

        public IEnumerable<StatItem> GetStatsByTime(double startTime, double endTime)
        {
            var start = TimeSpan.FromMinutes(startTime);
            var end = TimeSpan.FromMinutes(endTime);

            return _stamps
                .Where(s => s.StartTime >= start && s.EndTime <= end)
                .GroupBy(s => s.Tag?.Name)
                .Select(g => StatItem.Create(g.Key!, g.Count(), g.First().Tag.Color));
        }
    }
}
