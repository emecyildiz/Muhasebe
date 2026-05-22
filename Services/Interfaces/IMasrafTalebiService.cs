using Muhasebe.Models.Entities;
using Muhasebe.ViewModels;
namespace Muhasebe.Services.Interfaces
{
    public interface IMasrafTalebiService
    {

        Task<List<MasrafTalebi>> GetTumTaleplerAsync();
        Task<List<MasrafTalebi>> GetTaleplerByPersonelIdAsync(int personelId);
        Task<List<MasrafTalebi>> GetTaleplerByDepartmanIdAsync(int departmanId);
        Task<MasrafTalebi> GetTalepByIdAsync(int id);
        Task EkleAsync(MasrafTalebiCreateViewModel model, int personelId, int departmanId);
        Task DurumGuncelleAsync(int talepId, int durumId, int onaylayanId);

    }
}
