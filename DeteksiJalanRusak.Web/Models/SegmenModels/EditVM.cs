using System.ComponentModel.DataAnnotations;

namespace DeteksiJalanRusak.Web.Models.SegmenModels;

public class EditVM
{
    public required int Id { get; set; }

    [Display(Name = "Luas Sampel (meter persegi)")]
    [Required(ErrorMessage = "Harus diisi")]
    public required double LuasSampel { get; set; }

    public required string ReturnUrl { get; set; }
}
