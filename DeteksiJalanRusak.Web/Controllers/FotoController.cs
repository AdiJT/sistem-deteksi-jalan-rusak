using DeteksiJalanRusak.Web.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DeteksiJalanRusak.Web.Controllers;

public class FotoController : Controller
{
    private readonly AppDbContext _context;

    public FotoController(AppDbContext context)
    {
        _context = context;
    }

    [Route("[action]")]
    public async Task<IActionResult> FotoKerusakan(int id)
    {
        var foto = await _context.TblFoto.FirstOrDefaultAsync(x => x.Id == id);
        if (foto is null) return NotFound();

        return File(foto.Image, "image/jpg");
    }

    [Route("[action]")]
    public async Task<IActionResult> MaskKertas(int id)
    {
        var foto = await _context.TblFoto.FirstOrDefaultAsync(x => x.Id == id);
        if (foto is null || !foto.AdaKertas) return NotFound();

        return File(foto.MaskKertas, "image/jpg");
    }

    [Route("[action]")]
    public async Task<IActionResult> MaskKerusakan(int id)
    {
        var kerusakan = await _context.TblKerusakan.FirstOrDefaultAsync(x => x.Id == id);
        if (kerusakan is null) return NotFound();

        return File(kerusakan.Mask, "image/jpg");
    }
}
