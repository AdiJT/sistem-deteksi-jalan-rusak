using System.ComponentModel.DataAnnotations;

namespace DeteksiJalanRusak.Web.Models.SegmenModels;

public class TambahVM
{
    [Display(Name = "Foto-Foto Kerusakan dan Kertas (.jpg, .jpeg, .png)")]
    [Required(ErrorMessage = "Minimal 1 foto harus dipilih")]
    [MinLength(1)]
    public List<IFormFile> FormFiles { get; set; } = [];

    [Display(Name = "Luas Sampel (meter persegi)")]
    [Required(ErrorMessage = "Harus diisi")]
    public double LuasSampel { get; set; } = 200;

    [Display(Name = "Koordinat (Lat.)")]
    public double Lat { get; set; }

    [Display(Name = "Koordinat (Lng.)")]
    public double Lng { get; set; }

    public required int IdAnalisis { get; set; }
}
