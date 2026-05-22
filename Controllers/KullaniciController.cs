using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Muhasebe.Models.Entities;
using Muhasebe.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Muhasebe.Controllers
{
    public class KullaniciController : Controller
    {

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
            var departmanlar = await _departmanService.GetAllDepartmanAsync();
            ViewBag.DepartmanId = new SelectList(departmanlar, "DepartmanId", "DepartmanAdi");

            var roller = await _context.Rols.ToListAsync();
            ViewBag.RolListesi = new SelectList(roller, "RolId", "RolAdi");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Kullanici model)
        {
            if (ModelState.IsValid)
            {
                await _kullaniciService.CreateKullaniciAsync(model);
                return RedirectToAction("Index");
            }

            var departmanlar = await _departmanService.GetAllDepartmanAsync();
            ViewBag.DepartmanId = new SelectList(departmanlar, "DepartmanId", "DepartmanAdi", model.DepartmanId);

            var roller = await _context.Rols.ToListAsync();
            ViewBag.RolListesi = new SelectList(roller, "RolId", "RolAdi", model.RolId);

            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var kullanici = await _kullaniciService.GetKullaniciByIdAsync(id);
            if (kullanici == null)
            {
                return NotFound();
            }
            var departmanlar = await _departmanService.GetAllDepartmanAsync();
            ViewBag.DepartmanId = new SelectList(departmanlar, "DepartmanId", "DepartmanAdi", kullanici.DepartmanId);
            var roller = await _context.Rols.ToListAsync();
            ViewBag.RolListesi = new SelectList(roller, "RolId", "RolAdi", kullanici.RolId);
            return View(kullanici);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Kullanici model)
        {
            if (ModelState.IsValid)
            {
                await _kullaniciService.UpdateKullaniciAsync(model);
                return RedirectToAction("Index");
            }
            var departmanlar = await _departmanService.GetAllDepartmanAsync();
            ViewBag.DepartmanId = new SelectList(departmanlar, "DepartmanId", "DepartmanAdi", model.DepartmanId);
            var roller = await _context.Rols.ToListAsync();
            ViewBag.RolListesi = new SelectList(roller, "RolId", "RolAdi", model.RolId);
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var kullanici = await _kullaniciService.GetKullaniciByIdAsync(id);
            if (kullanici == null)
            {
                return NotFound();
            }
            return View(kullanici);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _kullaniciService.DeleteKullaniciAsync(id);
            return RedirectToAction("Index");

        }
    }

}
