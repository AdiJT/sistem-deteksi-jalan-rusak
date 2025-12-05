using System.ComponentModel.DataAnnotations;

namespace DeteksiJalanRusak.Web.Models.AnalisisModels;

public class TambahVM
{
    [Display(Name = "Nama")]
    [Required(ErrorMessage = "{0} harus diisi")]
    public string Nama { get; set; } = string.Empty;

    [Display(Name = "Lokasi")]
    [Required(ErrorMessage = "{0} harus diisi")]
    public string Lokasi { get; set; } = string.Empty;

    [Display(Name = "Pembuat")]
    [Required(ErrorMessage = "{0} harus diisi")]
    public string Pembuat { get; set; } = string.Empty;
}
