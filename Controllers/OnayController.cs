using Microsoft.AspNetCore.Mvc;

namespace Muhasebe.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class OnayController : Controller
    {
        public IActionResult Index() => RedirectToAction(nameof(BekleyenTalepler));

        public IActionResult BekleyenTalepler() => View();
    }
}
