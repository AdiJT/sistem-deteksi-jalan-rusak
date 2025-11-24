using System.ComponentModel.DataAnnotations;

namespace DeteksiJalanRusak.Web.Models.Home;

public class IndexVM
{
    [Display(Name = "Foto Kerusakan dan Kertas (.jpg, .jpeg, .png)")]
    [Required(ErrorMessage = "Foto harus dipilih")]
    public IFormFile? FormFile { get; set; }

    [Display(Name = "Luas Sampel (meter persegi)")]
    [Required(ErrorMessage = "Harus diisi")]
    public double LuasSampel { get; set; } = 200;

    public ResultVM? ResultVM { get; set; }
}
