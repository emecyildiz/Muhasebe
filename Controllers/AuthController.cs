using Microsoft.AspNetCore.Mvc;

namespace Muhasebe.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Index() => RedirectToAction(nameof(Login));

        public IActionResult Login() => View();

        public IActionResult Register() => View();
    }
}
