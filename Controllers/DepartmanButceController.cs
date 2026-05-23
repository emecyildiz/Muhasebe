using Microsoft.AspNetCore.Mvc;
using Muhasebe.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using Muhasebe.Models.Entities;
using Muhasebe.Common.Helpers;

namespace Muhasebe.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class DepartmanButceController : Controller
    {
        private const string EntityName = "Departman bütçesi";
        private readonly IDepartmanButceService _departmanButceService;
        private readonly IDepartmanService _departmanService;

        public DepartmanButceController(IDepartmanButceService departmanButceService, IDepartmanService departmanService)
        {
            _departmanButceService = departmanButceService;
            _departmanService = departmanService;
        }

        public async Task<IActionResult> Index()
        {
            var butceler = await _departmanButceService.GetAllAsync();
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

            var butce = await _departmanButceService.GetDetailByIdAsync(id);
            ViewBag.DepartmanAdi = departman.DepartmanAdi;
            return View(butce ?? new List<DepartmanButce>());
        }

        public async Task<IActionResult> Create()
        {
            await PopulateDepartmanSelectAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(DepartmanButce model)
        {
            if (ModelState.IsValid)
            {
                await _departmanButceService.CreateAsync(model);
                return RedirectToAction(nameof(Index));
            }
            await PopulateDepartmanSelectAsync(model.DepartmanId);
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var butce = await _departmanButceService.GetByIdAsync(id);
            if (butce == null)
            {
                return this.RecordNotFound(EntityName, "DepartmanButce", requestedId: id, listLabel: "Bütçe listesine dön");
            }
            await PopulateDepartmanSelectAsync(butce.DepartmanId);
            return View(butce);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, DepartmanButce model)
        {
            if (id != model.ButceId)
            {
                return this.RecordNotFound(EntityName, "DepartmanButce", requestedId: id, listLabel: "Bütçe listesine dön");
            }

            var existing = await _departmanButceService.GetByIdAsync(id);
            if (existing == null)
            {
                return this.RecordNotFound(EntityName, "DepartmanButce", requestedId: id, listLabel: "Bütçe listesine dön");
            }

            if (ModelState.IsValid)
            {
                await _departmanButceService.UpdateAsync(id, model);
                return RedirectToAction(nameof(Index));
            }
            await PopulateDepartmanSelectAsync(model.DepartmanId);
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var butce = await _departmanButceService.GetByIdAsync(id);
            if (butce == null)
            {
                return this.RecordNotFound(EntityName, "DepartmanButce", requestedId: id, listLabel: "Bütçe listesine dön");
            }
            return View(butce);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var butce = await _departmanButceService.GetByIdAsync(id);
            if (butce == null)
            {
                return this.RecordNotFound(EntityName, "DepartmanButce", requestedId: id, listLabel: "Bütçe listesine dön");
            }

            await _departmanButceService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateDepartmanSelectAsync(int? selectedId = null)
        {
            var departmanlar = await _departmanService.GetAllDepartmanAsync();
            ViewBag.DepartmanId = new SelectList(departmanlar, "DepartmanId", "DepartmanAdi", selectedId);
        }
    }
}
