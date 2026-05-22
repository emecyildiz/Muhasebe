using Muhasebe.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Muhasebe.Services.Interfaces;

namespace Muhasebe.Controllers
{
    public class DepartmanController : Controller
    {

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
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var departman = await _departmanService.GetDepartmanByIdAsync(id);
            if (departman == null)
            {
                return NotFound();
            }
            return View(departman);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Departman model)
        {
            if (ModelState.IsValid)
            {
                await _departmanService.UpdateDepartmanAsync(model);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var departman = await _departmanService.GetDepartmanByIdAsync(id);
            if (departman == null)
            {
                return NotFound();
            }
            return View(departman);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _departmanService.DeleteDepartmanAsync(id);
            return RedirectToAction("Index");

        }
    }
}
