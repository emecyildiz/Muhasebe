using Microsoft.AspNetCore.Mvc;

namespace Muhasebe.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
