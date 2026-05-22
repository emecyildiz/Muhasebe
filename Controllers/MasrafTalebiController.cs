using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Muhasebe.Common.Enums;
using Muhasebe.Common.Helpers;
using Muhasebe.Models.Entities;
using Muhasebe.Services.Interfaces;
using Muhasebe.ViewModels;

namespace Muhasebe.Controllers
{
    public class MasrafTalebiController : Controller
    {
        private const string EntityName = "Masraf talebi";
        private readonly IMasrafTalebiService _masrafTalebiService;
        private readonly MuhasebeContext _context;

        public MasrafTalebiController(IMasrafTalebiService masrafTalebiService, MuhasebeContext context)
        {
            _masrafTalebiService = masrafTalebiService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var talepler = await _masrafTalebiService.GetTumTaleplerAsync();
            return View(talepler);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateKategorilerAsync();
            return View(new MasrafTalebiCreateViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(MasrafTalebiCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                int aktifKullaniciId = 1;
                int aktifDepartmanId = 2;

                await _masrafTalebiService.EkleAsync(model, aktifKullaniciId, aktifDepartmanId);
                return RedirectToAction(nameof(Index));
            }

            await PopulateKategorilerAsync(model.KategoriId);
            return View(model);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var talep = await _context.MasrafTalebis
                .Include(m => m.TalepEdenKullanici)
                .Include(m => m.Departman)
                .Include(m => m.Kategori)
                .FirstOrDefaultAsync(m => m.TalepId == id);

            if (talep == null)
            {
                return this.RecordNotFound(EntityName, "MasrafTalebi", requestedId: id, listLabel: "Talep listesine dön");
            }

            ViewBag.DurumListesi = new SelectList(new[]
            {
                new { Id = (int)DurumEnum.Bekliyor, Ad = "Bekliyor" },
                new { Id = (int)DurumEnum.Onaylandi, Ad = "Onaylandı" },
                new { Id = (int)DurumEnum.Reddedildi, Ad = "Reddedildi" },
                new { Id = (int)DurumEnum.IptalEdildi, Ad = "İptal Edildi" }
            }, "Id", "Ad", talep.Durum);

            return View(talep);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateDurum(int talepId, int durumId)
        {
            var talep = await _masrafTalebiService.GetTalepByIdAsync(talepId);
            if (talep == null)
            {
                return this.RecordNotFound(EntityName, "MasrafTalebi", requestedId: talepId, listLabel: "Talep listesine dön");
            }

            int onaylayanId = 5;
            await _masrafTalebiService.DurumGuncelleAsync(talepId, durumId, onaylayanId);
            return RedirectToAction(nameof(Detail), new { id = talepId });
        }

        private async Task PopulateKategorilerAsync(int? selectedId = null)
        {
            var kategoriler = await _context.IslemKategorisis.OrderBy(k => k.KategoriAdi).ToListAsync();
            ViewBag.Kategoriler = new SelectList(kategoriler, "KategoriId", "KategoriAdi", selectedId);
        }
    }
}
