using System.ComponentModel.DataAnnotations;

namespace DeteksiJalanRusak.Web.Models.Home;

public class IndexVM
{
    [Display(Name = "Foto Kerusakan dan Kertas (.jpg, .jpeg, .png)")]
    [Required(ErrorMessage = "Foto harus dipilih")]
    public IFormFile? FormFile { get; set; }

    public double PanjangSegmen { get; set; }
    public double LebarSegmen { get; set; }

    public ResultVM? ResultVM { get; set; }
}
