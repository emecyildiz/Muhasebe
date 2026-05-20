using Microsoft.EntityFrameworkCore;
using Muhasebe.Models.Entities;

namespace Muhasebe.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Rol> Roller { get; set; }
        public DbSet<Departman> Departmanlar { get; set; }
        public DbSet<Kullanici> Kullanicilar { get; set; }
        public DbSet<Maas> Maaslar { get; set; }
        public DbSet<DepartmanButce> DepartmanButceleri { get; set; }
        public DbSet<FinansalIslem> FinansalIslemler { get; set; }
        public DbSet<IslemKategorisi> IslemKategorileri { get; set; }
        public DbSet<MasrafTalebi> MasrafTalepleri { get; set; }
        public DbSet<OnayAkisi> OnayAkislari { get; set; }
        public DbSet<Bildirim> Bildirimler { get; set; }
        public DbSet<Menu> Menuler { get; set; }
        public DbSet<RolMenu> RolMenuler { get; set; }
    }

}
