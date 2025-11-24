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

            if (vm.FormFile is null)
            {
                ModelState.AddModelError(nameof(IndexVM.FormFile), "Foto harus dipilih");
                return View(vm);
            }

            var foto = await _fileService.ProcessFormFile<IndexVM>(
                vm.FormFile,
                [".jpeg", ".jpg", ".png"],
                0,
                long.MaxValue);

            if (foto.IsFailure)
            {
                ModelState.AddModelError(nameof(IndexVM.FormFile), foto.Error.Message);
                return View(vm);
            }

            using var yolo = new Yolo(new YoloOptions
            {
                OnnxModel = _modelConfigurationOptions.FilePath,
                ExecutionProvider = new CpuExecutionProvider(),
                ImageResize = ImageResize.Stretched,
                SamplingOptions = new(SKFilterMode.Nearest, SKMipmapMode.None)
            });

            using var image = SKBitmap.Decode(foto.Value);
            var results = yolo.RunSegmentation(image, confidence: 0.5, pixelConfedence: 0.5, iou: 0.7);
            image.Draw(results, _drawingOptions);

            vm.ResultVM = new ResultVM
            {
                FileName = vm.FormFile.FileName,
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

                vm.ResultVM.Kertas = new Kerusakan
                {
                    Segmentation = kertas,
                    Luas = kertas.BitPackedPixelMask.Count(x => x != 0) * m2PerPixel,
                    Panjang = kertas.BoundingBox.Height * mPerPixel,
                    Lebar = kertas.BoundingBox.Width * mPerPixel,
                };

                vm.ResultVM.M2PerPiksel = m2PerPixel;
                vm.ResultVM.MPerPiksel = mPerPixel;

                foreach(var kerusakan in vm.ResultVM.Results)
                {
                    kerusakan.Luas = kerusakan.Segmentation.BitPackedPixelMask.Count(x => x != 0) * m2PerPixel;
                    kerusakan.Panjang = kerusakan.Segmentation.BoundingBox.Height * mPerPixel;
                    kerusakan.Lebar = kerusakan.Segmentation.BoundingBox.Width * mPerPixel;
                    kerusakan.DistressDensity = kerusakan.Luas / vm.LuasSampel;
                }
            }

            return View(vm);
        }
    }
}
