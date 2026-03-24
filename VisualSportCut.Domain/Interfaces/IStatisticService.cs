using VisualSportCut.Domain.Models;

namespace VisualSportCut.Domain.Interfaces
{
    public interface IStatisticService
    {
        IEnumerable<StatItem> GetStatsByTag(string tagName);
        IEnumerable<StatItem> GetStatsByPeriod(string periodName);
        IEnumerable<StatItem> GetStatsByLabel(string labelType, string labelValue);
    }
}
