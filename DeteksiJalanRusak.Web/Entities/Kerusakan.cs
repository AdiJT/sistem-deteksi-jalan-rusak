using DeteksiJalanRusak.Web.Models;

namespace DeteksiJalanRusak.Web.Entities;

public class Kerusakan
{
    public int Id { get; set; }

    public required string MaskBase64 { get; set; }
    public required LabelEnum Label { get; set; }
    public required double Luas { get; set; }
    public required double Panjang { get; set; }
    public required double Lebar { get; set; }
    public required int TinggiPiksel { get; set; }
    public required int LebarPiksel { get; set; }

    public double LuasPersegi => Panjang * Lebar;
    public KondisiKerusakan Kondisi => HelperFunctions.Kondisi(Label, Luas, Panjang, Lebar);

    public FotoKerusakan FotoKerusakan { get; set; }
}
