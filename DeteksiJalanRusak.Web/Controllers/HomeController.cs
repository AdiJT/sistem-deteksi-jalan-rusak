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

            var dvMax = 0d;

            foreach(var foto in daftarFoto)
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
                        kerusakan.DistressDensity = kerusakan.Luas / vm.LuasSampel;
                    }

                    dvMax = Math.Max(dvMax, resultVM.Results.Aggregate(0d, (max, x) => Math.Max(max, x.DeductValue)));
                }

                vm.ResultVMs.Add(resultVM);
            }

            var resultWithMeasure = vm.ResultVMs.Where(x => x.Kertas is not null).ToList();

            if (resultWithMeasure.Count == 0) return View(vm);

            var daftarDV = resultWithMeasure
                .SelectMany(x => x.Results)
                .Select(x => x.DeductValue)
                .OrderByDescending(x => x)
                .Take(DistressDensityToDV.QMAX)
                .ToList();

            var q = Math.Clamp(daftarDV.Count(x => x > 2), DistressDensityToDV.QMIN, DistressDensityToDV.QMAX);

            var mi = 1 + (9 / 98 * (100 - dvMax));
            var tdv = daftarDV.Sum();
            var cdvMax = DistressDensityToDV.TdvToCdv(tdv, q);

            for(int i = q - 1; i >= 1; i--)
            {
                tdv = daftarDV.Take(i).Sum() + 2 * q - i;
                cdvMax = Math.Max(cdvMax, DistressDensityToDV.TdvToCdv(tdv, i));
            }

            var pci = 100 - cdvMax;

            vm.PCI = pci;
            vm.CDVMax = cdvMax;
            vm.TDV = daftarDV.Sum();
            vm.MI = mi;

            return View(vm);
        }
    }
}
