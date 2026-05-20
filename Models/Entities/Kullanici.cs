using System;
using System.Collections.Generic;

namespace Muhasebe.Models.Entities;

public partial class Kullanici
{
    public int KullaniciId { get; set; }

    public string Ad { get; set; } = null!;

    public string Soyad { get; set; } = null!;

    public string Eposta { get; set; } = null!;

    public string SifreHash { get; set; } = null!;

    public int RolId { get; set; }

    public int DepartmanId { get; set; }

    public DateOnly? IseGirisTarihi { get; set; }

    public bool? Durum { get; set; }

    public virtual ICollection<Bildirim> Bildirims { get; set; } = new List<Bildirim>();

    public virtual Departman Departman { get; set; } = null!;

    public virtual ICollection<Departman> Departmen { get; set; } = new List<Departman>();

    public virtual ICollection<FinansalIslem> FinansalIslems { get; set; } = new List<FinansalIslem>();

    public virtual Maas? Maa { get; set; }

    public virtual ICollection<MasrafTalebi> MasrafTalebis { get; set; } = new List<MasrafTalebi>();

    public virtual ICollection<OnayAkisi> OnayAkisis { get; set; } = new List<OnayAkisi>();

    public virtual Rol Rol { get; set; } = null!;
}
