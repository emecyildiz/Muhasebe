using System;
using System.Collections.Generic;

namespace Muhasebe.Models.Entities;

public partial class Rol
{
    public int RolId { get; set; }

    public string RolAdi { get; set; } = null!;

    public string? Aciklama { get; set; }

    public virtual ICollection<Kullanici> Kullanicis { get; set; } = new List<Kullanici>();

    public virtual ICollection<RolMenu> RolMenus { get; set; } = new List<RolMenu>();
}
