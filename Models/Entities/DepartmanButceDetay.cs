using System;
using System.Collections.Generic;

namespace Muhasebe.Models.Entities;

public partial class DepartmanButceDetay
{
    public int DetayId { get; set; }

    public int ButceId { get; set; }

    public int KategoriId { get; set; }

    public decimal AyrilanTutar { get; set; }

    public decimal? KullanilanTutar { get; set; }
}
