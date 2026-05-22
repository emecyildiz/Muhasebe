using Muhasebe.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Muhasebe.Services.Interfaces;
namespace Muhasebe.Services
{
    public class KullaniciService : IKullaniciService
    {

        private readonly MuhasebeContext _context;

        public KullaniciService(MuhasebeContext context)
        {
            _context = context;
        }

        public async Task<List<Kullanici>> GetAllKullaniciAsync()
        {
            return await _context.Kullanicis
                .Include(k => k.Rol)
                .Include(k => k.Departman)
                .ToListAsync();
        }

        public async Task<Kullanici> GetKullaniciByIdAsync(int id)
        {
            return await _context.Kullanicis.FindAsync(id);
        }

        public async Task CreateKullaniciAsync(Kullanici kullanici)
        {
            _context.Kullanicis.Add(kullanici);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateKullaniciAsync(Kullanici kullanici)
        {
            var existingKullanici = await _context.Kullanicis.FindAsync(kullanici.KullaniciId);
            if (existingKullanici != null)
            {
                existingKullanici.Ad = kullanici.Ad;
                existingKullanici.Soyad = kullanici.Soyad;
                existingKullanici.Eposta = kullanici.Eposta;
                existingKullanici.SifreHash = kullanici.SifreHash;
                existingKullanici.RolId = kullanici.RolId;
                existingKullanici.DepartmanId = kullanici.DepartmanId;
                existingKullanici.IseGirisTarihi = kullanici.IseGirisTarihi;
                existingKullanici.Durum = kullanici.Durum;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteKullaniciAsync(int id)
        {
            var kullanici = await _context.Kullanicis.FindAsync(id);
            if (kullanici != null)
            {
                _context.Kullanicis.Remove(kullanici);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Kullanici> GetKullaniciDetailAsync(int id)
        {
            return await _context.Kullanicis
                .Include(k => k.Rol)
                .Include(k => k.Departman)
                .Include(k => k.Maas)
                .FirstOrDefaultAsync(k => k.KullaniciId == id);
        }
    }
}
