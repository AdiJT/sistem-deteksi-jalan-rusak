using DeteksiJalanRusak.Web.Configurations;
using DeteksiJalanRusak.Web.Extensions;
using DeteksiJalanRusak.Web.Models;
using DeteksiJalanRusak.Web.Models.Home;
using DeteksiJalanRusak.Web.Services.FileServices;
using Microsoft.AspNetCore.Mvc;
using SkiaSharp;
using YoloDotNet;
using YoloDotNet.Core;
using YoloDotNet.Enums;
using YoloDotNet.Extensions;
using YoloDotNet.Models;

namespace DeteksiJalanRusak.Web.Controllers
{
    public class HomeController : Controller
    {
        public const double LEBAR_KERTASA4_CM = 21;
        public const double PANJANG_KERTASA4_CM = 29.7;
        public const double LEBAR_KERTASA4_M = LEBAR_KERTASA4_CM / 100;
        public const double PANJANG_KERTASA4_M = PANJANG_KERTASA4_CM / 100;
        public const double LUAS_KERTASA4_CM2 = LEBAR_KERTASA4_CM * PANJANG_KERTASA4_CM;
        public const double LUAS_KERTASA4_M2 = LUAS_KERTASA4_CM2 / 10000;

        private readonly ILogger<HomeController> _logger;
        private readonly IFileService _fileService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly SegmentationDrawingOptions _drawingOptions;
        private readonly ModelConfigurationOptions _modelConfigurationOptions;

        public HomeController(
            ILogger<HomeController> logger,
            IFileService fileService,
            IWebHostEnvironment webHostEnvironment,
            ModelConfigurationOptions modelConfigurationOptions)
        {
            _logger = logger;
            _fileService = fileService;
            _webHostEnvironment = webHostEnvironment;
            _drawingOptions = new SegmentationDrawingOptions
            {
                DrawBoundingBoxes = true,
                DrawConfidenceScore = true,
                DrawLabels = true,
                EnableFontShadow = true,
                Font = SKTypeface.Default,
                FontSize = 18,
                FontColor = SKColors.White,
                DrawLabelBackground = true,
                EnableDynamicScaling = true,
                BorderThickness = 2,
                BoundingBoxOpacity = 128,
                DrawSegmentationPixelMask = true
            };
            _modelConfigurationOptions = modelConfigurationOptions;
        }

        public IActionResult Index()
        {
            return View(new IndexVM());
        }

        [HttpPost]
        public async Task<IActionResult> Index(IndexVM vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var tasks = vm.FormFiles.Select(x => _fileService.ProcessFormFile<IndexVM>(x, [".jpg", ".jpeg", ".png"], 0, long.MaxValue)).ToList();
            var daftarFoto = await Task.WhenAll(tasks);

            var failures = daftarFoto.Where(x => x.IsFailure);
            if (failures.Any())
            {
                ModelState.AddModelError(nameof(IndexVM.FormFiles), string.Join("\n", failures.Select(x => x.Error.Message)));
                return View(vm);
            }

            using var yolo = new Yolo(new YoloOptions
            {
                OnnxModel = _modelConfigurationOptions.FilePath,
                ExecutionProvider = new CpuExecutionProvider(),
                ImageResize = ImageResize.Stretched,
                SamplingOptions = new(SKFilterMode.Nearest, SKMipmapMode.None)
            });

            foreach (var foto in daftarFoto)
            {
                using var image = SKBitmap.Decode(foto.Value);
                var results = yolo.RunSegmentation(image, confidence: 0.5, pixelConfedence: 0.5, iou: 0.7);
                image.Draw(results, _drawingOptions);

                var resultVM = new ResultVM
                {
                    FileName = vm.FormFiles[Array.IndexOf(daftarFoto, foto)].FileName,
                    ImageBase64 = image.ToBase64String(),
                    Lebar = image.Width,
                    Tinggi = image.Height,
                    Kertas = null,
                    Results = [.. results.Where(r => r.Label.Name != LabelEnum.Kertas.ToString()).Select(r => new Kerusakan { Segmentation = r })]
                };

                var kertas = results.FirstOrDefault(s => s.Label.Name == LabelEnum.Kertas.ToString());

                if (kertas is not null)
                {
                    var totalPixel = kertas.BitPackedPixelMask.Count(x => x != 0);
                    var m2PerPixel = LUAS_KERTASA4_M2 / totalPixel;

                    var mPerPixel = PANJANG_KERTASA4_M /
                        (kertas.BoundingBox.Width > kertas.BoundingBox.Height ? kertas.BoundingBox.Width : kertas.BoundingBox.Height);

                    resultVM.Kertas = new Kerusakan
                    {
                        Segmentation = kertas,
                        Luas = kertas.BitPackedPixelMask.Count(x => x != 0) * m2PerPixel,
                        Panjang = kertas.BoundingBox.Height * mPerPixel,
                        Lebar = kertas.BoundingBox.Width * mPerPixel,
                    };

                    resultVM.M2PerPiksel = m2PerPixel;
                    resultVM.MPerPiksel = mPerPixel;

                    foreach (var kerusakan in resultVM.Results)
                    {
                        kerusakan.Luas = kerusakan.Segmentation.BitPackedPixelMask.Count(x => x != 0) * m2PerPixel;
                        kerusakan.Panjang = kerusakan.Segmentation.BoundingBox.Height * mPerPixel;
                        kerusakan.Lebar = kerusakan.Segmentation.BoundingBox.Width * mPerPixel;
                    }
                }

                vm.ResultVMs.Add(resultVM);
            }

            vm.DensityDVs = [.. vm.ResultVMs
                .Where(x => x.Kertas is not null)
                .SelectMany(x => x.Results)
                .GroupBy(x => new { x.Label, x.Kondisi })
                .Select(x => new DensityDV
                {
                    DistressDensity = x.Sum(y => y.LuasPersegi) / vm.LuasSampel * 100,
                    TotalLuas = x.Sum(y => y.LuasPersegi),
                    Kondisi = x.Key.Kondisi,
                    Label = x.Key.Label
                })
                .OrderByDescending(x => x.DeductValue)];

            if (vm.DensityDVs.Count != 0)
            {
                var q = Math.Clamp(vm.DensityDVs.Count(x => x.DeductValue > 2), DistressDensityToDV.QMIN, DistressDensityToDV.QMAX);
                var dvMax = vm.DensityDVs.Select(x => x.DeductValue).Max();
                var mi = 1 + (9 / 98 * (100 - dvMax));
                var tdv = vm.DensityDVs.Sum(x => x.DeductValue);
                var cdvMax = DistressDensityToDV.TdvToCdv(tdv, q);

                for (int i = q - 1; i >= 1; i--)
                {
                    tdv = vm.DensityDVs.Take(i).Sum(x => x.DeductValue) + 2 * q - i;
                    cdvMax = Math.Max(cdvMax, DistressDensityToDV.TdvToCdv(tdv, i));
                }

                var pci = 100 - cdvMax;

                vm.PCIResult = new PCIResult
                {
                    PCI = pci,
                    CDVMax = cdvMax,
                    TDV = vm.DensityDVs.Sum(x => x.DeductValue),
                    MI = mi,
                };
            }

            return View(vm);
        }
    }
}
