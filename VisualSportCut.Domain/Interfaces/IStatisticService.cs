using VisualSportCut.Domain.Models;

namespace VisualSportCut.Domain.Interfaces
{
    public interface IStatisticsService
    {
        IEnumerable<StatItem> GetStatsByTag(string tagName);
        IEnumerable<StatItem> GetStatsByPeriod(string periodName);
        IEnumerable<StatItem> GetStatsByLabel(string labelType, string labelValue);
        TimeSpan ParseTime(string timeString);
    }
}
