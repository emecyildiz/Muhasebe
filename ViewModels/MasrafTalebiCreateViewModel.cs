using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
namespace Muhasebe.ViewModels
{
    public class MasrafTalebiCreateViewModel
    {

        [Required(ErrorMessage = "Lütfen masraf kategorisini seçiniz.")]
        public int KategoriId { get; set; }

        [Required(ErrorMessage = "Lütfen masraf tutarını giriniz.")]
        public decimal Tutar { get; set; }

        [Required(ErrorMessage = "Lütfen masrafın nedenini açıklayınız.")]
        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
        public string Aciklama { get; set; } = null!;


    }
}
