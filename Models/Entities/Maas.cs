using System;
using System.Collections.Generic;

namespace Muhasebe.Models.Entities;

public partial class Maas
{
    public int MaasId { get; set; }

    public int KullaniciId { get; set; }

    public decimal AylikTutar { get; set; }

    public DateOnly BaslangicTarihi { get; set; }

    public DateOnly? BitisTarihi { get; set; }

    public virtual ICollection<FinansalIslem> FinansalIslems { get; set; } = new List<FinansalIslem>();

    public virtual Kullanici Kullanici { get; set; } = null!;
}
