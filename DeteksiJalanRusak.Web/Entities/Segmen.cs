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

        var dvMoreThan2 = densityDVs.Where(x => x.DeductValue > 2).OrderByDescending(x => x.DeductValue);
        var dv2OrLess = densityDVs.Where(x => x.DeductValue <= 2).OrderByDescending(x => x.DeductValue);
        var dvMax = densityDVs.Select(x => x.DeductValue).Max();
        var mi = 1 + (9 / 98 * (100 - dvMax));

        var q = Math.Clamp(dvMoreThan2.Count(), HelperFunctions.QMIN, HelperFunctions.QMAX);
        var tdv = dvMoreThan2.Sum(x => x.DeductValue) + dv2OrLess.Sum(x => x.DeductValue);
        var cdvMax = HelperFunctions.TdvToCdv(tdv, q);

        var qMax = q;
        var tdvMax = tdv;

        for (int i = q - 1; i >= 1; i--)
        {
            tdv = dvMoreThan2.Take(i).Sum(x => x.DeductValue) + (2 * (q - i)) + dv2OrLess.Sum(x => x.DeductValue);
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
