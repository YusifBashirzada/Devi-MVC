using Devi.Areas.AdminPanel.ViewModels;
using Devi.Areas.AdminPanel.ViewModels;
using Devi.Data;
using Devi.Models;
using Devi.Utilities.Enums;
using Devi.Utilities.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Devi.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]
    public class PortfolioController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public PortfolioController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            List<Portfolio> portfolios = await _context.Portfolios.Where(p => !p.IsDeleted).ToListAsync();
            return View(portfolios);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(PortfolioCreateVM portfolioCreateVM)
        {
            if (portfolioCreateVM.Photo is null)
            {
                ModelState.AddModelError("Photo", "Photo is required!");
                return View(portfolioCreateVM); 
            }

            if (!portfolioCreateVM.Photo.CheckFileType("image/"))
            {
                ModelState.AddModelError("Photo", "File type is incorrect!");
                return View();
            }

            if (!portfolioCreateVM.Photo.CheckFileSize(FileSize.MB, 2))
            {
                ModelState.AddModelError("Photo", "File size must be less than 2 mb!");
                return View();
            }

            if (!ModelState.IsValid) return View(portfolioCreateVM);

            Portfolio portfolio = new()
            {
                Category = portfolioCreateVM.Category,
                Description = portfolioCreateVM.Description,
                Image = await portfolioCreateVM.Photo.CreateFile(_env.WebRootPath, "assets", "img")
            };

            await _context.Portfolios.AddAsync(portfolio);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null || id < 1) return BadRequest();

            Portfolio? portfolio = await _context.Portfolios.FirstOrDefaultAsync(p => p.Id == id);

            if (portfolio == null) return NotFound();

            portfolio.Image.DeleteFile(_env.WebRootPath, "assets", "img");

            _context.Remove(portfolio);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Detail(int? id)
        {
            if (id is null || id < 1) return BadRequest();

            Portfolio? portfolio = await _context.Portfolios
                .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);

            if (portfolio == null) return NotFound();

            return View(portfolio);
        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id is null || id < 1) return BadRequest();

            Portfolio? existPortfolio = await _context.Portfolios.Where(p => !p.IsDeleted).FirstOrDefaultAsync(p => p.Id == id);

            if (existPortfolio == null) return NotFound();

            PortfolioUpdateVM portfolioUpdateVM = new()
            {
                Category = existPortfolio.Category,
                Description = existPortfolio.Description,

            };

            return View(portfolioUpdateVM);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int? id, PortfolioUpdateVM portfolioUpdateVM)
        {
            if (id == null || id < 1) return BadRequest();

            Portfolio? existPortfolio = await _context.Portfolios
                .Where(p => !p.IsDeleted)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (existPortfolio == null) return NotFound();

            if (portfolioUpdateVM.Photo is not null)
            {
                if (!portfolioUpdateVM.Photo.CheckFileType("image/"))
                {
                    ModelState.AddModelError("Photo", "File type is incorrect!");
                    return View(portfolioUpdateVM);
                }

                if (!portfolioUpdateVM.Photo.CheckFileSize(FileSize.MB, 2))
                {
                    ModelState.AddModelError("Photo", "File size must be less than 2 mb!");
                    return View(portfolioUpdateVM);
                }
            }

            if (!ModelState.IsValid) return View(portfolioUpdateVM);

            if (portfolioUpdateVM.Photo is not null)
            {
                existPortfolio.Image.DeleteFile(_env.WebRootPath, "assets", "img");

                existPortfolio.Image = await portfolioUpdateVM.Photo.CreateFile(_env.WebRootPath, "assets", "images", "website-images");
            }

            existPortfolio.Category = portfolioUpdateVM.Category;
            existPortfolio.Description = portfolioUpdateVM.Description;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
