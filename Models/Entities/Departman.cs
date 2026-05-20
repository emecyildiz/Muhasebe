using System;
using System.Collections.Generic;

namespace Muhasebe.Models.Entities;

public partial class Departman
{
    public int DepartmanId { get; set; }

    public string DepartmanAdi { get; set; } = null!;

    public int? MudurId { get; set; }

    public DateTime? OlusturulmaTarihi { get; set; }

    public virtual ICollection<DepartmanButce> DepartmanButces { get; set; } = new List<DepartmanButce>();

    public virtual ICollection<FinansalIslem> FinansalIslems { get; set; } = new List<FinansalIslem>();

    public virtual ICollection<Kullanici> Kullanicis { get; set; } = new List<Kullanici>();

    public virtual ICollection<MasrafTalebi> MasrafTalebis { get; set; } = new List<MasrafTalebi>();

    public virtual Kullanici? Mudur { get; set; }
}
