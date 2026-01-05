using MathNet.Numerics.Interpolation;
using OpenCvSharp;

namespace DeteksiJalanRusak.Web.Models;

public static class HelperFunctions
{
    private static readonly double[] _x = [0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100];

    public const int QMIN = 1;
    public const int QMAX = 7;

    public static double DDToDV(LabelEnum label, double density, KondisiKerusakan kondisi) =>
        label switch
        {
            LabelEnum.Alur => Alur(density, kondisi),
            LabelEnum.Bleding => Bleeding(density, kondisi),
            LabelEnum.Lubang => Lubang(density, kondisi),
            LabelEnum.PelepasanButiran => PelepasanButir(density, kondisi),
            LabelEnum.RetakBuaya => RetakBuaya(density, kondisi),
            LabelEnum.RetakMemanjang => RetakMemanjang(density, kondisi),
            LabelEnum.Tambalan => Tambalan(density, kondisi),
            _ => throw new NotImplementedException()
        };

    public static double RetakBuaya(double density, KondisiKerusakan type)
    {
        double[] yL = [0, 3, 3, 4, 5, 6, 7, 8, 9, 10, 11, 16, 21, 24, 26, 28, 30, 31, 32, 33, 41, 46, 50, 53, 55, 58, 60, 61, 100];
        double[] yM = [0, 7, 10, 11, 14, 16, 17, 18, 20, 21, 22, 29, 33, 36, 39, 41, 43, 44, 45, 47, 56, 61, 65, 69, 71, 73, 74, 77, 100];
        double[] yH = [0, 11, 16, 19, 20, 21, 25, 27, 28, 29, 30, 40, 46, 50, 52, 56, 58, 60, 61, 62, 71, 78, 80, 81, 85, 88, 89, 90, 100];

        return type switch
        {
            KondisiKerusakan.Low => CubicSpline.InterpolatePchipSorted(_x, yL).Interpolate(density),
            KondisiKerusakan.Medium => CubicSpline.InterpolatePchipSorted(_x, yM).Interpolate(density),
            KondisiKerusakan.High => CubicSpline.InterpolatePchipSorted(_x, yH).Interpolate(density),
            _ => throw new ArgumentException(null, nameof(type)),
        };
    }

    public static double Bleeding(double density, KondisiKerusakan type)
    {
        double[] yL = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0.5, 1, 1.25, 1.5, 1.5, 1.75, 2, 2.25, 2.5, 6, 8.5, 10.5, 12.5, 14.5, 16, 17.5, 19, 100/*20*/];
        double[] yM = [0, 0, 1, 1.25, 1.5, 1.75, 2, 2.25, 2.5, 2.6, 2.75, 4.75, 6.5, 7.75, 9, 10, 10.5, 11, 12, 12.5, 18.5, 22.5, 26, 29, 31.5, 33.5, 36, 38.5, 100/*40*/];
        double[] yH = [0, 2, 2.75, 3, 3.25, 3.5, 4, 4.25, 4.5, 5, 5.5, 8.5, 11, 12.25, 14.5, 16.5, 18, 20, 21.5, 23, 34.5, 43.5, 49, 54, 59, 63, 67, 71, 100/*73*/];

