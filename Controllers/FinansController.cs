using Microsoft.AspNetCore.Mvc;

namespace Muhasebe.Controllers
{
    public class FinansController : Controller
    {
        public IActionResult Index() => RedirectToAction(nameof(AnaKasa));

        public IActionResult AnaKasa() => View();
    }
}
