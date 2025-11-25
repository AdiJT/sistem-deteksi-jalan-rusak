using DeteksiJalanRusak.Web.Extensions;
using Humanizer;
using YoloDotNet.Extensions;
using YoloDotNet.Models;

namespace DeteksiJalanRusak.Web.Models.Home;

public class Kerusakan
{
    public required Segmentation Segmentation { get; set; }
    public double Luas { get; set; }
    public double Panjang { get; set; }
    public double Lebar { get; set; }
    public double LuasPersegi => Panjang * Lebar;

    public LabelEnum Label => Segmentation.Label.Name.DehumanizeTo<LabelEnum>();
    public KondisiKerusakan Kondisi => DistressDensityToDV.Kondisi(Label, Luas, Panjang, Lebar);
    public string MaskBase64 => Segmentation.BitPackedPixelMask.UnpackToBitmap(Segmentation.BoundingBox.Width, Segmentation.BoundingBox.Height).ToBase64String();
}
