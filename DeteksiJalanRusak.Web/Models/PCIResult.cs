namespace DeteksiJalanRusak.Web.Models;

public class PCIResult
{
    public required double PCI { get; set; }
    public required double CDVMax { get; set; }
    public required double TDVMax { get; set; }
    public required double QMax { get; set; }
    public required double MI { get; set; }

    public string Kondisi => HelperFunctions.KondisiPCI(PCI);

    public string JenisPenanganan => HelperFunctions.JenisPenangananPCI(PCI);
}
