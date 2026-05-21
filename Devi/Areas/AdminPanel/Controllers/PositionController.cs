using Devi.Areas.AdminPanel.ViewModels;
using Devi.Data;
using Devi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Devi.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]
    public class PositionController : Controller
    {
        private readonly AppDbContext _context;

        public PositionController(AppDbContext context)
        {
           _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var position = await _context.Positions.ToListAsync();
            return View(position);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Position position)
        {
            if (!ModelState.IsValid) return View(position);

            await _context.Positions.AddAsync(position);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
