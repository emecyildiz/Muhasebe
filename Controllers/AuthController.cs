using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Muhasebe.Common.Helpers;
using Muhasebe.Models.Entities;
using Muhasebe.ViewModels;
using System.Security.Claims;

namespace Muhasebe.Controllers
{
    public class AuthController : Controller
    {
        private readonly MuhasebeContext _context;

        public AuthController(MuhasebeContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Dashboard", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var kullanici = await _context.Kullanicis
                    .Include(k => k.Rol)
                    .FirstOrDefaultAsync(k => k.Eposta == model.Eposta && k.Durum == true);
                if (kullanici != null && SifrelemeHelper.SifreKontrol(model.Sifre, kullanici.SifreHash))
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, kullanici.KullaniciId.ToString()),
                        new Claim(ClaimTypes.Name, $"{kullanici.Ad} {kullanici.Soyad}"),
                        new Claim(ClaimTypes.Email, kullanici.Eposta),
                        new Claim(ClaimTypes.Role, kullanici.Rol.RolAdi),
                        new Claim("DepartmanId", kullanici.DepartmanId.ToString())
                    };
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
                    };
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity), authProperties);
                    return RedirectToAction("Dashboard", "Home");
                }
                ModelState.AddModelError(string.Empty, "Geçersiz e-posta veya şifre.");
            }
            return View(model);
        }


        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Auth");
        }
         public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