        return type switch
        {
            KondisiKerusakan.Low => CubicSpline.InterpolatePchipSorted(_x, yL).Interpolate(density),
            KondisiKerusakan.Medium => CubicSpline.InterpolatePchipSorted(_x, yM).Interpolate(density),
            KondisiKerusakan.High => CubicSpline.InterpolatePchipSorted(_x, yH).Interpolate(density),
            _ => throw new ArgumentException(null, nameof(type)),
        };
    }

    public static double Alur(double density, KondisiKerusakan type)
    {
        double[] yL = [0, 1, 2, 3, 4, 5, 5.5, 6, 7, 8, 9, 14, 17, 20, 21, 23, 24, 25, 26, 27, 34, 40, 44, 46, 47, 48, 49, 50, 100];
        double[] yM = [0, 4, 7, 9.5, 10.5, 12.5, 14, 15, 16, 17.5, 18.5, 25.5, 30, 33, 35.5, 37.5, 39.5, 41, 43, 44, 53.5, 58, 61, 63, 64.5, 65.5, 66, 67, 100];
        double[] yH = [0, 6, 12, 16, 18.5, 20.5, 22, 23.5, 25, 26.5, 28, 36.5, 42, 45.5, 49, 52, 54.5, 56.5, 59, 61, 73, 78.5, 82.5, 85, 86.5, 87.5, 88, 89, 100];

        return type switch
        {
            KondisiKerusakan.Low => CubicSpline.InterpolatePchipSorted(_x, yL).Interpolate(density),
            KondisiKerusakan.Medium => CubicSpline.InterpolatePchipSorted(_x, yM).Interpolate(density),
            KondisiKerusakan.High => CubicSpline.InterpolatePchipSorted(_x, yH).Interpolate(density),
            _ => throw new ArgumentException(null, nameof(type)),
        };
    }

    public static double Lubang(double density, KondisiKerusakan type)
    {
        double[] yL = [0, 2, 5, 7.5, 10, 12, 14, 15.5, 17, 18.5, 19.5, 29, 36, 40.5, 44, 47, 50, 52, 54, 55.5, 67.5, 75, 80, 84, 88, 91, 94, 96, 100];
        double[] yM = [0, 5, 10, 14, 18, 20.5, 23, 25.5, 27.5, 29.5, 31.5, 45, 54.5, 62, 68, 73, 77, 81, 84, 87, 100, 100, 100, 100, 100, 100, 100, 100, 100];
        double[] yH = [0, 20, 25, 31.5, 36, 40, 43, 46, 48, 50.5, 52.5, 66.5, 75.5, 83, 88, 92.5, 96.5, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100];

        return type switch
        {
            KondisiKerusakan.Low => CubicSpline.InterpolatePchipSorted(_x, yL).Interpolate(density),
            KondisiKerusakan.Medium => CubicSpline.InterpolatePchipSorted(_x, yM).Interpolate(density),
            KondisiKerusakan.High => CubicSpline.InterpolatePchipSorted(_x, yH).Interpolate(density),
            _ => throw new ArgumentException(null, nameof(type)),
        };
    }

    public static double PelepasanButir(double density, KondisiKerusakan type)
    {
        double[] yL = [0, 0, 0.25, 0.75, 1, 1.25, 1.5, 1.6, 1.75, 1.8, 2, 2.25, 2.5, 2.6, 3, 3.5, 3.75, 4, 4.25, 4.75, 7.5, 9.5, 11, 12, 13.5, 14, 15, 15.5, 100];
        double[] yM = [0, 4, 5.5, 6, 6.75, 7.25, 7.5, 7.75, 8, 8, 8.25, 10, 11.25, 12.5, 13.5, 14.5, 15.5, 16.5, 17.25, 18, 25, 29, 32.5, 35, 37.25, 39.5, 41, 42.5, 100];
        double[] yH = [0, 6, 8.5, 10, 11.5, 12.5, 13.5, 14, 15, 15.5, 16, 21, 24.5, 28, 30.5, 33, 35.5, 37.5, 40, 41.5, 55.5, 62, 66, 69, 71.5, 73.5, 75.5, 76.5, 100];

        return type switch
        {
            KondisiKerusakan.Low => CubicSpline.InterpolatePchipSorted(_x, yL).Interpolate(density),
            KondisiKerusakan.Medium => CubicSpline.InterpolatePchipSorted(_x, yM).Interpolate(density),
            KondisiKerusakan.High => CubicSpline.InterpolatePchipSorted(_x, yH).Interpolate(density),
            _ => throw new ArgumentException(null, nameof(type)),
        };
    }

    public static double RetakMemanjang(double density, KondisiKerusakan type)
    {
        double[] x = [0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 20, 30];

        double[] yL = [0, 0, 0, 0, 0, 0.5, 0.75, 1, 1.5, 2, 2.25, 5.5, 7.5, 10, 11.5, 12.5, 14, 15.5, 16.5, 17, 24, 29];
        double[] yM = [0, 0, 0.5, 2.5, 3.5, 4.5, 5.5, 6.5, 7.5, 8, 9, 14, 18, 20.5, 22.5, 25, 26.5, 28.5, 30, 31, 40, 44];
        double[] yH = [0, 4, 6.5, 9, 10, 12, 14, 15, 16.5, 17.5, 18.5, 28, 34, 40, 45, 50, 63, 66, 69, 72, 80, 87];

        return type switch
        {
            KondisiKerusakan.Low => CubicSpline.InterpolatePchipSorted(x, yL).Interpolate(density),
            KondisiKerusakan.Medium => CubicSpline.InterpolatePchipSorted(x, yM).Interpolate(density),
            KondisiKerusakan.High => CubicSpline.InterpolatePchipSorted(x, yH).Interpolate(density),
            _ => throw new ArgumentException(null, nameof(type)),
        };
    }

    public static double Tambalan(double density, KondisiKerusakan type)
    {
        double[] x = [0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 20, 30, 40, 50];

        double[] yL = [0, 0, 0, 0, 0.5, 0.75, 1, 1.5, 1.75, 2, 2.5, 4.5, 6.5, 8.5, 10, 11.25, 12.5, 14, 15, 16, 23, 27, 30, 33];
        double[] yM = [0, 3, 4, 5, 5.5, 7, 7.5, 8, 9, 9.5, 10, 14, 17, 20, 22, 24.5, 26.5, 28.5, 30, 31, 41, 48, 53.5, 57.5];
        double[] yH = [0, 6, 8.5, 10.5, 12.5, 14.5, 15.5, 17, 18, 19, 20, 26, 30, 34, 37.5, 41, 44, 46.5, 50, 51.5, 66.5, 74.5, 77, 80];

        return type switch
        {
            KondisiKerusakan.Low => CubicSpline.InterpolatePchipSorted(x, yL).Interpolate(density),
            KondisiKerusakan.Medium => CubicSpline.InterpolatePchipSorted(x, yM).Interpolate(density),
            KondisiKerusakan.High => CubicSpline.InterpolatePchipSorted(x, yH).Interpolate(density),
            _ => throw new ArgumentException(null, nameof(type)),
        };
    }

    public static double TdvToCdv(double tdv, int q = 1)
    {
        q = Math.Clamp(q, QMIN, QMAX);

        var x = new List<double[]>()
        {
            new double[] { 0, 50, 100 },
            new double[] { 14, 20, 30, 40, 50, 60, 70, 80, 90, 100, 110, 120, 130, 140, 150, 160, 166 },
            new double[] { 19, 20, 30, 40, 50, 60, 70, 80, 90, 100, 110, 120, 130, 140, 150, 160, 170, 180, 181},
            new double[] { 26, 30, 40, 50, 60, 70, 80, 90, 100, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200 },
            new double[] { 28, 30, 40, 50, 60, 70, 80, 90, 100, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200 },
            new double[] { 42, 50, 60, 70, 80, 90, 100, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200 },
            new double[] { 42, 50, 60, 70, 80, 90, 100, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200 }
        };

        var y = new List<double[]>()
        {
            new double[] { 0, 50, 100 },
            new double[] { 9, 14, 22, 29.5, 37.5, 44.5, 51, 58, 64.5, 71, 76.5, 81.5, 86, 90.5, 94.5, 98, 100 },
            new double[] { 9, 9.5, 17, 24, 31.5, 38, 44.5, 51, 57.5, 63.5, 69, 74, 79, 84, 88, 92.5, 96, 99.75, 100 },
            new double[] { 9, 12, 19.5, 26, 33, 39, 45, 52, 57.5, 63, 68.75, 74, 78.5, 83, 87, 90.5, 94, 96.5, 98 },
            new double[] { 9, 10, 16.5, 23, 29, 35.5, 40.5, 46.5, 52, 58, 63, 68, 72.5, 77, 81, 85, 88.5, 91, 94 },
            new double[] { 15.5, 20, 26, 32, 38, 44, 48.5, 54, 59, 64, 68, 73, 78, 81.5, 85, 88, 90 },
            new double[] { 15.5, 20, 26, 32, 38, 44, 48.5, 54, 59, 64, 68, 71, 74.5, 78, 80, 81, 82 },
        };

        return CubicSpline.InterpolatePchipSorted(x[q-1], y[q-1]).Interpolate(tdv);
    }

    public static KondisiKerusakan Kondisi(LabelEnum labelEnum, double luas, double panjang, double lebar)
    {
        var kondisi = panjang > lebar ? panjang : lebar;

        switch (labelEnum)
        {
            case LabelEnum.Alur:
                if (kondisi < 0.5) return KondisiKerusakan.Low;
                else if (kondisi >= 0.5 && kondisi <= 5) return KondisiKerusakan.Medium;
                else return KondisiKerusakan.High;
            case LabelEnum.Bleding:
                if (kondisi < 0.5) return KondisiKerusakan.Low;
                else if (kondisi >= 0.5 && kondisi <= 5) return KondisiKerusakan.Medium;
                else return KondisiKerusakan.High;
            case LabelEnum.Lubang:
                if (kondisi < 0.5) return KondisiKerusakan.Low;
                else if (kondisi >= 0.5 && kondisi <= 3) return KondisiKerusakan.Medium;
                else return KondisiKerusakan.High;
            case LabelEnum.PelepasanButiran:
                if (kondisi < 0.5) return KondisiKerusakan.Low;
                else if (kondisi >= 0.5 && kondisi <= 3) return KondisiKerusakan.Medium;
                else return KondisiKerusakan.High;
            case LabelEnum.RetakBuaya:
                if (kondisi < 0.5) return KondisiKerusakan.Low;
                else if (kondisi >= 0.5 && kondisi <= 5) return KondisiKerusakan.Medium;
                else return KondisiKerusakan.High;
            case LabelEnum.RetakMemanjang:
                if (kondisi < 0.5) return KondisiKerusakan.Low;
                else if (kondisi >= 0.5 && kondisi <= 5) return KondisiKerusakan.Medium;
                else return KondisiKerusakan.High;
            case LabelEnum.Tambalan:
                if (kondisi < 1) return KondisiKerusakan.Low;
                else if (kondisi >= 1 && kondisi <= 3) return KondisiKerusakan.Medium;
                else return KondisiKerusakan.High;
            default:
                throw new NotImplementedException();
        }
    }

    public static string KondisiPCI(double pci) =>
        pci switch
        {
            >= 0d and < 11d => "Gagal (Failed)",
            >= 11d and < 25d => "Sangat Buruk (Very Poor)",
            >= 26d and < 41d => "Buruk (Poor)",
            >= 41d and < 55d => "Sedang (Fair)",
            >= 56d and < 71d => "Baik (Good)",
            >= 71d and < 86d => "Sangat Baik (Very Good)",
            >= 86d and <= 100d => "Sempuran (Excelent)",
            _ => "",
        };

    public static string JenisPenangananPCI(double pci) =>
        pci switch
        {
            >= 0d and < 11d => "Rekonstruksi",
            >= 11d and < 25d => "Rekonstruksi",
            >= 26d and < 41d => "Berkala",
            >= 41d and < 55d => "Rutin",
            >= 56d and < 71d => "Rutin",
            >= 71d and < 86d => "Rutin",
            >= 86d and <= 100d => "Rutin",
            _ => ""
        };
}

public enum KondisiKerusakan
{
    High, Medium, Low
}
