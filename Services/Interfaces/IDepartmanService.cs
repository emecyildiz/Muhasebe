using Muhasebe.Models.Entities;
namespace Muhasebe.Services.Interfaces
{
    public interface IDepartmanService
    {
        Task<List<Departman>> GetAllDepartmanAsync();
        Task<Departman> GetDepartmanByIdAsync(int id);
        Task CreateDepartmanAsync(Departman departman);
        Task UpdateDepartmanAsync(Departman departman);
        Task DeleteDepartmanAsync(int id);
    }
}
