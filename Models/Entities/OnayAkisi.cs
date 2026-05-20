using System;
using System.Collections.Generic;

namespace Muhasebe.Models.Entities;

public partial class OnayAkisi
{
    public int OnayId { get; set; }

    public int TalepId { get; set; }

    public int OnaylayanId { get; set; }

    public string OnayDurumu { get; set; } = null!;

    public string? Aciklama { get; set; }

    public DateTime? IslemTarihi { get; set; }

    public virtual Kullanici Onaylayan { get; set; } = null!;

    public virtual MasrafTalebi Talep { get; set; } = null!;
}
