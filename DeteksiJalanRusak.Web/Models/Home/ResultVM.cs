using YoloDotNet.Models;

namespace DeteksiJalanRusak.Web.Models.Home;

public class ResultVM
{
    public required string FileName { get; set; }
    public required string ImageBase64 { get; set; }
    public required double Lebar { get; set; }
    public required double Tinggi { get; set; }
    public required bool AdaKertas { get; set; }
    public required List<Kerusakan> Results { get; set; }

    public double M2PerPiksel { get; set; }
    public double MPerPiksel { get; set; }
}

public class Kerusakan
{
    public required Segmentation Segmentation { get; set; }
    public double Luas { get; set; }
    public double LuasPersegi { get; set; }
    public double Panjang { get; set; }
    public double Lebar { get; set; }
}