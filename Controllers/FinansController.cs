using Microsoft.AspNetCore.Mvc;

namespace Muhasebe.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class FinansController : Controller
    {
        public IActionResult Index() => RedirectToAction(nameof(AnaKasa));

        public IActionResult AnaKasa() => View();
    }
}
