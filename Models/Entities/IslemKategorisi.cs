using System;
using System.Collections.Generic;

namespace Muhasebe.Models.Entities;

public partial class IslemKategorisi
{
    public int KategoriId { get; set; }

    public string KategoriAdi { get; set; } = null!;

    public string Tur { get; set; } = null!;

    public virtual ICollection<MasrafTalebi> MasrafTalebis { get; set; } = new List<MasrafTalebi>();
}
