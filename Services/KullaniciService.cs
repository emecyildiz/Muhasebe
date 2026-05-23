using Muhasebe.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Muhasebe.Services.Interfaces;
using Muhasebe.Common.Helpers;

namespace Muhasebe.Services
{
    public class KullaniciService : IKullaniciService
    {

        private readonly MuhasebeContext _context;

        public KullaniciService(MuhasebeContext context)
        {
            _context = context;
        }

        public async Task<List<Kullanici>> GetAllKullaniciAsync(int? departmanId = null)
        {
            var query = _context.Kullanicis
                .Include(k => k.Rol)
                .Include(k => k.Departman)
                .AsQueryable();

            if (departmanId.HasValue)
            {
                query = query.Where(k => k.DepartmanId == departmanId.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<Kullanici> GetKullaniciByIdAsync(int id)
        {
            return await _context.Kullanicis.FindAsync(id);
        }

        public async Task CreateKullaniciAsync(Kullanici kullanici)
        {

            kullanici.SifreHash = SifrelemeHelper.Sifrele(kullanici.SifreHash);
            kullanici.Durum = true;

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
                if (!string.IsNullOrEmpty(kullanici.SifreHash) && kullanici.SifreHash.Length < 50)
                {
                    existingKullanici.SifreHash = SifrelemeHelper.Sifrele(kullanici.SifreHash);
                }
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
                .Include(k => k.MasrafTalebis)
                .FirstOrDefaultAsync(k => k.KullaniciId == id);
        }
    }
}
