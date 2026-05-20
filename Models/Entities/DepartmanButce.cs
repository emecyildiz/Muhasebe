using System;
using System.Collections.Generic;

namespace Muhasebe.Models.Entities;

public partial class DepartmanButce
{
    public int ButceId { get; set; }

    public int DepartmanId { get; set; }

    public int Yil { get; set; }

    public int? Ay { get; set; }

    public decimal AyrilanButce { get; set; }

    public decimal? KullanilanButce { get; set; }

    public virtual Departman Departman { get; set; } = null!;
}
