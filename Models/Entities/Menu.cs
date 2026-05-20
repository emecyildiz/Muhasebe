using System;
using System.Collections.Generic;

namespace Muhasebe.Models.Entities;

public partial class Menu
{
    public int MenuId { get; set; }

    public int? UstMenuId { get; set; }

    public string MenuAdi { get; set; } = null!;

    public string Url { get; set; } = null!;

    public string? Icon { get; set; }

    public int? Sira { get; set; }

    public virtual ICollection<Menu> InverseUstMenu { get; set; } = new List<Menu>();

    public virtual ICollection<RolMenu> RolMenus { get; set; } = new List<RolMenu>();

    public virtual Menu? UstMenu { get; set; }
}
