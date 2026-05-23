using Muhasebe.Models.Entities;
namespace Muhasebe.Services.Interfaces
{
    public interface IKullaniciService
    {
        Task<List<Kullanici>> GetAllKullaniciAsync(int? departmanId = null);
        Task<Kullanici> GetKullaniciByIdAsync(int id);
        Task CreateKullaniciAsync(Kullanici kullanici);
        Task UpdateKullaniciAsync(Kullanici kullanici);
        Task DeleteKullaniciAsync(int id);
        Task<Kullanici> GetKullaniciDetailAsync(int id);
    }
}
