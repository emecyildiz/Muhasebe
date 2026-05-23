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
            var maaslar = _context.Maas.Include(m => m.Kullanici).ThenInclude(k => k.Rol).AsQueryable();
            var masrafTalepleri = _context.MasrafTalebis.AsQueryable();

            if (userRole == "Mudur" && aktifDepartmanId.HasValue)
            {
                finansalIslemler = finansalIslemler.Where(f => f.DepartmanId == aktifDepartmanId.Value || (f.Kullanici != null && f.Kullanici.DepartmanId == aktifDepartmanId.Value));
                maaslar = maaslar.Where(m => m.Kullanici.DepartmanId == aktifDepartmanId.Value && m.Kullanici.Rol.RolAdi != "Mudur" && m.Kullanici.Rol.RolAdi != "Admin");
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
            var maasYuku = maaslar.Where(m => m.BitisTarihi == null).Sum(m => m.AylikTutar);
            
            var bekliyor = Muhasebe.Common.Enums.DurumEnum.Bekliyor.ToString();
            var onaylandi = Muhasebe.Common.Enums.DurumEnum.Onaylandi.ToString();
            
            var bekleyenTalep = masrafTalepleri.Where(t => t.Durum == bekliyor).Sum(t => t.Tutar);
            var onaylananTalep = masrafTalepleri.Where(t => t.Durum == onaylandi).Sum(t => t.Tutar);

            var model = new Muhasebe.ViewModels.DashboardViewModel
            {
                ToplamGelir = gelir, // Bütçe sonra eklenecek
                ToplamGider = gider + maasYuku + onaylananTalep,
                ToplamMaasYuku = maasYuku,
                BekleyenMasrafTalebi = bekleyenTalep,
                ButceDetaylari = new List<Muhasebe.ViewModels.DepartmanButce.CategoryAllocationViewModel>()
            };

            // Son Finansal İşlemler ve Talepler listesi birleştirilerek en son 5 işlem alınacak
            var sonFinansalIslemler = finansalIslemler
                .OrderByDescending(f => f.IslemTarihi)
                .Take(5)
                .Select(f => new Muhasebe.ViewModels.SonIslemViewModel
                {
                    IslemTuru = "Finansal İşlem",
                    Aciklama = string.IsNullOrEmpty(f.Aciklama) ? "Açıklama yok" : f.Aciklama,
                    Tutar = f.Tutar,
                    Tarih = f.IslemTarihi ?? DateTime.Now,
                    Durum = f.IslemTuru,
                    CSSRenk = f.IslemTuru == "Gelir" ? "success" : "danger"
                }).ToList();

            var sonTalepler = masrafTalepleri
                .OrderByDescending(t => t.TalepTarihi)
                .Take(5)
                .Select(t => new Muhasebe.ViewModels.SonIslemViewModel
                {
                    IslemTuru = "Masraf Talebi",
                    Aciklama = string.IsNullOrEmpty(t.Aciklama) ? "Masraf talebi" : t.Aciklama,
                    Tutar = t.Tutar,
                    Tarih = t.TalepTarihi,
                    Durum = t.Durum,
                    CSSRenk = t.Durum == onaylandi ? "success" : (t.Durum == bekliyor ? "warning" : "danger")
                }).ToList();

            model.SonIslemler = sonFinansalIslemler.Concat(sonTalepler)
                .OrderByDescending(x => x.Tarih)
                .Take(7)
                .ToList();

            // Aktif Bütçe Bilgileri (Müdür için kendi departmanı, Admin için tüm departmanlar toplamı vs.)
            if (aktifDepartmanId.HasValue)
            {
                var aktifButce = _context.DepartmanButces
                    .Where(b => b.DepartmanId == aktifDepartmanId.Value)
                    .OrderByDescending(b => b.Yil)
                    .ThenByDescending(b => b.Ay)
                    .FirstOrDefault();

                if (aktifButce != null)
                {
                    model.ToplamAktifButce = aktifButce.AyrilanButce;
                    model.ToplamGelir += aktifButce.AyrilanButce; // Ana bütçeyi de departmanın gelirine dahil et
                    model.KalanAktifButce = aktifButce.AyrilanButce - (aktifButce.KullanilanButce ?? 0);

                    var detaylar = _context.DepartmanButceDetays
                        .Where(d => d.ButceId == aktifButce.ButceId && d.AyrilanTutar > 0)
                        .ToList();

                    var kategoriler = _context.IslemKategorisis.ToList();

                    foreach (var d in detaylar)
                    {
                        var kategori = kategoriler.FirstOrDefault(k => k.KategoriId == d.KategoriId);
                        string kAdi = kategori?.KategoriAdi ?? "Bilinmiyor";

                        model.ButceDetaylari.Add(new Muhasebe.ViewModels.DepartmanButce.CategoryAllocationViewModel
                        {
                            KategoriId = d.KategoriId,
                            KategoriAdi = kAdi,
                            AyrilanTutar = d.AyrilanTutar
                        });
                    }
                }
            }

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
