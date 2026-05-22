using Muhasebe.Models.Entities;
namespace Muhasebe.Services.Interfaces
{
    public interface IDepartmanButceService
    {

        Task<List<DepartmanButce>> GetAllAsync();
        Task<DepartmanButce> GetByIdAsync(int id);
        Task CreateAsync(DepartmanButce departmanButce);
        Task UpdateAsync(int id, DepartmanButce departmanButce);
        Task DeleteAsync(int id);

        Task<List<DepartmanButce>> GetDetailByIdAsync(int id);


    }
}
