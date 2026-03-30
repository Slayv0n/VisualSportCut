using VisualSportCut.Domain.Models;

namespace VisualSportCut.Domain.Interfaces
{
    public interface IStatisticService
    {
        IEnumerable<StatItem> GetStatsByGroup(string tagGroupName);
        IEnumerable<StatItem> GetStatsByPeriod(string periodName);
        IEnumerable<StatItem> GetStatsByLabel(string labelGroup, string labelName);
        IEnumerable<StatItem> GetStatsByTime(double startTime, double endTime);
        void SetStamps(List<Stamp> stamps);
    }
}
