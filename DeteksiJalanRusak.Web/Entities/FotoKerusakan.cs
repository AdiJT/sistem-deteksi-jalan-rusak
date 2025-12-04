namespace DeteksiJalanRusak.Web.Entities;

public class FotoKerusakan
{
    public int Id { get; set; }

    public required string FileName { get; set; }
    public required string ImageBase64 { get; set; }
    public required double Lebar { get; set; }
    public required double Tinggi { get; set; }
    public required bool AdaKertas { get; set; }
    public string Mask64Kertas { get; set; } = string.Empty;
    public double LuasKertas { get; set; }
    public double PanjangKertas { get; set; }
    public double LebarKertas { get; set; }
    public double LuasPersegiKertas => PanjangKertas * LebarKertas;
    public double M2PerPiksel { get; set; }
    public double MPerPiksel { get; set; }
    public int TinggiPikselKertas { get; set; }
    public int LebarPikselKertas { get; set; }

    public Segmen Segmen { get; set; }
    public List<Kerusakan> DaftarKerusakan { get; set; } = [];
}
