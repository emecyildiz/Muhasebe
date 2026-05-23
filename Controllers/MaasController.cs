using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Muhasebe.Models.Entities;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;

namespace Muhasebe.Controllers
{
    [Authorize]
    public class MaasController : Controller
    {
        private readonly MuhasebeContext _context;

        public MaasController(MuhasebeContext context)
        {
            _context = context;
        }

        // GET: Maas
        public async Task<IActionResult> Index()
        {
            var aktifKullaniciId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            IQueryable<Maas> maaslar = _context.Maas.Include(m => m.Kullanici);

            if (userRole == "Mudur")
            {
                var aktifDepartmanId = int.Parse(User.FindFirstValue("DepartmanId"));
                maaslar = maaslar.Where(m => m.Kullanici.DepartmanId == aktifDepartmanId);
            }
            else if (userRole != "Admin")
            {
                // Çalışanlar sadece kendilerini görür
                maaslar = maaslar.Where(m => m.KullaniciId == aktifKullaniciId);
            }

            return View(await maaslar.ToListAsync());
        }

        private async Task PopulateKullaniciListAsync(int? selectedKullaniciId = null)
        {
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            IQueryable<Kullanici> kullanicilar = _context.Kullanicis;

            if (userRole == "Mudur")
            {
                var aktifDepartmanId = int.Parse(User.FindFirstValue("DepartmanId"));
                var aktifKullaniciId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                // Müdür kendi departmanındaki kişileri görür ancak kendini göremez (kendine maaş atayamaz)
                kullanicilar = kullanicilar.Where(k => k.DepartmanId == aktifDepartmanId && k.KullaniciId != aktifKullaniciId);
            }

            var secilebilirKullanicilar = await kullanicilar
                .Select(k => new { k.KullaniciId, AdSoyad = k.Ad + " " + k.Soyad })
                .ToListAsync();

            ViewData["KullaniciId"] = new SelectList(secilebilirKullanicilar, "KullaniciId", "AdSoyad", selectedKullaniciId);
        }

        private async Task<bool> IsAuthorizedForKullaniciAsync(int hedefKullaniciId)
        {
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            if (userRole == "Admin") return true;

            if (userRole == "Mudur")
            {
                var aktifDepartmanId = int.Parse(User.FindFirstValue("DepartmanId"));
                var aktifKullaniciId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                // Kendisine müdahale edemez
                if (hedefKullaniciId == aktifKullaniciId) return false;

                var hedefKullanici = await _context.Kullanicis.FindAsync(hedefKullaniciId);
                return hedefKullanici != null && hedefKullanici.DepartmanId == aktifDepartmanId;
            }

            return false;
        }

        // GET: Maas/Create
        [Authorize(Roles = "Admin,Mudur")]
        public async Task<IActionResult> Create()
        {
            await PopulateKullaniciListAsync();
            return View();
        }

        // POST: Maas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Mudur")]
        public async Task<IActionResult> Create([Bind("KullaniciId,AylikTutar,BaslangicTarihi,BitisTarihi")] Maas maas)
        {
            ModelState.Remove("Kullanici");
            ModelState.Remove("FinansalIslems");

            if (ModelState.IsValid)
            {
                if (!await IsAuthorizedForKullaniciAsync(maas.KullaniciId))
                {
                    return Forbid();
                }

                _context.Add(maas);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            await PopulateKullaniciListAsync(maas.KullaniciId);
            return View(maas);
        }

        // GET: Maas/Edit/5
        [Authorize(Roles = "Admin,Mudur")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var maas = await _context.Maas.FindAsync(id);
            if (maas == null) return NotFound();

            if (!await IsAuthorizedForKullaniciAsync(maas.KullaniciId))
            {
                return Forbid();
            }

            await PopulateKullaniciListAsync(maas.KullaniciId);
            return View(maas);
        }

        // POST: Maas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Mudur")]
        public async Task<IActionResult> Edit(int id, [Bind("MaasId,KullaniciId,AylikTutar,BaslangicTarihi,BitisTarihi")] Maas maas)
        {
            if (id != maas.MaasId) return NotFound();

            ModelState.Remove("Kullanici");
            ModelState.Remove("FinansalIslems");

            if (ModelState.IsValid)
            {
                if (!await IsAuthorizedForKullaniciAsync(maas.KullaniciId))
                {
                    return Forbid();
                }

                try
                {
                    _context.Update(maas);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MaasExists(maas.MaasId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            await PopulateKullaniciListAsync(maas.KullaniciId);
            return View(maas);
        }

        // GET: Maas/Delete/5
        [Authorize(Roles = "Admin,Mudur")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var maas = await _context.Maas
                .Include(m => m.Kullanici)
                .FirstOrDefaultAsync(m => m.MaasId == id);
            
            if (maas == null) return NotFound();

            if (!await IsAuthorizedForKullaniciAsync(maas.KullaniciId))
            {
                return Forbid();
            }

            return View(maas);
        }

        // POST: Maas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Mudur")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var maas = await _context.Maas.FindAsync(id);
            if (maas != null)
            {
                if (!await IsAuthorizedForKullaniciAsync(maas.KullaniciId))
                {
                    return Forbid();
                }

                _context.Maas.Remove(maas);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool MaasExists(int id)
        {
            return _context.Maas.Any(e => e.MaasId == id);
        }
    }
}
