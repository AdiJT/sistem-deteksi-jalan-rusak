using System.ComponentModel.DataAnnotations;

namespace DeteksiJalanRusak.Web.Models.AnalisisModels;

public class EditVM
{
    public required int Id { get; set; }

    [Display(Name = "Nama")]
    [Required(ErrorMessage = "{0} harus diisi")]
    public required string Nama { get; set; }

    [Display(Name = "Lokasi")]
    [Required(ErrorMessage = "{0} harus diisi")]
    public required string Lokasi { get; set; }

    [Display(Name = "Pembuat")]
    [Required(ErrorMessage = "{0} harus diisi")]
    public required string Pembuat { get; set; }
}
