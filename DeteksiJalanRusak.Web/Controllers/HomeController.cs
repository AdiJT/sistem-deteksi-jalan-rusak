using DeteksiJalanRusak.Web.Configurations;
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
        private const double LUAS_KERTAS_M2 = 0.0006237;

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
            var results = yolo.RunSegmentation(image, confidence: 0.3, pixelConfedence: 0.5, iou: 0.7);
            image.Draw(results, _drawingOptions);

            vm.ResultVM = new ResultVM
            {
                FileName = vm.FormFile.FileName,
                ImageBase64 = $"data:image/png;base64,{Convert.ToBase64String(image.Encode(SKEncodedImageFormat.Png, 100).ToArray())}",
                Results = [.. results.Select(r => new Kerusakan { Segmentation = r, AdaUkuran = false })]
            };

            var kertas = results.FirstOrDefault(s => s.Label.Name == LabelEnum.Kertas.ToString());

            if (kertas is not null)
            {
                var totalPixel = kertas.BitPackedPixelMask.Count(x => x != 0);
                var luasPerPixel = LUAS_KERTAS_M2 / totalPixel;
                var panjangPerPixel = Math.Sqrt(luasPerPixel);

                vm.ResultVM.Results = [.. vm.ResultVM.Results.Select(x => new Kerusakan
                {
                    Segmentation = x.Segmentation,
                    AdaUkuran = true,
                    Luas = x.Segmentation.BitPackedPixelMask.Count(x => x != 0) * luasPerPixel,
                    Panjang = x.Segmentation.BoundingBox.Height * panjangPerPixel,
                    Lebar = x.Segmentation.BoundingBox.Width * panjangPerPixel,
                })];
            }

            return View(vm);
        }
    }
}
