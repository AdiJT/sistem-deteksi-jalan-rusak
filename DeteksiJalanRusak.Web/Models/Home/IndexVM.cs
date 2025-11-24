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
    public double PCI { get; set; }
    public double CDVMax { get; set; }
    public double TDV { get; set; }
    public double MI { get; set; }
}
