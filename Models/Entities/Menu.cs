using System;
using System.Collections.Generic;

namespace Muhasebe.Models.Entities;

public partial class Menu
{
    public int MenuId { get; set; }

    public int? UstMenuId { get; set; }

    public string MenuAdi { get; set; } = null!;
    public string ActionAdi { get; set; } = null!;
    public string ControllerAdi { get; set; } = null!;
    public string? Icon { get; set; }
    public bool Durum { get; set; }
    public int? Sira { get; set; }

    public virtual ICollection<Menu> AltMenuler { get; set; } = new List<Menu>();

    public virtual ICollection<RolMenu> RolMenus { get; set; } = new List<RolMenu>();

    public virtual Menu? UstMenu { get; set; }
}
