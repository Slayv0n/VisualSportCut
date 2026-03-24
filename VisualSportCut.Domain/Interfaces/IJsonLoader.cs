using VisualSportCut.Domain.Models;

namespace VisualSportCut.Domain.Interfaces
{
    public interface IJsonLoader
    {
        Task<List<Stamp>> LoadAsync(string filePath);
    }
}
