namespace DeteksiJalanRusak.Web.Models;

public class DensityDV
{
    public required KondisiKerusakan Kondisi { get; set; }
    public required LabelEnum Label { get; set; }
    public required double DistressDensity { get; set; }
    public required double TotalLuas { get; set; }

    public double DeductValue => HelperFunctions.DDToDV(Label, DistressDensity, Kondisi);
}