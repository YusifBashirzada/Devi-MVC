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
    public class ServiceController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ServiceController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            List<Service> services = await _context.Services.Where(s => !s.IsDeleted).ToListAsync();
            return View(services);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ServiceCreateVM serviceCreateVM)
        {
            if (!serviceCreateVM.Photo.CheckFileType("image/"))
            {
                ModelState.AddModelError("Photo", "File type is incorrect!");
                return View();
            }

            if (!serviceCreateVM.Photo.CheckFileSize(FileSize.MB, 2))
            {
                ModelState.AddModelError("Photo", "File size must be less than 2 mb!");
                return View();
            }

            Service service = new()
            {
                Name = serviceCreateVM.Name,
                Description = serviceCreateVM.Description,
                Image = await serviceCreateVM.Photo.CreateFile(_env.WebRootPath, "assets", "img")
            };

            await _context.Services.AddAsync(service);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null || id < 1) return BadRequest();

            Service? service = await _context.Services.FirstOrDefaultAsync(s => s.Id == id);

            if (service == null) return NotFound(); 

            service.Image.DeleteFile(_env.WebRootPath, "assets", "img");
             
            _context.Remove(service);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Detail(int? id)
        {
            if (id is null || id < 1) return BadRequest();

            Service? service = await _context.Services
                .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);

            if (service == null) return NotFound();

            return View(service);
        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id is null || id < 1) return BadRequest();
            
            Service? existService = await _context.Services.Where(s => !s.IsDeleted).FirstOrDefaultAsync(s => s.Id == id);

            if (existService == null) return NotFound();

            ServiceUpdateVM serviceUpdateVM = new()
            {
                Name = existService.Name,
                Description = existService.Description,

            };

            return View(serviceUpdateVM);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int? id, ServiceUpdateVM serviceUpdateVM)
        {
            if (id == null || id < 1) return BadRequest();

            Service? existService = await _context.Services
                .Where(s => !s.IsDeleted)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (existService == null) return NotFound();

            if (serviceUpdateVM.Photo is not null)
            {
                if (!serviceUpdateVM.Photo.CheckFileType("image/"))
                {
                    ModelState.AddModelError("Photo", "File type is incorrect!");
                    return View(serviceUpdateVM);
                }

                if (!serviceUpdateVM.Photo.CheckFileSize(FileSize.MB, 2))
                {
                    ModelState.AddModelError("Photo", "File size must be less than 2 mb!");
                    return View(serviceUpdateVM);
                }
            }

            if (!ModelState.IsValid) return View(serviceUpdateVM);

            if (serviceUpdateVM.Photo is not null)
            {
                existService.Image.DeleteFile(_env.WebRootPath, "assets", "img");

                existService.Image = await serviceUpdateVM.Photo.CreateFile(_env.WebRootPath, "assets", "images", "website-images");
            }

            existService.Name = serviceUpdateVM.Name;
            existService.Description = serviceUpdateVM.Description;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        } 
    }
}
