using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Muhasebe.Models.Entities;
using Muhasebe.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Muhasebe.Common.Helpers;

namespace Muhasebe.Controllers
{
    public class KullaniciController : Controller
    {
        private const string EntityName = "Kullanıcı";
        private readonly IKullaniciService _kullaniciService;
        private readonly IDepartmanService _departmanService;
        private readonly MuhasebeContext _context;

        public KullaniciController(IKullaniciService kullaniciService, IDepartmanService departmanService, MuhasebeContext context)
        {
            _kullaniciService = kullaniciService;
            _departmanService = departmanService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var kullanicilar = await _kullaniciService.GetAllKullaniciAsync();
            return View(kullanicilar);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateSelectListsAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Kullanici model)
        {
            if (ModelState.IsValid)
            {
                await _kullaniciService.CreateKullaniciAsync(model);
                return RedirectToAction(nameof(Index));
            }

            await PopulateSelectListsAsync(model.DepartmanId, model.RolId);
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var kullanici = await _kullaniciService.GetKullaniciByIdAsync(id);
            if (kullanici == null)
            {
                return this.RecordNotFound(EntityName, "Kullanici", requestedId: id, listLabel: "Kullanıcı listesine dön");
            }
            await PopulateSelectListsAsync(kullanici.DepartmanId, kullanici.RolId);
            return View(kullanici);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Kullanici model)
        {
            if (id != model.KullaniciId)
            {
                return this.RecordNotFound(EntityName, "Kullanici", requestedId: id, listLabel: "Kullanıcı listesine dön");
            }

            var existing = await _kullaniciService.GetKullaniciByIdAsync(id);
            if (existing == null)
            {
                return this.RecordNotFound(EntityName, "Kullanici", requestedId: id, listLabel: "Kullanıcı listesine dön");
            }

            if (ModelState.IsValid)
            {
                await _kullaniciService.UpdateKullaniciAsync(model);
                return RedirectToAction(nameof(Index));
            }

            await PopulateSelectListsAsync(model.DepartmanId, model.RolId);
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var kullanici = await _kullaniciService.GetKullaniciByIdAsync(id);
            if (kullanici == null)
            {
                return this.RecordNotFound(EntityName, "Kullanici", requestedId: id, listLabel: "Kullanıcı listesine dön");
            }
            return View(kullanici);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var kullanici = await _kullaniciService.GetKullaniciByIdAsync(id);
            if (kullanici == null)
            {
                return this.RecordNotFound(EntityName, "Kullanici", requestedId: id, listLabel: "Kullanıcı listesine dön");
            }

            await _kullaniciService.DeleteKullaniciAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Detail(int id)
        {
            var kullanici = await _kullaniciService.GetKullaniciByIdAsync(id);
            if (kullanici == null)
            {
                return this.RecordNotFound(EntityName, "Kullanici", requestedId: id, listLabel: "Kullanıcı listesine dön");
            }
            return View(kullanici);
        }

        private async Task PopulateSelectListsAsync(int? departmanId = null, int? rolId = null)
        {
            var departmanlar = await _departmanService.GetAllDepartmanAsync();
            ViewBag.DepartmanId = new SelectList(departmanlar, "DepartmanId", "DepartmanAdi", departmanId);

            var roller = await _context.Rols.ToListAsync();
            ViewBag.RolListesi = new SelectList(roller, "RolId", "RolAdi", rolId);
        }
    }
}
