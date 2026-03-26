using VisualSportCut.Domain.Models;

namespace VisualSportCut.Domain.Interfaces
{
    public interface IStatisticService
    {
        IEnumerable<StatItem> GetStatsByGroup(string tagGroupName);
        IEnumerable<StatItem> GetStatsByPeriod(string periodName);
        IEnumerable<StatItem> GetStatsByLabel(string labelGroup, string labelName);
        void SetStamps(List<Stamp> stamps);
    }
}
