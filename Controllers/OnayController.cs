using Microsoft.AspNetCore.Mvc;

namespace Muhasebe.Controllers
{
    public class OnayController : Controller
    {
        public IActionResult Index() => RedirectToAction(nameof(BekleyenTalepler));

        public IActionResult BekleyenTalepler() => View();
    }
}
