using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Muhasebe.Models.Entities;

public partial class MuhasebeContext : DbContext
{
    public MuhasebeContext()
    {
    }

    public MuhasebeContext(DbContextOptions<MuhasebeContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Bildirim> Bildirims { get; set; }

    public virtual DbSet<Departman> Departmen { get; set; }

    public virtual DbSet<DepartmanButce> DepartmanButces { get; set; }
    
    public virtual DbSet<DepartmanButceDetay> DepartmanButceDetays { get; set; }

    public virtual DbSet<FinansalIslem> FinansalIslems { get; set; }

    public virtual DbSet<IslemKategorisi> IslemKategorisis { get; set; }

    public virtual DbSet<Kullanici> Kullanicis { get; set; }

    public virtual DbSet<Maas> Maas { get; set; }

    public virtual DbSet<MasrafTalebi> MasrafTalebis { get; set; }

    public virtual DbSet<Menu> Menus { get; set; }

    public virtual DbSet<OnayAkisi> OnayAkisis { get; set; }

    public virtual DbSet<Rol> Rols { get; set; }

    public virtual DbSet<RolMenu> RolMenus { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=EMEC\\SQLEXPRESS;Database=Muhasebe;Integrated Security=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bildirim>(entity =>
        {
            entity.HasKey(e => e.BildirimId).HasName("PK__Bildirim__E778CF9E000F7766");

            entity.ToTable("Bildirim");

            entity.Property(e => e.BildirimId).HasColumnName("BildirimID");
            entity.Property(e => e.AliciId).HasColumnName("AliciID");
            entity.Property(e => e.BildirimTuru).HasMaxLength(50);
            entity.Property(e => e.Mesaj).HasMaxLength(500);
            entity.Property(e => e.Okundu).HasDefaultValue(false);
            entity.Property(e => e.OlusturulmaTarihi)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TalepId).HasColumnName("TalepID");

            entity.HasOne(d => d.Alici).WithMany(p => p.Bildirims)
                .HasForeignKey(d => d.AliciId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Bildirim_Alici");

            entity.HasOne(d => d.Talep).WithMany(p => p.Bildirims)
                .HasForeignKey(d => d.TalepId)
                .HasConstraintName("FK_Bildirim_Talep");
        });

        modelBuilder.Entity<Departman>(entity =>
        {
            entity.HasKey(e => e.DepartmanId).HasName("PK__Departma__3A2312369606886F");

            entity.ToTable("Departman");

            entity.Property(e => e.DepartmanId).HasColumnName("DepartmanID");
            entity.Property(e => e.DepartmanAdi).HasMaxLength(100);
            entity.Property(e => e.MudurId).HasColumnName("MudurID");
            entity.Property(e => e.OlusturulmaTarihi)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Mudur).WithMany(p => p.Departmen)
                .HasForeignKey(d => d.MudurId)
                .HasConstraintName("FK_Departman_Mudur");
        });

        modelBuilder.Entity<DepartmanButce>(entity =>
        {
            entity.HasKey(e => e.ButceId).HasName("PK__Departma__04A1D98B6F5DB3AF");

            entity.ToTable("DepartmanButce");

            entity.HasIndex(e => new { e.DepartmanId, e.Yil, e.Ay }, "UQ_Butce_Donem").IsUnique();

            entity.Property(e => e.ButceId).HasColumnName("ButceID");
            entity.Property(e => e.AyrilanButce).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DepartmanId).HasColumnName("DepartmanID");
            entity.Property(e => e.KullanilanButce)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Departman).WithMany(p => p.DepartmanButces)
                .HasForeignKey(d => d.DepartmanId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Butce_Departman");
        });
        
