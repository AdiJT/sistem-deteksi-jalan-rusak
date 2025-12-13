using DeteksiJalanRusak.Web.Configurations;
using DeteksiJalanRusak.Web.Database;
using DeteksiJalanRusak.Web.Entities;
using DeteksiJalanRusak.Web.Extensions;
using DeteksiJalanRusak.Web.Models;
using DeteksiJalanRusak.Web.Models.SegmenModels;
using DeteksiJalanRusak.Web.Services.FileServices;
using DeteksiJalanRusak.Web.Services.Toastr;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkiaSharp;
using YoloDotNet;
using YoloDotNet.Core;
using YoloDotNet.Enums;
using YoloDotNet.Extensions;
using YoloDotNet.Models;

namespace DeteksiJalanRusak.Web.Controllers;

public class SegmenController : Controller
{
    public const double LEBAR_KERTASA4_CM = 21;
    public const double PANJANG_KERTASA4_CM = 29.7;
    public const double LEBAR_KERTASA4_M = LEBAR_KERTASA4_CM / 100;
    public const double PANJANG_KERTASA4_M = PANJANG_KERTASA4_CM / 100;
    public const double LUAS_KERTASA4_CM2 = LEBAR_KERTASA4_CM * PANJANG_KERTASA4_CM;
    public const double LUAS_KERTASA4_M2 = LUAS_KERTASA4_CM2 / 10000;

    private readonly SegmentationDrawingOptions _drawingOptions;
    private readonly ModelConfigurationOptions _modelConfigurationOptions;
    private readonly AppDbContext _appDbContext;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IFileService _fileService;
    private readonly IToastrNotificationService _notificationService;
    private readonly ILogger<SegmenController> _logger;

    public SegmenController(
        ModelConfigurationOptions modelConfigurationOptions,
        AppDbContext appDbContext,
        IWebHostEnvironment webHostEnvironment,
        IFileService fileService,
        IToastrNotificationService notificationService,
        ILogger<SegmenController> logger)
    {
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
        _appDbContext = appDbContext;
        _webHostEnvironment = webHostEnvironment;
        _fileService = fileService;
        _notificationService = notificationService;
        _logger = logger;
    }

    public IActionResult Tambah(int idAnalisis) => View(new TambahVM { IdAnalisis = idAnalisis });

    [HttpPost]
    [RequestSizeLimit(100_000_000)]
    public async Task<IActionResult> Tambah(TambahVM vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var analisis = await _appDbContext.TblAnalisis.FirstOrDefaultAsync(x => x.Id == vm.IdAnalisis);
        if (analisis is null)
        {
            _notificationService.AddError("Analisis tidak ditemukan");
            return RedirectToActionPermanent("Index", "Analisis");
        }

        var tasks = vm.FormFiles.Select(x => _fileService.ProcessFormFile<TambahVM>(x, [".jpg", ".jpeg", ".png"], 0, long.MaxValue)).ToList();
        var daftarFoto = await Task.WhenAll(tasks);

        var failures = daftarFoto.Where(x => x.IsFailure);
        if (failures.Any())
        {
            ModelState.AddModelError(nameof(vm.FormFiles), string.Join("\n", failures.Select(x => x.Error.Message)));
            return View(vm);
        }

        var segmen = new Segmen
        {
            LuasSampel = vm.LuasSampel,
            Analisis = analisis,
            DaftarFoto = []
        };

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

            var fotoKerusakan = new FotoKerusakan
            {
                AdaKertas = false,
                FileName = vm.FormFiles[Array.IndexOf(daftarFoto, foto)].FileName,
                ImageBase64 = image.ToBase64String(),
                Lebar = image.Width,
                Tinggi = image.Height,
                DaftarKerusakan = [.. results.Where(x => x.Label.Name != LabelEnum.Kertas.ToString()).Select(x => new Kerusakan
                {
                    Label = x.Label.Name.DehumanizeTo<LabelEnum>(),
                    MaskBase64 = x.BitPackedPixelMask.UnpackToBitmap(x.BoundingBox.Width, x.BoundingBox.Height).ToBase64String(),
                    Luas = 0,
                    Panjang = 0,
                    Lebar = 0,
                    LebarPiksel = 0,
                    TinggiPiksel = 0,
                })]
            };

            var kertas = results.FirstOrDefault(s => s.Label.Name == LabelEnum.Kertas.ToString());

            if (kertas is not null)
            {
                var totalPixel = kertas.BitPackedPixelMask.Count(x => x != 0);
                var m2PerPixel = LUAS_KERTASA4_M2 / totalPixel;

                var mPerPixel = PANJANG_KERTASA4_M /
                    (kertas.BoundingBox.Width > kertas.BoundingBox.Height ? kertas.BoundingBox.Width : kertas.BoundingBox.Height);

                fotoKerusakan.AdaKertas = true;

                fotoKerusakan.Mask64Kertas = kertas
                    .BitPackedPixelMask.UnpackToBitmap(kertas.BoundingBox.Width, kertas.BoundingBox.Height).ToBase64String();

                fotoKerusakan.M2PerPiksel = m2PerPixel;
                fotoKerusakan.MPerPiksel = mPerPixel;
                fotoKerusakan.LuasKertas = kertas.BitPackedPixelMask.Count(x => x != 0) * m2PerPixel;
                fotoKerusakan.PanjangKertas = kertas.BoundingBox.Height * mPerPixel;
                fotoKerusakan.LebarKertas = kertas.BoundingBox.Width * mPerPixel;
                fotoKerusakan.TinggiPikselKertas = kertas.BoundingBox.Height;
                fotoKerusakan.LebarPikselKertas = kertas.BoundingBox.Width;

                fotoKerusakan.DaftarKerusakan = [];

                foreach (var segmentation in results.Where(x => x.Label.Name != LabelEnum.Kertas.ToString()))
                {
                    fotoKerusakan.DaftarKerusakan.Add(new Kerusakan
                    {
                        Label = segmentation.Label.Name.DehumanizeTo<LabelEnum>(),
                        MaskBase64 = segmentation
                            .BitPackedPixelMask
                            .UnpackToBitmap(segmentation.BoundingBox.Width, segmentation.BoundingBox.Height)
                            .ToBase64String(),
                        Luas = segmentation.BitPackedPixelMask.Count(x => x != 0) * m2PerPixel,
                        Panjang = segmentation.BoundingBox.Height * mPerPixel,
                        Lebar = segmentation.BoundingBox.Width * mPerPixel,
                        TinggiPiksel = segmentation.BoundingBox.Height,
                        LebarPiksel = segmentation.BoundingBox.Width,
                    });
                }
            }

            segmen.DaftarFoto.Add(fotoKerusakan);
        }

