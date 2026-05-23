using System.Collections.Generic;

namespace Muhasebe.ViewModels.DepartmanButce
{
    public class AllocateViewModel
    {
        public int ButceId { get; set; }
        public string DepartmanAdi { get; set; }
        public decimal ToplamButce { get; set; }
        public decimal ToplamDagitilan { get; set; }
        public decimal MaasButcesi { get; set; }
        
        public List<CategoryAllocationViewModel> Allocations { get; set; } = new List<CategoryAllocationViewModel>();
    }

    public class CategoryAllocationViewModel
    {
        public int KategoriId { get; set; }
        public string KategoriAdi { get; set; }
        public decimal AyrilanTutar { get; set; }
    }
}
