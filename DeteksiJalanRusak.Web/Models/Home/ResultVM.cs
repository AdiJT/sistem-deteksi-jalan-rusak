namespace DeteksiJalanRusak.Web.Models.Home;

public class ResultVM
{
    public required string FileName { get; set; }
    public required string ImageBase64 { get; set; }
    public required double Lebar { get; set; }
    public required double Tinggi { get; set; }
    public required Kerusakan? Kertas { get; set; }
    public required List<Kerusakan> Results { get; set; }

    public double M2PerPiksel { get; set; }
    public double MPerPiksel { get; set; }
}