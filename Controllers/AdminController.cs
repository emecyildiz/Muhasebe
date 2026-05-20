using Microsoft.AspNetCore.Mvc;

namespace Muhasebe.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
