using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Muhasebe.Models;

namespace Muhasebe.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Dashboard");
            }
            return View();
        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult NotFound(int statusCode)
        {
            Response.StatusCode = StatusCodes.Status404NotFound;
            return View("RecordNotFound", new NotFoundViewModel
            {
                EntityName = "Sayfa veya kayıt",
                Message = "Aradığınız adres geçersiz olabilir ya da ilgili kayıt artık mevcut olmayabilir.",
                ListController = "Home",
                ListAction = "Dashboard",
                ListLabel = "Panele dön"
            });
        }
    }
}
