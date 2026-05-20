using Microsoft.AspNetCore.Mvc;

namespace Muhasebe.Controllers
{
    public class MasrafController : Controller
    {
        public IActionResult Index() => RedirectToAction(nameof(Taleplerim));

        public IActionResult Taleplerim() => View();
    }
}
