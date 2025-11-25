using System.ComponentModel.DataAnnotations;

namespace DeteksiJalanRusak.Web.Models.Home;

public class IndexVM
{
    [Display(Name = "Foto-Foto Kerusakan dan Kertas (.jpg, .jpeg, .png)")]
    [Required(ErrorMessage = "Minimal 1 foto harus dipilih")]
    [MinLength(1)]
    public List<IFormFile> FormFiles { get; set; } = [];

    [Display(Name = "Luas Sampel (meter persegi)")]
    [Required(ErrorMessage = "Harus diisi")]
    public double LuasSampel { get; set; } = 200;

    public List<ResultVM> ResultVMs { get; set; } = [];
    public List<DensityDV> DensityDVs{ get; set; } = [];
    public PCIResult? PCIResult { get; set; }
}
