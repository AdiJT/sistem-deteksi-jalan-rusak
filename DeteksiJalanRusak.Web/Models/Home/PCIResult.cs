namespace DeteksiJalanRusak.Web.Models.Home;

public class PCIResult
{
    public required double PCI { get; set; }
    public required double CDVMax { get; set; }
    public required double TDV { get; set; }
    public required double MI { get; set; }

    public string Kondisi => PCI switch
    {
        >= 0 and <= 10 => "Gagal (Failed)",
        >= 11 and <= 25 => "Sangat Buruk (Very Poor)",
        >= 26 and <= 40 => "Buruk (Poor)",
        >= 41 and <= 55 => "Sedang (Fair)",
        >= 56 and <= 70 => "Baik (Good)",
        >= 71 and <= 85 => "Sangat Baik (Very Good)",
        >= 86 and <= 100 => "Sempuran (Excelent)",
        _ => throw new NotImplementedException()
    };

    public string JenisPenanganan => PCI switch
    {
        >= 0 and <= 10 => "Rekonstruksi",
        >= 11 and <= 25 => "Rekonstruksi",
        >= 26 and <= 40 => "Berkala",
        >= 41 and <= 55 => "Rutin",
        >= 56 and <= 70 => "Rutin",
        >= 71 and <= 85 => "Rutin",
        >= 86 and <= 100 => "Rutin",
        _ => throw new NotImplementedException()
    };
}
