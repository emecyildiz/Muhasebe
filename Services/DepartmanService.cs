using Muhasebe.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Muhasebe.Services.Interfaces;
namespace Muhasebe.Services
{
    public class DepartmanService : IDepartmanService
    {
        private readonly MuhasebeContext _context;

        public DepartmanService(MuhasebeContext context)
        {
            _context = context;
        }

        public async Task<List<Departman>> GetAllDepartmanAsync()
        {
            return await _context.Departmen.Include(d => d.Mudur).ToListAsync();
        }

        public async Task<Departman> GetDepartmanByIdAsync(int id)
        {
            return await _context.Departmen.FindAsync(id);
        }

        public async Task CreateDepartmanAsync(Departman departman)
        {
            _context.Departmen.Add(departman);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateDepartmanAsync(Departman departman)
        {
            _context.Departmen.Update(departman);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDepartmanAsync(int id)
        {
            var departman = await _context.Departmen.FindAsync(id);
            if (departman != null)
            {
                _context.Departmen.Remove(departman);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Departman> GetDepartmanDetailAsync(int id)
        {
            return await _context.Departmen
                .Include(d => d.Kullanicis)
                .Include(d => d.Mudur)
                .FirstOrDefaultAsync(d => d.DepartmanId == id);
        }

    }
}
