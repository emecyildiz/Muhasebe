using Muhasebe.Models.Entities;
using Muhasebe.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace Muhasebe.Services
{
    public class DepartmanButceService : IDepartmanButceService
    {

        private readonly MuhasebeContext _context;

        public DepartmanButceService(MuhasebeContext context)
        {
            _context = context;
        }

        public async Task<List<DepartmanButce>> GetAllAsync()
        {
            return await _context.DepartmanButces
                .Include(db => db.Departman)
                .ToListAsync();
        }

        public async Task<DepartmanButce> GetByIdAsync(int id)
        {
            return await _context.DepartmanButces.FindAsync(id);
        }

        public async Task CreateAsync(DepartmanButce departmanButce)
        {
            _context.DepartmanButces.Add(departmanButce);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(int id, DepartmanButce departmanButce)
        {
            var existingButce = await _context.DepartmanButces.FindAsync(id);
            if (existingButce != null)
            {
                existingButce.DepartmanId = departmanButce.DepartmanId;
                existingButce.Yil = departmanButce.Yil;
                existingButce.Ay = departmanButce.Ay;
                existingButce.AyrilanButce = departmanButce.AyrilanButce;
                existingButce.KullanilanButce = departmanButce.KullanilanButce;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var butce = await _context.DepartmanButces.FindAsync(id);
            if (butce != null)
            {
                _context.DepartmanButces.Remove(butce);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<DepartmanButce>> GetDetailByIdAsync(int id)
        {
            return await _context.DepartmanButces
                .Where(db => db.DepartmanId == id)
                .Include(db => db.Departman)
                .ToListAsync();
        }
    }
}
