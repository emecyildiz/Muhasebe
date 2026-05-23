using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Muhasebe.Models.Entities;
using System.Security.Claims;

namespace Muhasebe.ViewComponents
{
    public class SidebarMenuViewComponent : ViewComponent
    {
        private readonly MuhasebeContext _context;
        public SidebarMenuViewComponent(MuhasebeContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var kullaniciRolu = ((ClaimsIdentity)HttpContext.User.Identity)?.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(kullaniciRolu))
            {
                return View(new List<Menu>());
            }

            if (kullaniciRolu.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                var tumMenuler = await _context.Menus
                    .Where(m => m.Durum == true)
                    .OrderBy(m => m.Sira)
                    .ToListAsync();
                return View(tumMenuler);
            }

            var izinVerilenMenuler = await _context.RolMenus
                .Include(rm => rm.Menu)
                .Include(rm => rm.Rol)
                .Where(rm => rm.Rol.RolAdi == kullaniciRolu && rm.Menu.Durum == true)
                .Select(rm => rm.Menu)
                .OrderBy(m => m.Sira) 
                .ToListAsync();

            return View(izinVerilenMenuler);
        }
    }
}
