using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Muhasebe.Models.Entities;
using System.Security.Claims;

namespace Muhasebe.Controllers
{
    [Authorize]
    public class BildirimController : Controller
    {
        private readonly MuhasebeContext _context;

        public BildirimController(MuhasebeContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> MarkAsRead(int id)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var bildirim = await _context.Bildirims.FirstOrDefaultAsync(b => b.BildirimId == id && b.AliciId == userId);
            
            if (bildirim != null)
            {
                bildirim.Okundu = true;
                await _context.SaveChangesAsync();
                
                if (bildirim.TalepId.HasValue)
                {
                    return RedirectToAction("Detail", "MasrafTalebi", new { id = bildirim.TalepId.Value });
                }
            }

            return RedirectToAction("Dashboard", "Home");
        }

        public async Task<IActionResult> MarkAllAsRead()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var okunmamisBildirimler = await _context.Bildirims
                .Where(b => b.AliciId == userId && b.Okundu == false)
                .ToListAsync();

            if (okunmamisBildirimler.Any())
            {
                foreach (var b in okunmamisBildirimler)
                {
                    b.Okundu = true;
                }
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Dashboard", "Home");
        }
    }
}
