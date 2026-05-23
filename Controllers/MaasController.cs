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
                await PopulateMaasButcesiBilgileriAsync(aktifDepartmanId);
            }
            else if (userRole != "Admin")
            {
                // Çalışanlar sadece kendilerini görür
                maaslar = maaslar.Where(m => m.KullaniciId == aktifKullaniciId);
                await PopulateMaasButcesiBilgileriAsync(maaslar.FirstOrDefault()?.Kullanici?.DepartmanId);
            }

            ViewBag.AktifKullaniciId = aktifKullaniciId;
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

        private async Task<int> GetMaasKategoriIdAsync()
        {
            var maasKategori = await _context.IslemKategorisis.FirstOrDefaultAsync(k => k.KategoriAdi == "Maaş Bütçesi");
            if (maasKategori == null)
            {
                maasKategori = new IslemKategorisi { KategoriAdi = "Maaş Bütçesi", Tur = "Gider" };
                _context.IslemKategorisis.Add(maasKategori);
                await _context.SaveChangesAsync();
            }
            return maasKategori.KategoriId;
        }

        private async Task PopulateMaasButcesiBilgileriAsync(int? departmanId = null)
        {
            if (departmanId == null)
            {
                var claimDept = User.FindFirstValue("DepartmanId");
                if (!int.TryParse(claimDept, out int dId)) return;
                departmanId = dId;
            }

            var aktifButce = await _context.DepartmanButces
                .Where(b => b.DepartmanId == departmanId)
                .OrderByDescending(b => b.Yil)
                .ThenByDescending(b => b.Ay)
                .FirstOrDefaultAsync();

            decimal toplamMaasButcesi = 0;
            decimal kullanilanMaas = 0;

            if (aktifButce != null)
            {
                int maasKategoriId = await GetMaasKategoriIdAsync();
                var maasButceDetay = await _context.DepartmanButceDetays
                    .FirstOrDefaultAsync(d => d.ButceId == aktifButce.ButceId && d.KategoriId == maasKategoriId);

                toplamMaasButcesi = maasButceDetay?.AyrilanTutar ?? 0;

                kullanilanMaas = await _context.Maas
                    .Include(m => m.Kullanici)
                    .ThenInclude(k => k.Rol)
                    .Where(m => m.Kullanici.DepartmanId == departmanId && m.BitisTarihi == null && m.Kullanici.Rol.RolAdi != "Mudur" && m.Kullanici.Rol.RolAdi != "Admin")
                    .SumAsync(m => m.AylikTutar);
            }

            ViewBag.ToplamMaasButcesi = toplamMaasButcesi;
            ViewBag.KullanilanMaas = kullanilanMaas;
            ViewBag.KalanMaas = toplamMaasButcesi - kullanilanMaas;
        }

        private async Task<string?> ValidateMaasButcesiAsync(int kullaniciId, decimal yeniMaasTutar, int? mevcutMaasId = null)
        {
            var user = await _context.Kullanicis.Include(k => k.Rol).FirstOrDefaultAsync(k => k.KullaniciId == kullaniciId);
            if (user == null) return "Kullanıcı bulunamadı.";

            // Eğer maaşı atanan kişi Müdür veya Admin ise, departman bütçesi limitine takılmaz (Şirket ana bütçesinden karşılanır).
            if (user.Rol?.RolAdi == "Mudur" || user.Rol?.RolAdi == "Admin")
            {
                return null;
            }

            // Aktif bütçeyi bul (örneğin en son yıla ait)
            var aktifButce = await _context.DepartmanButces
                .Where(b => b.DepartmanId == user.DepartmanId)
                .OrderByDescending(b => b.Yil)
                .ThenByDescending(b => b.Ay)
                .FirstOrDefaultAsync();

            if (aktifButce == null)
            {
                return "Departmanın tanımlı bir bütçesi bulunmamaktadır.";
            }

            int maasKategoriId = await GetMaasKategoriIdAsync();

            var maasButceDetay = await _context.DepartmanButceDetays
                .FirstOrDefaultAsync(d => d.ButceId == aktifButce.ButceId && d.KategoriId == maasKategoriId);

            decimal toplamMaasButcesi = maasButceDetay?.AyrilanTutar ?? 0;

            // Departmandaki aktif personelin maaş toplamı (Müdürler hariç)
            var digerMaaslar = await _context.Maas
                .Include(m => m.Kullanici)
                .ThenInclude(k => k.Rol)
                .Where(m => m.Kullanici.DepartmanId == user.DepartmanId && m.BitisTarihi == null && m.Kullanici.Rol.RolAdi != "Mudur" && m.Kullanici.Rol.RolAdi != "Admin")
                .ToListAsync();

            decimal mevcutToplam = digerMaaslar
                .Where(m => m.MaasId != mevcutMaasId)
                .Sum(m => m.AylikTutar);

            if ((mevcutToplam + yeniMaasTutar) > toplamMaasButcesi)
            {
                decimal kalan = toplamMaasButcesi - mevcutToplam;
                return $"Bu işlem departmanın maaş bütçesini aşıyor! (Atanabilir maksimum tutar: {kalan:N2} ₺)";
            }

            return null; // Sorun yok
        }

        // GET: Maas/Create
        [Authorize(Roles = "Admin,Mudur")]
        public async Task<IActionResult> Create()
        {
            await PopulateKullaniciListAsync();
            await PopulateMaasButcesiBilgileriAsync();
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

                // Maaş Bütçesi Kontrolü
                var validationError = await ValidateMaasButcesiAsync(maas.KullaniciId, maas.AylikTutar);
                if (validationError != null)
                {
                    ModelState.AddModelError("", validationError);
                    await PopulateKullaniciListAsync(maas.KullaniciId);
                    await PopulateMaasButcesiBilgileriAsync();
                    return View(maas);
                }

                _context.Add(maas);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            await PopulateKullaniciListAsync(maas.KullaniciId);
            await PopulateMaasButcesiBilgileriAsync();
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
            await PopulateMaasButcesiBilgileriAsync(maas.Kullanici.DepartmanId);
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

                // Maaş Bütçesi Kontrolü
                var validationError = await ValidateMaasButcesiAsync(maas.KullaniciId, maas.AylikTutar, maas.MaasId);
                if (validationError != null)
                {
                    ModelState.AddModelError("", validationError);
                    await PopulateKullaniciListAsync(maas.KullaniciId);
                    await PopulateMaasButcesiBilgileriAsync();
                    return View(maas);
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
            await PopulateMaasButcesiBilgileriAsync(maas.Kullanici?.DepartmanId);
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
