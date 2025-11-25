namespace DeteksiJalanRusak.Web.Models.Home;

public class DensityDV
{
    public required KondisiKerusakan Kondisi { get; set; }
    public required LabelEnum Label { get; set; }
    public required double DistressDensity { get; set; }
    public required double TotalLuas { get; set; }

    public double DeductValue => DistressDensityToDV.DDToDV(Label, DistressDensity, Kondisi);
}