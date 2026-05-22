using Muhasebe.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Muhasebe.Services.Interfaces;
using Muhasebe.Common.Helpers;

namespace Muhasebe.Controllers
{
    public class DepartmanController : Controller
    {
        private const string EntityName = "Departman";
        private readonly IDepartmanService _departmanService;

        public DepartmanController(IDepartmanService departmanService)
        {
            _departmanService = departmanService;
        }

        public async Task<IActionResult> Index()
        {
            var departmanlar = await _departmanService.GetAllDepartmanAsync();
            return View(departmanlar);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Departman model)
        {
            if (ModelState.IsValid)
            {
                await _departmanService.CreateDepartmanAsync(model);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var departman = await _departmanService.GetDepartmanByIdAsync(id);
            if (departman == null)
            {
                return this.RecordNotFound(EntityName, "Departman", requestedId: id, listLabel: "Departman listesine dön");
            }
            return View(departman);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Departman model)
        {
            if (id != model.DepartmanId)
            {
                return this.RecordNotFound(EntityName, "Departman", requestedId: id, listLabel: "Departman listesine dön");
            }

            var existing = await _departmanService.GetDepartmanByIdAsync(id);
            if (existing == null)
            {
                return this.RecordNotFound(EntityName, "Departman", requestedId: id, listLabel: "Departman listesine dön");
            }

            if (ModelState.IsValid)
            {
                await _departmanService.UpdateDepartmanAsync(model);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var departman = await _departmanService.GetDepartmanByIdAsync(id);
            if (departman == null)
            {
                return this.RecordNotFound(EntityName, "Departman", requestedId: id, listLabel: "Departman listesine dön");
            }
            return View(departman);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var departman = await _departmanService.GetDepartmanByIdAsync(id);
            if (departman == null)
            {
                return this.RecordNotFound(EntityName, "Departman", requestedId: id, listLabel: "Departman listesine dön");
            }

            await _departmanService.DeleteDepartmanAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Detail(int id)
        {
            var departman = await _departmanService.GetDepartmanDetailAsync(id);
            if (departman == null)
            {
                return this.RecordNotFound(EntityName, "Departman", requestedId: id, listLabel: "Departman listesine dön");
            }

            return View(departman);
        }
    }
}
