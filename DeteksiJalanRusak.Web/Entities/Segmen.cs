using DeteksiJalanRusak.Web.Models;

namespace DeteksiJalanRusak.Web.Entities;

public class Segmen
{
    public int Id { get; set; }

    public required double LuasSampel { get; set; }

    public List<DensityDV> DensityDVs() => [.. DaftarFoto
        .Where(x => x.AdaKertas)
        .SelectMany(x => x.DaftarKerusakan)
        .GroupBy(x => new { x.Label, x.Kondisi })
        .Select(x => new DensityDV
        {
            DistressDensity = x.Sum(x => x.LuasPersegi) / LuasSampel * 100d,
            Kondisi = x.Key.Kondisi,
            Label = x.Key.Label,
            TotalLuas = x.Sum(x => x.LuasPersegi)
        })
        .OrderByDescending(x => x.DeductValue)];

    public PCIResult? MetodePCI()
    {
        var densityDVs = DensityDVs();
        if (densityDVs.Count == 0) return null;

        var dvLebihDari2 = densityDVs.Where(x => x.DeductValue > 2d).OrderByDescending(x => x.DeductValue);
        var dvKurangDariSamaDengan2 = densityDVs.Where(x => x.DeductValue <= 2d).OrderByDescending(x => x.DeductValue);
        var dvMax = densityDVs.Select(x => x.DeductValue).Max();
        var mi = 1 + (9 / 98 * (100 - dvMax));

        var q = Math.Clamp(dvLebihDari2.Count(), HelperFunctions.QMIN, HelperFunctions.QMAX);
        var tdv = dvLebihDari2.Sum(x => x.DeductValue) + dvKurangDariSamaDengan2.Sum(x => x.DeductValue);
        var cdvMax = HelperFunctions.TdvToCdv(tdv, q);

        var qMax = q;
        var tdvMax = tdv;

        for (int i = q - 1; i >= 1; i--)
        {
            tdv = dvLebihDari2.Take(i).Sum(x => x.DeductValue) + (2d * (q - i)) + dvKurangDariSamaDengan2.Sum(x => x.DeductValue);
            var cdv = HelperFunctions.TdvToCdv(tdv, i);
            if (cdv > cdvMax)
            {
                tdvMax = tdv;
                qMax = i;
                cdvMax = cdv;
            }
        }

        var pci = 100 - cdvMax;

        return new PCIResult
        {
            PCI = pci,
            CDVMax = cdvMax,
            TDVMax = tdvMax,
            QMax = qMax,
            MI = mi,
        };
    }

    public Analisis Analisis { get; set; }
    public List<FotoKerusakan> DaftarFoto { get; set; } = [];
}
