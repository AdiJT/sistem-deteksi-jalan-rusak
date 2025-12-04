using DeteksiJalanRusak.Web.Models;

namespace DeteksiJalanRusak.Web.Entities;

public class Analisis
{
    public int Id { get; set; }
    public required string Nama { get; set; }
    public required string Lokasi { get; set; }
    public required string Pembuat { get; set; }

    public double RataRataPCI
    {
        get
        {
            var daftarPCI = DaftarSegmen.Select(x => x.MetodePCI()).Where(x => x != null).Select(x => x!.PCI);

            return daftarPCI.Any() ? daftarPCI.Average() : 0;
        }
    }

    public string Kondisi => HelperFunctions.KondisiPCI(RataRataPCI);
    public string JenisPenanganan => HelperFunctions.JenisPenangananPCI(RataRataPCI);

    public List<Segmen> DaftarSegmen { get; set; } = [];
}