        _appDbContext.TblSegmen.Add(segmen);

        try
        {
            await _appDbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _notificationService.AddError("Simpan Gagal");
            _logger.LogError(ex.ToString());
            return View(vm);
        }

        _notificationService.AddSuccess("Simpan Berhasil");

        return RedirectToActionPermanent("Detail", new { id = segmen.Id });
    }

    public async Task<IActionResult> Edit(int id, string? returnUrl = null)
    {
        var segmen = await _appDbContext.TblSegmen.Include(x => x.Analisis).FirstOrDefaultAsync(x => x.Id == id);
        if (segmen is null) return NotFound();

        return View(new EditVM
        {
            Id = id,
            LuasSampel = segmen.LuasSampel,
            ReturnUrl = returnUrl ?? Url.ActionLink("Detail", "Analisis", new { id = segmen.Analisis.Id })!
        });
    }

    [HttpPost]
    public async Task<IActionResult> Edit(EditVM vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var segmen = await _appDbContext.TblSegmen.Include(x => x.Analisis).FirstOrDefaultAsync(x => x.Id == vm.Id);
        if (segmen is null) return NotFound();

        segmen.LuasSampel = vm.LuasSampel;

        try
        {
            await _appDbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _notificationService.AddError("Simpan Gagal");
            _logger.LogError(ex.ToString());
            return View(vm);
        }

        _notificationService.AddSuccess("Simpan Berhasil");
        return RedirectPermanent(vm.ReturnUrl);
    }

    public async Task<IActionResult> Detail(int id)
    {
        var segmen = await _appDbContext
            .TblSegmen
            .Include(x => x.Analisis)
            .Include(x => x.DaftarFoto).ThenInclude(x => x.DaftarKerusakan)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (segmen is null) return NotFound();

        return View(segmen);
    }

    [HttpPost]
    public async Task<IActionResult> Hapus(int id)
    {
        var segmen = await _appDbContext.TblSegmen.Include(x => x.Analisis).FirstOrDefaultAsync(x => x.Id == id);
        if (segmen is null) return NotFound();

        var idAnalisis = segmen.Analisis.Id;

        _appDbContext.TblSegmen.Remove(segmen);

        try
        {
            await _appDbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _notificationService.AddError("Simpan Gagal");
            _logger.LogError(ex.ToString());

            return RedirectToActionPermanent("Detail", "Analisis", new { id = idAnalisis });
        }

        _notificationService.AddSuccess("Simpan Berhasil");

        return RedirectToActionPermanent("Detail", "Analisis", new { id = idAnalisis });
    }
}
