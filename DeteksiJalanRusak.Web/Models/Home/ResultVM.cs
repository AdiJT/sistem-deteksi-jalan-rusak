using YoloDotNet.Models;

namespace DeteksiJalanRusak.Web.Models.Home;

public class ResultVM
{
    public required string FileName { get; set; }
    public required string ImageBase64 { get; set; }
    public required List<Kerusakan> Results { get; set; }
}

public class Kerusakan
{
    public required Segmentation Segmentation { get; set; }
    public required bool AdaUkuran { get; set; }
    public double Luas { get; set; }
    public double Panjang { get; set; }
    public double Lebar { get; set; }
}