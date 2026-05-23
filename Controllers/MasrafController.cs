using Microsoft.AspNetCore.Mvc;

namespace Muhasebe.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class MasrafController : Controller
    {
        public IActionResult Index() => RedirectToAction("Index", "MasrafTalebi");

        public IActionResult Taleplerim() => RedirectToAction("Index", "MasrafTalebi");
    }
}
