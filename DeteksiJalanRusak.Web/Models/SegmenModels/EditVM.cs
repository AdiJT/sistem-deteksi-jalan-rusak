using System.ComponentModel.DataAnnotations;

namespace DeteksiJalanRusak.Web.Models.SegmenModels;

public class EditVM
{
    public required int Id { get; set; }

    [Display(Name = "Luas Sampel (meter persegi)")]
    [Required(ErrorMessage = "Harus diisi")]
    public required double LuasSampel { get; set; }

    [Display(Name = "Koordinat (Lat.)")]
    public required double? Lat { get; set; } = 0;

    [Display(Name = "Koordinat (Lng.)")]
    public required double? Lng { get; set; } = 0;

    public required string ReturnUrl { get; set; }
}
