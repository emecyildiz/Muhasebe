using System;
using System.Collections.Generic;

namespace Muhasebe.Models.Entities;

public partial class MasrafTalebi
{
    public int TalepId { get; set; }

    public int TalepEdenKullaniciId { get; set; }

    public int DepartmanId { get; set; }

    public int KategoriId { get; set; }

    public decimal Tutar { get; set; }

    public string? Aciklama { get; set; }

    public DateTime TalepTarihi { get; set; }

    public int Durum { get; set; }

    public virtual ICollection<Bildirim> Bildirims { get; set; } = new List<Bildirim>();

    public virtual Departman Departman { get; set; } = null!;

    public virtual ICollection<FinansalIslem> FinansalIslems { get; set; } = new List<FinansalIslem>();

    public virtual IslemKategorisi Kategori { get; set; } = null!;

    public virtual ICollection<OnayAkisi> OnayAkisis { get; set; } = new List<OnayAkisi>();

    public virtual Kullanici TalepEdenKullanici { get; set; } = null!;
}
