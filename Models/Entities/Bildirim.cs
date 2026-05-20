using System;
using System.Collections.Generic;

namespace Muhasebe.Models.Entities;

public partial class Bildirim
{
    public int BildirimId { get; set; }

    public int AliciId { get; set; }

    public int? TalepId { get; set; }

    public string BildirimTuru { get; set; } = null!;

    public string Mesaj { get; set; } = null!;

    public bool? Okundu { get; set; }

    public DateTime? OlusturulmaTarihi { get; set; }

    public virtual Kullanici Alici { get; set; } = null!;

    public virtual MasrafTalebi? Talep { get; set; }
}
