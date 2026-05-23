using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Muhasebe.Models.Entities;
using System.Security.Claims;

namespace Muhasebe.ViewComponents
{
    public class BildirimlerViewComponent : ViewComponent
    {
        private readonly MuhasebeContext _context;

        public BildirimlerViewComponent(MuhasebeContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var userIdClaim = ((ClaimsPrincipal)User).FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    var bildirimler = await _context.Bildirims
                        .Where(b => b.AliciId == userId && b.Okundu == false)
                        .OrderByDescending(b => b.OlusturulmaTarihi)
                        .Take(5)
                        .ToListAsync();

                    return View(bildirimler);
                }
            }

            return View(new List<Bildirim>());
        }
    }
}
