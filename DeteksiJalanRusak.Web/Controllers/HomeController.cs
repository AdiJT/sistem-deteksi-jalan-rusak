using DeteksiJalanRusak.Web.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DeteksiJalanRusak.Web.Controllers
{
    public class HomeController : Controller
    {
        
        private readonly AppDbContext _appDbContext;

        public HomeController(AppDbContext appDbContext)
        {
            
            _appDbContext = appDbContext;
        }

        public async Task<IActionResult> Index() => View(await _appDbContext.TblAnalisis.ToListAsync());
    }
}
