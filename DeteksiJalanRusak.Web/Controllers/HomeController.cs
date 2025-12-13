using DeteksiJalanRusak.Web.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DeteksiJalanRusak.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(
            AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index() => View(await _context.TblAnalisis.ToListAsync());

        public IActionResult Panduan() => View();
    }
}
