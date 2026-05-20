using Microsoft.AspNetCore.Mvc;

namespace Muhasebe.Controllers
{
    public class DepartmanController : Controller
    {
        public IActionResult Index() => RedirectToAction(nameof(Personelim));

        public IActionResult Personelim() => View();
    }
}
