using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Muhasebe.Models;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace Muhasebe.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Muhasebe.Models.Entities.MuhasebeContext _context;

        public HomeController(ILogger<HomeController> logger, Muhasebe.Models.Entities.MuhasebeContext context)
        {
            _logger = logger;
            _context = context;
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
            var userRole = User.FindFirstValue(System.Security.Claims.ClaimTypes.Role);
            var aktifKullaniciId = int.Parse(User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier));
            
            var departmanClaim = User.FindFirstValue("DepartmanId");
            int? aktifDepartmanId = string.IsNullOrEmpty(departmanClaim) ? null : int.Parse(departmanClaim);

            var finansalIslemler = _context.FinansalIslems.Include(f => f.Kullanici).AsQueryable();
            var maaslar = _context.Maas.Include(m => m.Kullanici).AsQueryable();
            var masrafTalepleri = _context.MasrafTalebis.AsQueryable();

            if (userRole == "Mudur" && aktifDepartmanId.HasValue)
            {
                finansalIslemler = finansalIslemler.Where(f => f.DepartmanId == aktifDepartmanId.Value || (f.Kullanici != null && f.Kullanici.DepartmanId == aktifDepartmanId.Value));
                maaslar = maaslar.Where(m => m.Kullanici.DepartmanId == aktifDepartmanId.Value);
                masrafTalepleri = masrafTalepleri.Where(t => t.DepartmanId == aktifDepartmanId.Value);
            }
            else if (userRole != "Admin")
            {
                // Çalışanlar sadece kendilerini ilgilendiren bilgileri görür
                finansalIslemler = finansalIslemler.Where(f => f.KullaniciId == aktifKullaniciId);
                maaslar = maaslar.Where(m => m.KullaniciId == aktifKullaniciId);
                masrafTalepleri = masrafTalepleri.Where(t => t.TalepEdenKullaniciId == aktifKullaniciId);
            }

            var gelir = finansalIslemler.Where(f => f.IslemTuru == "Gelir").Sum(f => f.Tutar);
            var gider = finansalIslemler.Where(f => f.IslemTuru == "Gider").Sum(f => f.Tutar);
            var maasYuku = maaslar.Sum(m => m.AylikTutar);
            
            var bekliyor = Muhasebe.Common.Enums.DurumEnum.Bekliyor.ToString();
            var bekleyenTalep = masrafTalepleri.Where(t => t.Durum == bekliyor).Sum(t => t.Tutar);

            var model = new Muhasebe.ViewModels.DashboardViewModel
            {
                ToplamGelir = gelir,
                ToplamGider = gider,
                ToplamMaasYuku = maasYuku,
                BekleyenMasrafTalebi = bekleyenTalep
            };

            return View(model);
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
