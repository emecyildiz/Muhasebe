using Microsoft.EntityFrameworkCore;
using Muhasebe.Common.Enums;
using Muhasebe.Models.Entities;
using Muhasebe.Services.Interfaces;
using Muhasebe.ViewModels;

namespace Muhasebe.Services
{
    public class MasrafTalebiService : IMasrafTalebiService
    {

        private readonly MuhasebeContext _context;

        public MasrafTalebiService(MuhasebeContext context)
        {
            _context = context;
        }

        public async Task<List<MasrafTalebi>> GetTumTaleplerAsync()
        {
            return await _context.MasrafTalebis
                .Include(m => m.TalepEdenKullanici)
                .Include(m => m.Departman)
                .Include(m => m.Kategori)
                .OrderByDescending(m => m.TalepId)
                .ToListAsync();
        }

        public async Task<List<MasrafTalebi>> GetTaleplerByPersonelIdAsync(int personelId)
        {
            return await _context.MasrafTalebis
                .Where(m=> m.TalepEdenKullaniciId == personelId)
                .ToListAsync();
        }

        public async Task<List<MasrafTalebi>> GetTaleplerByDepartmanIdAsync(int departmanId)
        {
            return await _context.MasrafTalebis
                .Include(m=> m.TalepEdenKullanici)
                .Where(m => m.DepartmanId == departmanId)
                .ToListAsync();
        }

        public async Task<MasrafTalebi> GetTalepByIdAsync(int id)
        {
            return await _context.MasrafTalebis.FindAsync(id);
        }

        public async Task EkleAsync(MasrafTalebiCreateViewModel model, int personelId, int departmanId)
        {
            var masrafTalebi = new MasrafTalebi
            {
                TalepEdenKullaniciId = personelId,
                DepartmanId = departmanId,
                KategoriId = model.KategoriId,
                Aciklama = model.Aciklama,
                Tutar = model.Tutar,
                TalepTarihi = DateTime.Now,
                Durum = DurumEnum.Bekliyor.ToString(),
            };
            await _context.MasrafTalebis.AddAsync(masrafTalebi);
            await _context.SaveChangesAsync();
        }

        public async Task DurumGuncelleAsync(int talepId, int durumId, int onaylayanId)
        {
            var talep = await _context.MasrafTalebis.FindAsync(talepId);
            if (talep != null)
            {
                talep.Durum = ((DurumEnum)durumId).ToString();
                await _context.SaveChangesAsync();
            }
        }

    }
}
