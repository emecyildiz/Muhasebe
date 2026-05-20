using System;
using System.Collections.Generic;

namespace Muhasebe.Models.Entities;

public partial class FinansalIslem
{
    public int IslemId { get; set; }

    public string IslemTuru { get; set; } = null!;

    public decimal Tutar { get; set; }

    public DateTime? IslemTarihi { get; set; }

    public int? DepartmanId { get; set; }

    public int? MasrafTalepId { get; set; }

    public int? OdenenMaasId { get; set; }

    public int? KullaniciId { get; set; }

    public string? Aciklama { get; set; }

    public virtual Departman? Departman { get; set; }

    public virtual Kullanici? Kullanici { get; set; }

    public virtual MasrafTalebi? MasrafTalep { get; set; }

    public virtual Maas? OdenenMaas { get; set; }
}
