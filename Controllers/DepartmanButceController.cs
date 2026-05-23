using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Muhasebe.Models.Entities;
using Muhasebe.Services.Interfaces;
using Muhasebe.ViewModels.DepartmanButce;
using Muhasebe.Common.Helpers;

namespace Muhasebe.Controllers
{
    [Authorize(Roles = "Admin,Mudur")]
    public class DepartmanButceController : Controller
    {
        private const string EntityName = "Departman bütçesi";
        private readonly MuhasebeContext _context;
        private readonly IDepartmanButceService _butceService;
        private readonly IDepartmanService _departmanService;

        public DepartmanButceController(MuhasebeContext context, IDepartmanButceService butceService, IDepartmanService departmanService)
        {
            _context = context;
            _butceService = butceService;
            _departmanService = departmanService;
        }

        public async Task<IActionResult> Index()
        {
            string aktifRol = User.FindFirstValue(ClaimTypes.Role);
            var query = _context.DepartmanButces
                                .Include(b => b.Departman)
                                .AsQueryable();

            if (aktifRol == "Mudur")
            {
                var claimDept = User.FindFirstValue("DepartmanId");
                if (int.TryParse(claimDept, out int depId))
                {
                    query = query.Where(b => b.DepartmanId == depId);
                }
            }

            var butceler = await query.ToListAsync();
            
            // ViewBag'e dashboard için toplam değerleri ekleyelim (özellikle müdür için)
            if (aktifRol == "Mudur")
            {
                ViewBag.ToplamAnaButce = butceler.Sum(b => b.AyrilanButce);
                ViewBag.DagitilanTutar = butceler.Sum(b => b.KullanilanButce ?? 0);
                ViewBag.KalanButce = ViewBag.ToplamAnaButce - ViewBag.DagitilanTutar;
            }

            return View(butceler);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var departman = await _departmanService.GetDepartmanByIdAsync(id);
            if (departman == null)
            {
                return this.RecordNotFound("Departman", "Departman", requestedId: id, listLabel: "Departman listesine dön",
                    message: "Bu departmana ait bütçe geçmişi görüntülenemiyor çünkü departman kaydı bulunamadı.");
            }

            var butce = await _butceService.GetDetailByIdAsync(id);
            ViewBag.DepartmanAdi = departman.DepartmanAdi;
            return View(butce ?? new List<DepartmanButce>());
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            await PopulateDepartmanSelectAsync();
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("DepartmanId,Yil,Ay,AyrilanButce")] DepartmanButce model)
        {
            ModelState.Remove("Departman");
            ModelState.Remove("DepartmanButceDetays");
            
            if (ModelState.IsValid)
            {
                model.KullanilanButce = 0;
                await _butceService.CreateAsync(model);
                return RedirectToAction(nameof(Index));
            }
            await PopulateDepartmanSelectAsync(model.DepartmanId);
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var butce = await _butceService.GetByIdAsync(id);
            if (butce == null)
            {
                return this.RecordNotFound(EntityName, "DepartmanButce", requestedId: id, listLabel: "Bütçe listesine dön");
            }
            await PopulateDepartmanSelectAsync(butce.DepartmanId);
            return View(butce);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("ButceId,DepartmanId,Yil,Ay,AyrilanButce")] DepartmanButce model)
        {
            if (id != model.ButceId)
            {
                return this.RecordNotFound(EntityName, "DepartmanButce", requestedId: id, listLabel: "Bütçe listesine dön");
            }

            var existing = await _butceService.GetByIdAsync(id);
            if (existing == null)
            {
                return this.RecordNotFound(EntityName, "DepartmanButce", requestedId: id, listLabel: "Bütçe listesine dön");
            }

            ModelState.Remove("Departman");
            ModelState.Remove("DepartmanButceDetays");
            ModelState.Remove("KullanilanButce");

            if (ModelState.IsValid)
            {
                model.KullanilanButce = existing.KullanilanButce;
                await _butceService.UpdateAsync(id, model);
                return RedirectToAction(nameof(Index));
            }
            await PopulateDepartmanSelectAsync(model.DepartmanId);
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var butce = await _butceService.GetByIdAsync(id);
            if (butce == null)
            {
                return this.RecordNotFound(EntityName, "DepartmanButce", requestedId: id, listLabel: "Bütçe listesine dön");
            }
            return View(butce);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var butce = await _butceService.GetByIdAsync(id);
            if (butce == null)
            {
                return this.RecordNotFound(EntityName, "DepartmanButce", requestedId: id, listLabel: "Bütçe listesine dön");
            }

            await _butceService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
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

        [HttpGet]
        [Authorize(Roles = "Admin,Mudur")]
        public async Task<IActionResult> Allocate(int id)
        {
            var butce = await _context.DepartmanButces
                                      .Include(b => b.Departman)
                                      .FirstOrDefaultAsync(b => b.ButceId == id);
                                      
            if (butce == null) return NotFound();

            var claimDept = User.FindFirstValue("DepartmanId");
            string aktifRol = User.FindFirstValue(ClaimTypes.Role);

            if (aktifRol == "Mudur" && butce.DepartmanId.ToString() != claimDept)
            {
                return Unauthorized();
            }

            int maasKategoriId = await GetMaasKategoriIdAsync();
            var kategoriler = await _context.IslemKategorisis.Where(k => k.KategoriId != maasKategoriId).ToListAsync();
            var mevcutDetaylar = await _context.DepartmanButceDetays
                                               .Where(d => d.ButceId == id)
                                               .ToListAsync();

            var vm = new AllocateViewModel
            {
                ButceId = butce.ButceId,
                DepartmanAdi = butce.Departman?.DepartmanAdi ?? "",
                ToplamButce = butce.AyrilanButce,
                ToplamDagitilan = mevcutDetaylar.Where(d => d.KategoriId != maasKategoriId).Sum(d => d.AyrilanTutar),
                MaasButcesi = mevcutDetaylar.FirstOrDefault(d => d.KategoriId == maasKategoriId)?.AyrilanTutar ?? 0,
                Allocations = kategoriler.Select(k => new CategoryAllocationViewModel
                {
                    KategoriId = k.KategoriId,
                    KategoriAdi = k.KategoriAdi,
                    AyrilanTutar = mevcutDetaylar.FirstOrDefault(d => d.KategoriId == k.KategoriId)?.AyrilanTutar ?? 0
                }).ToList()
            };

            return View(vm);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Mudur")]
        public async Task<IActionResult> Allocate(AllocateViewModel model)
        {
            var butce = await _context.DepartmanButces.FirstOrDefaultAsync(b => b.ButceId == model.ButceId);
            if (butce == null) return NotFound();

            var claimDept = User.FindFirstValue("DepartmanId");
            string aktifRol = User.FindFirstValue(ClaimTypes.Role);

            if (aktifRol == "Mudur" && butce.DepartmanId.ToString() != claimDept)
            {
                return Unauthorized();
            }

            decimal toplamDagitilan = model.Allocations.Sum(a => a.AyrilanTutar) + model.MaasButcesi;
            if (toplamDagitilan > butce.AyrilanButce)
            {
                ModelState.AddModelError("", "Dağıtılan toplam tutar (Maaş Bütçesi dahil), departmanın ana bütçesini aşamaz!");
                // Kategorileri tekrar doldurmamız gerekir ki form düzgün yüklensin
                int mId = await GetMaasKategoriIdAsync();
                var kategoriler = await _context.IslemKategorisis.Where(k => k.KategoriId != mId).ToListAsync();
                foreach (var allocation in model.Allocations)
                {
                    allocation.KategoriAdi = kategoriler.FirstOrDefault(k => k.KategoriId == allocation.KategoriId)?.KategoriAdi ?? "";
                }
                return View(model);
            }

            int maasKategoriId = await GetMaasKategoriIdAsync();

            // Kategori Bütçeleri
            foreach (var allocation in model.Allocations)
            {
                var detay = await _context.DepartmanButceDetays
                    .FirstOrDefaultAsync(d => d.ButceId == model.ButceId && d.KategoriId == allocation.KategoriId);

                if (detay != null)
                {
                    detay.AyrilanTutar = allocation.AyrilanTutar;
                }
                else
                {
                    if (allocation.AyrilanTutar > 0)
                    {
                        _context.DepartmanButceDetays.Add(new DepartmanButceDetay
                        {
                            ButceId = model.ButceId,
                            KategoriId = allocation.KategoriId,
                            AyrilanTutar = allocation.AyrilanTutar,
                            KullanilanTutar = 0
                        });
                    }
                }
            }

            // Maaş Bütçesi
            var maasDetay = await _context.DepartmanButceDetays
                .FirstOrDefaultAsync(d => d.ButceId == model.ButceId && d.KategoriId == maasKategoriId);

            if (maasDetay != null)
            {
                maasDetay.AyrilanTutar = model.MaasButcesi;
            }
            else
            {
                if (model.MaasButcesi > 0)
                {
                    _context.DepartmanButceDetays.Add(new DepartmanButceDetay
                    {
                        ButceId = model.ButceId,
                        KategoriId = maasKategoriId,
                        AyrilanTutar = model.MaasButcesi,
                        KullanilanTutar = 0
                    });
                }
            }

            butce.KullanilanButce = toplamDagitilan;
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        private async Task PopulateDepartmanSelectAsync(int? selectedId = null)
        {
            var departmanlar = await _departmanService.GetAllDepartmanAsync();
            ViewBag.DepartmanId = new SelectList(departmanlar, "DepartmanId", "DepartmanAdi", selectedId);
        }
    }
}
