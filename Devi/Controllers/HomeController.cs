using Devi.Data;
using Devi.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Devi.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            HomeVM homeVM = new()
            {
                Services = await _context.Services.Where(s => !s.IsDeleted).ToListAsync(),
                Portfolios = await _context.Portfolios.Where(p => !p.IsDeleted).ToListAsync(),
                Employees = await _context.Employees.Where(e => !e.IsDeleted).Include(e => e.Position).Include(e => e.SocialMedias).ToListAsync()
            };
            return View(homeVM);
        }
    }
}
