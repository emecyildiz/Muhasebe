using System;
using System.Collections.Generic;

namespace Muhasebe.Models.Entities;

public partial class RolMenu
{
    public int EslesmeId { get; set; }

    public int RolId { get; set; }

    public int MenuId { get; set; }

    public virtual Menu Menu { get; set; } = null!;

    public virtual Rol Rol { get; set; } = null!;
}
