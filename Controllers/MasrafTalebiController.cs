using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Muhasebe.Common.Enums;
using Muhasebe.Common.Helpers;
using Muhasebe.Models.Entities;
using Muhasebe.Services.Interfaces;
using Muhasebe.ViewModels;
using System.Security.Claims;

namespace Muhasebe.Controllers
{
    [Authorize]
    public class MasrafTalebiController : Controller
    {
        private const string EntityName = "Masraf talebi";
        private readonly IMasrafTalebiService _masrafTalebiService;
        private readonly MuhasebeContext _context;

        public MasrafTalebiController(IMasrafTalebiService masrafTalebiService, MuhasebeContext context)
        {
            _masrafTalebiService = masrafTalebiService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            
            List<MasrafTalebi> talepler;

            if (userRole == "Admin")
            {
                talepler = await _masrafTalebiService.GetTumTaleplerAsync();
            }
            else if (userRole == "Mudur")
            {
                var departmanId = int.Parse(User.FindFirstValue("DepartmanId"));
                talepler = await _masrafTalebiService.GetTaleplerByDepartmanIdAsync(departmanId);
            }
            else
            {
                talepler = await _masrafTalebiService.GetTaleplerByPersonelIdAsync(userId);
            }

            return View(talepler);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateKategorilerAsync();
            return View(new MasrafTalebiCreateViewModel());
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(MasrafTalebiCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                int aktifKullaniciId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                int aktifDepartmanId = int.Parse(User.FindFirstValue("DepartmanId"));

                await _masrafTalebiService.EkleAsync(model, aktifKullaniciId, aktifDepartmanId);
                return RedirectToAction(nameof(Index));
            }

            await PopulateKategorilerAsync(model.KategoriId);
            return View(model);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var talep = await _context.MasrafTalebis
                .Include(m => m.TalepEdenKullanici)
                .Include(m => m.Departman)
                .Include(m => m.Kategori)
                .FirstOrDefaultAsync(m => m.TalepId == id);

            if (talep == null)
            {
                return this.RecordNotFound(EntityName, "MasrafTalebi", requestedId: id, listLabel: "Talep listesine dön");
            }

            int aktifKullaniciId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            string rol = User.FindFirstValue(ClaimTypes.Role);
            
            ViewBag.AktifKullaniciId = aktifKullaniciId;
            ViewBag.KullaniciRolu = rol;

            ViewBag.DurumListesi = new SelectList(new[]
            {
                new { Id = DurumEnum.Bekliyor.ToString(), Ad = "Bekliyor" },
                new { Id = DurumEnum.Onaylandi.ToString(), Ad = "Onaylandı" },
                new { Id = DurumEnum.Reddedildi.ToString(), Ad = "Reddedildi" }
            }, "Id", "Ad", talep.Durum);

            return View(talep);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Mudur")]
        public async Task<IActionResult> UpdateDurum(int talepId, string durumAdi)
        {
            var talep = await _masrafTalebiService.GetTalepByIdAsync(talepId);
            if (talep == null)
            {
                return this.RecordNotFound(EntityName, "MasrafTalebi", requestedId: talepId, listLabel: "Talep listesine dön");
            }

            int aktifKullaniciId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            // Müdür kendi talebini güncelleyemez, sadece Admin güncelleyebilir
            if (userRole == "Mudur" && talep.TalepEdenKullaniciId == aktifKullaniciId)
            {
                // Yetkisiz işlem
                return RedirectToAction(nameof(Detail), new { id = talepId });
            }

            // Veritabanı CHK_TalepDurum kısıtlamasına uymayan durumlar için:
            // "IptalEdildi" listede olmadığı için hata alınıyor olabilir. 
            // Şimdilik sadece kısıtlamada olanları kaydedelim.
            talep.Durum = durumAdi;
            
            // Kullanıcıya bildirim gönder
            var mesaj = durumAdi == "Onaylandi" ? "Masraf talebiniz onaylandı." : 
                        durumAdi == "Reddedildi" ? "Masraf talebiniz reddedildi." : 
                        $"Masraf talebinizin durumu güncellendi: {durumAdi}";

            var bildirim = new Bildirim
            {
                AliciId = talep.TalepEdenKullaniciId,
                TalepId = talep.TalepId,
                BildirimTuru = "Durum Güncellemesi",
                Mesaj = mesaj,
                Okundu = false,
                OlusturulmaTarihi = DateTime.Now
            };

            await _context.Bildirims.AddAsync(bildirim);
            await _context.SaveChangesAsync();
            
            return RedirectToAction(nameof(Detail), new { id = talepId });
        }

        private async Task PopulateKategorilerAsync(int? selectedId = null)
        {
            var kategoriler = await _context.IslemKategorisis.OrderBy(k => k.KategoriAdi).ToListAsync();
            ViewBag.Kategoriler = new SelectList(kategoriler, "KategoriId", "KategoriAdi", selectedId);
        }
    }
}
