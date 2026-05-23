using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Muhasebe.Models.Entities;

namespace Muhasebe.Controllers
{
    [Authorize(Roles = "Admin")]
    public class IslemKategorisiController : Controller
    {
        private readonly MuhasebeContext _context;

        public IslemKategorisiController(MuhasebeContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _context.IslemKategorisis.ToListAsync();
            return View(data);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IslemKategorisi model)
        {
            if (ModelState.IsValid)
            {
                _context.IslemKategorisis.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var kategori = await _context.IslemKategorisis.FindAsync(id);
            if (kategori == null) return NotFound();
            return View(kategori);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, IslemKategorisi model)
        {
            if (id != model.KategoriId) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var kategori = await _context.IslemKategorisis.FindAsync(id);
            if (kategori == null) return NotFound();
            return View(kategori);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var kategori = await _context.IslemKategorisis.FindAsync(id);
            if (kategori != null)
            {
                _context.IslemKategorisis.Remove(kategori);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
