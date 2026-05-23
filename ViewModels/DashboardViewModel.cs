namespace Muhasebe.ViewModels
{
    public class DashboardViewModel
    {
        public decimal ToplamGelir { get; set; }
        public decimal ToplamGider { get; set; }
        public decimal ToplamMaasYuku { get; set; }
        public decimal BekleyenMasrafTalebi { get; set; }
        public List<DepartmanButce.CategoryAllocationViewModel> ButceDetaylari { get; set; } = new List<DepartmanButce.CategoryAllocationViewModel>();
        public decimal ToplamAktifButce { get; set; }
        public decimal KalanAktifButce { get; set; }
        public List<SonIslemViewModel> SonIslemler { get; set; } = new List<SonIslemViewModel>();
    }

    public class SonIslemViewModel
    {
        public string IslemTuru { get; set; } // Gelir, Gider, Masraf Talebi vs.
        public string Aciklama { get; set; }
        public decimal Tutar { get; set; }
        public DateTime Tarih { get; set; }
        public string Durum { get; set; } // Onaylandı, Bekliyor vs.
        public string CSSRenk { get; set; } // ui için
    }
}
