using Devi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Devi.Controllers
{
    public class PortfolioController : Controller
    {
        private readonly AppDbContext _context;

        public PortfolioController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null || id < 1) return BadRequest();

            var portfolio = await _context.Portfolios
                .FirstOrDefaultAsync(p => p.Id == id);

            if (portfolio == null) return NotFound();   

            return View(portfolio);
        }
    }
}
