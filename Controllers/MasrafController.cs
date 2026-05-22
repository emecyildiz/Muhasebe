using Microsoft.AspNetCore.Mvc;

namespace Muhasebe.Controllers
{
    public class MasrafController : Controller
    {
        public IActionResult Index() => RedirectToAction("Index", "MasrafTalebi");

        public IActionResult Taleplerim() => RedirectToAction("Index", "MasrafTalebi");
    }
}