        modelBuilder.Entity<DepartmanButceDetay>(entity =>
        {
            entity.HasKey(e => e.DetayId).HasName("PK__Departma__8E8163455EE90821");

            entity.ToTable("DepartmanButceDetay");

            entity.Property(e => e.AyrilanTutar).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.KullanilanTutar)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<FinansalIslem>(entity =>
        {
            entity.HasKey(e => e.IslemId).HasName("PK__Finansal__246DE2BB9F9283EE");

            entity.ToTable("FinansalIslem");

            entity.Property(e => e.IslemId).HasColumnName("IslemID");
            entity.Property(e => e.Aciklama).HasMaxLength(500);
            entity.Property(e => e.DepartmanId).HasColumnName("DepartmanID");
            entity.Property(e => e.IslemTarihi)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IslemTuru).HasMaxLength(10);
            entity.Property(e => e.KullaniciId).HasColumnName("KullaniciID");
            entity.Property(e => e.MasrafTalepId).HasColumnName("MasrafTalepID");
            entity.Property(e => e.OdenenMaasId).HasColumnName("OdenenMaasID");
            entity.Property(e => e.Tutar).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Departman).WithMany(p => p.FinansalIslems)
                .HasForeignKey(d => d.DepartmanId)
                .HasConstraintName("FK_Finans_Departman");

            entity.HasOne(d => d.Kullanici).WithMany(p => p.FinansalIslems)
                .HasForeignKey(d => d.KullaniciId)
                .HasConstraintName("FK_Finans_Kullanici");

            entity.HasOne(d => d.MasrafTalep).WithMany(p => p.FinansalIslems)
                .HasForeignKey(d => d.MasrafTalepId)
                .HasConstraintName("FK_Finans_Talep");

            entity.HasOne(d => d.OdenenMaas).WithMany(p => p.FinansalIslems)
                .HasForeignKey(d => d.OdenenMaasId)
                .HasConstraintName("FK_Finans_Maas");
        });

        modelBuilder.Entity<IslemKategorisi>(entity =>
        {
            entity.HasKey(e => e.KategoriId).HasName("PK__IslemKat__1782CC92F25EC73B");

            entity.ToTable("IslemKategorisi");

            entity.Property(e => e.KategoriId).HasColumnName("KategoriID");
            entity.Property(e => e.KategoriAdi).HasMaxLength(100);
            entity.Property(e => e.Tur).HasMaxLength(50);
        });

        modelBuilder.Entity<Kullanici>(entity =>
        {
            entity.HasKey(e => e.KullaniciId).HasName("PK__Kullanic__E011F09BA31E9BBA");

            entity.ToTable("Kullanici");

            entity.HasIndex(e => e.Eposta, "UQ__Kullanic__03ABA3918C2C05E6").IsUnique();

            entity.Property(e => e.KullaniciId).HasColumnName("KullaniciID");
            entity.Property(e => e.Ad).HasMaxLength(50);
            entity.Property(e => e.DepartmanId).HasColumnName("DepartmanID");
            entity.Property(e => e.Durum).HasDefaultValue(true);
            entity.Property(e => e.Eposta).HasMaxLength(100);
            entity.Property(e => e.RolId).HasColumnName("RolID");
            entity.Property(e => e.SifreHash).HasMaxLength(255);
            entity.Property(e => e.Soyad).HasMaxLength(50);

            entity.HasOne(d => d.Departman).WithMany(p => p.Kullanicis)
                .HasForeignKey(d => d.DepartmanId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Kullanici_Departman");

            entity.HasOne(d => d.Rol).WithMany(p => p.Kullanicis)
                .HasForeignKey(d => d.RolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Kullanici_Rol");
        });

        modelBuilder.Entity<Maas>(entity =>
        {
            entity.HasKey(e => e.MaasId).HasName("PK__Maas__22D00316D1BF7F01");

            entity.HasIndex(e => e.KullaniciId, "UQ_AktifMaas")
                .IsUnique()
                .HasFilter("([BitisTarihi] IS NULL)");

            entity.Property(e => e.MaasId).HasColumnName("MaasID");
            entity.Property(e => e.AylikTutar).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.KullaniciId).HasColumnName("KullaniciID");

            entity.HasOne(d => d.Kullanici).WithOne(p => p.Maas)
                .HasForeignKey<Maas>(d => d.KullaniciId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Maas_Kullanici");
        });

        modelBuilder.Entity<MasrafTalebi>(entity =>
        {
            entity.HasKey(e => e.TalepId).HasName("PK__MasrafTa__AF651643BAEE756A");

            entity.ToTable("MasrafTalebi");

            entity.Property(e => e.TalepId).HasColumnName("TalepID");
            entity.Property(e => e.Aciklama).HasMaxLength(500);
            entity.Property(e => e.DepartmanId).HasColumnName("DepartmanID");
            entity.Property(e => e.Durum)
                .HasDefaultValue(1);
            entity.Property(e => e.KategoriId).HasColumnName("KategoriID");
            entity.Property(e => e.TalepEdenKullaniciId).HasColumnName("TalepEdenKullaniciID");
            entity.Property(e => e.TalepTarihi)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Tutar).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Departman).WithMany(p => p.MasrafTalebis)
                .HasForeignKey(d => d.DepartmanId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Talep_Departman");

            entity.HasOne(d => d.Kategori).WithMany(p => p.MasrafTalebis)
                .HasForeignKey(d => d.KategoriId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Talep_Kategori");

            entity.HasOne(d => d.TalepEdenKullanici).WithMany(p => p.MasrafTalebis)
                .HasForeignKey(d => d.TalepEdenKullaniciId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Talep_Eden");
        });

        modelBuilder.Entity<Menu>(entity =>
        {
            entity.HasKey(e => e.MenuId).HasName("PK__Menu__C99ED2505B08567D");

            entity.ToTable("Menu");

            entity.Property(e => e.MenuId).HasColumnName("MenuID");
            entity.Property(e => e.Icon).HasMaxLength(50);
            entity.Property(e => e.MenuAdi).HasMaxLength(50);
            entity.Property(e => e.Sira).HasDefaultValue(0);
            entity.Property(e => e.UstMenuId).HasColumnName("UstMenuID");
            entity.Property(e => e.ControllerAdi).HasMaxLength(50).IsRequired();
            entity.Property(e => e.ActionAdi).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Durum).HasDefaultValue(true);

            entity.HasOne(d => d.UstMenu).WithMany(p => p.AltMenuler)
                .HasForeignKey(d => d.UstMenuId)
                .HasConstraintName("FK_Menu_UstMenu");
        });

        modelBuilder.Entity<OnayAkisi>(entity =>
        {
            entity.HasKey(e => e.OnayId).HasName("PK__OnayAkis__4A872CE70AD7FB44");

            entity.ToTable("OnayAkisi");

            entity.Property(e => e.OnayId).HasColumnName("OnayID");
            entity.Property(e => e.Aciklama).HasMaxLength(500);
            entity.Property(e => e.IslemTarihi)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.OnayDurumu).HasMaxLength(20);
            entity.Property(e => e.OnaylayanId).HasColumnName("OnaylayanID");
            entity.Property(e => e.TalepId).HasColumnName("TalepID");

            entity.HasOne(d => d.Onaylayan).WithMany(p => p.OnayAkisis)
                .HasForeignKey(d => d.OnaylayanId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Onay_Onaylayan");

            entity.HasOne(d => d.Talep).WithMany(p => p.OnayAkisis)
                .HasForeignKey(d => d.TalepId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Onay_Talep");
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.RolId).HasName("PK__Rol__F92302D144ED1B95");

            entity.ToTable("Rol");

            entity.Property(e => e.RolId).HasColumnName("RolID");
            entity.Property(e => e.Aciklama).HasMaxLength(255);
            entity.Property(e => e.RolAdi).HasMaxLength(50);
        });

        modelBuilder.Entity<RolMenu>(entity =>
        {
            entity.HasKey(e => e.EslesmeId).HasName("PK__Rol_Menu__A4F5139C905020B9");

            entity.ToTable("Rol_Menu");

            entity.HasIndex(e => new { e.RolId, e.MenuId }, "UQ_Rol_Menu").IsUnique();

            entity.Property(e => e.EslesmeId).HasColumnName("EslesmeID");
            entity.Property(e => e.MenuId).HasColumnName("MenuID");
            entity.Property(e => e.RolId).HasColumnName("RolID");

            entity.HasOne(d => d.Menu).WithMany(p => p.RolMenus)
                .HasForeignKey(d => d.MenuId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RolMenu_Menu");

            entity.HasOne(d => d.Rol).WithMany(p => p.RolMenus)
                .HasForeignKey(d => d.RolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RolMenu_Rol");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
