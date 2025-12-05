using DeteksiJalanRusak.Web.Database;
using DeteksiJalanRusak.Web.Entities;
using DeteksiJalanRusak.Web.Models.AnalisisModels;
using DeteksiJalanRusak.Web.Services.Toastr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DeteksiJalanRusak.Web.Controllers;

public class AnalisisController : Controller
{
    private readonly AppDbContext _context;
    private readonly IToastrNotificationService _notificationService;
    private readonly ILogger<AnalisisController> _logger;

    public AnalisisController(
        AppDbContext context,
        IToastrNotificationService notificationService,
        ILogger<AnalisisController> logger)
    {
        _context = context;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<IActionResult> Index() => View(await _context.TblAnalisis.ToListAsync());

    public async Task<IActionResult> Detail(int id)
    {
        var analisis = await _context
            .TblAnalisis
            .Include(x => x.DaftarSegmen).ThenInclude(x => x.DaftarFoto).ThenInclude(x => x.DaftarKerusakan)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (analisis is null) return NotFound();

        return View(analisis);
    }

    public IActionResult Tambah() => View(new TambahVM());

    [HttpPost]
    public async Task<IActionResult> Tambah(TambahVM vm)
    {
        if (!ModelState.IsValid) return View(vm);

        if (await _context.TblAnalisis.AnyAsync(x => x.Nama.ToLower() == vm.Nama.ToLower()))
        {
            ModelState.AddModelError(nameof(vm.Nama), "Sudah digunakan");
            return View(vm);
        }

        var analisis = new Analisis
        {
            Lokasi = vm.Lokasi,
            Nama = vm.Nama,
            Pembuat = vm.Pembuat
        };

        _context.TblAnalisis.Add(analisis);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _notificationService.AddError("Simpan Gagal");
            _logger.LogError(ex.ToString());

            return View(vm);
        }

        _notificationService.AddSuccess("Simpan Berhasil");
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var analisis = await _context.TblAnalisis.FirstOrDefaultAsync(x => x.Id == id);
        if (analisis is null) return NotFound();

        return View(new EditVM
        {
            Id = id,
            Nama = analisis.Nama,
            Lokasi = analisis.Lokasi,
            Pembuat = analisis.Pembuat
        });
    }

    [HttpPost]
    public async Task<IActionResult> Edit(EditVM vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var analisis = await _context.TblAnalisis.FirstOrDefaultAsync(x => x.Id == vm.Id);
        if (analisis is null) return NotFound();

        if (await _context.TblAnalisis.AnyAsync(x => x.Id != analisis.Id && x.Nama.ToLower() == vm.Nama.ToLower()))
        {
            ModelState.AddModelError(nameof(vm.Nama), "Sudah digunakan");
            return View(vm);
        }

        analisis.Nama = vm.Nama;
        analisis.Lokasi = vm.Lokasi;
        analisis.Pembuat = vm.Pembuat;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _notificationService.AddError("Simpan Gagal");
            _logger.LogError(ex.ToString());

            return View(vm);
        }

        _notificationService.AddSuccess("Simpan Berhasil");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Hapus(int id)
    {
        var analisis = await _context.TblAnalisis.FirstOrDefaultAsync(x => x.Id == id);
        if (analisis is null) return NotFound();

        _context.TblAnalisis.Remove(analisis);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _notificationService.AddError("Simpan Gagal");
            _logger.LogError(ex.ToString());

            return RedirectToAction(nameof(Index));
        }

        _notificationService.AddSuccess("Simpan Berhasil");
        return RedirectToAction(nameof(Index));
    }
}
