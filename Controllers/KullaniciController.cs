using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Muhasebe.Models.Entities;
using Muhasebe.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Muhasebe.Common.Helpers;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Muhasebe.Controllers
{
    [Authorize(Roles = "Admin,Mudur")]
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
            string aktifRol = User.FindFirstValue(ClaimTypes.Role);
            int? departmanId = null;

            if (aktifRol == "Mudur")
            {
                var claimDept = User.FindFirstValue("DepartmanId");
                if (int.TryParse(claimDept, out int depId))
                {
                    departmanId = depId;
                }
            }

            var kullanicilar = await _kullaniciService.GetAllKullaniciAsync(departmanId);
            return View(kullanicilar);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            await PopulateSelectListsAsync();
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Kullanici model)
        {
            ModelState.Remove("Departman");
            ModelState.Remove("Rol");

            if (await _context.Kullanicis.AnyAsync(k => k.Eposta == model.Eposta))
            {
                ModelState.AddModelError("Eposta", "Bu e-posta adresi zaten sistemde kayıtlı.");
            }

            if (ModelState.IsValid)
            {
                await _kullaniciService.CreateKullaniciAsync(model);
                return RedirectToAction(nameof(Index));
            }

            await PopulateSelectListsAsync(model.DepartmanId, model.RolId);
            return View(model);
        }

        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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

            ModelState.Remove("Departman");
            ModelState.Remove("Rol");

            if (await _context.Kullanicis.AnyAsync(k => k.Eposta == model.Eposta && k.KullaniciId != id))
            {
                ModelState.AddModelError("Eposta", "Bu e-posta adresi başka bir personel tarafından kullanılıyor.");
            }

            if (ModelState.IsValid)
            {
                await _kullaniciService.UpdateKullaniciAsync(model);
                return RedirectToAction(nameof(Index));
            }

            await PopulateSelectListsAsync(model.DepartmanId, model.RolId);
            return View(model);
        }

        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
            var kullanici = await _kullaniciService.GetKullaniciDetailAsync(id);
            if (kullanici == null)
            {
                return this.RecordNotFound(EntityName, "Kullanici", requestedId: id, listLabel: "Kullanıcı listesine dön");
            }

            string aktifRol = User.FindFirstValue(ClaimTypes.Role);
            int aktifDepartmanId = int.Parse(User.FindFirstValue("DepartmanId") ?? "0");

            if (aktifRol == "Mudur" && kullanici.DepartmanId != aktifDepartmanId)
            {
                return RedirectToAction("AccessDenied", "Auth");
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

        [HttpGet]
        public async Task<IActionResult> KullaniciAra(string q)
        {
            var query = _context.Kullanicis.AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(k => (k.Ad + " " + k.Soyad).Contains(q));
            }

            var result = await query
                .Select(k => new { id = k.KullaniciId, text = k.Ad + " " + k.Soyad })
                .Take(20)
                .ToListAsync();

            return Json(result);
        }
    }
}
