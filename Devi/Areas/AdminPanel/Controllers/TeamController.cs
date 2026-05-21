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
    public class TeamController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public TeamController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            var employees = await _context.Employees
                .Where(e => !e.IsDeleted)
                .Include(e => e.Position)
                .ToListAsync();

            return View(employees);
        }

        public async Task<IActionResult> Create()
        {
            EmployeeCreateVM employeeCreateVM = new()
            {
                Positions = await _context.Positions.ToListAsync()
            };

            return View(employeeCreateVM);
        }

        [HttpPost]
        public async Task<IActionResult> Create(EmployeeCreateVM employeeCreateVM)
        {
            employeeCreateVM.Positions = await _context.Positions.ToListAsync();

            if (!ModelState.IsValid) return View(employeeCreateVM);

            if (!employeeCreateVM.Photo.CheckFileType("image/"))
            {
                ModelState.AddModelError("Photo", "File type is incorrect!");
                return View(employeeCreateVM);
            }

            Employee employee = new()
            {
                Name = employeeCreateVM.Name,
                PositionId = employeeCreateVM.PositionId,
                ImageUrl = await employeeCreateVM.Photo.CreateFile(_env.WebRootPath, "assets", "img"),
                SocialMedias = new List<SocialMedia>()
            };


            if (!string.IsNullOrEmpty(employeeCreateVM.TwitterUrl))
            {
                employee.SocialMedias.Add(new SocialMedia { Name = "Twitter", Url = employeeCreateVM.TwitterUrl });
            }

            if (!string.IsNullOrEmpty(employeeCreateVM.FacebookUrl))
            {
                employee.SocialMedias.Add(new SocialMedia { Name = "Facebook", Url = employeeCreateVM.FacebookUrl });
            }

            if (!string.IsNullOrEmpty(employeeCreateVM.InstagramUrl))
            {
                employee.SocialMedias.Add(new SocialMedia { Name = "Instagram", Url = employeeCreateVM.InstagramUrl });
            }
            if (!string.IsNullOrEmpty(employeeCreateVM.LinkedinUrl))
            {
                employee.SocialMedias.Add(new SocialMedia { Name = "Linkedin", Url = employeeCreateVM.LinkedinUrl });
            }

            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id is null || id < 1) return BadRequest();

            Employee? existEmployee = await _context.Employees.Where(e => !e.IsDeleted).Include(e => e.SocialMedias).FirstOrDefaultAsync(e => e.Id == id);

            if (existEmployee == null) return NotFound();

            var socialMedias = existEmployee.SocialMedias?.ToList() ?? new List<SocialMedia>();

            EmployeeUpdateVM employeeUpdateVM = new()
            {
                Name = existEmployee.Name,
                PositionId = existEmployee.PositionId,
                Positions = await _context.Positions.ToListAsync(),
                TwitterUrl = socialMedias.FirstOrDefault(s => s.Name == "Twitter")?.Url,
                FacebookUrl = socialMedias.FirstOrDefault(s => s.Name == "Facebook")?.Url,
                InstagramUrl = socialMedias.FirstOrDefault(s => s.Name == "Instagram")?.Url,
                LinkedinUrl = socialMedias.FirstOrDefault(s => s.Name == "Linkedin")?.Url
            };

            return View(employeeUpdateVM);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int? id, EmployeeUpdateVM employeeUpdateVM)
        {
            if (id is null || id < 1) return BadRequest();

            Employee? existEmployee = await _context.Employees.Where(e => !e.IsDeleted).FirstOrDefaultAsync(e => e.Id == id);

            if (existEmployee == null) return NotFound();

            employeeUpdateVM.Positions = await _context.Positions.ToListAsync();

            if (employeeUpdateVM.Photo is not null)
            {
                if (!employeeUpdateVM.Photo.CheckFileType("image/"))
                {
                    employeeUpdateVM.Positions = await _context.Positions.ToListAsync();
                    ModelState.AddModelError("Photo", "File type is incorrect!");
                    return View(employeeUpdateVM);
                }

                if (!employeeUpdateVM.Photo.CheckFileSize(FileSize.MB, 1))
                {
                    employeeUpdateVM.Positions = await _context.Positions.ToListAsync();
                    ModelState.AddModelError("Photo", "File size must be less than 2 mb!");
                    return View(employeeUpdateVM);
                }
            }

            if (!ModelState.IsValid) return View(employeeUpdateVM);

            if (employeeUpdateVM.Photo is not null)
            {
                existEmployee.ImageUrl.DeleteFile(_env.WebRootPath, "assets", "img");

                existEmployee.ImageUrl = await employeeUpdateVM.Photo.CreateFile(_env.WebRootPath, "assets", "img");
            }

            existEmployee.Name = employeeUpdateVM.Name;
            existEmployee.PositionId = employeeUpdateVM.PositionId;

            if (existEmployee.SocialMedias != null && existEmployee.SocialMedias.Any())
            {
                _context.SocialMedias.RemoveRange(existEmployee.SocialMedias);
            }

            existEmployee.SocialMedias = new List<SocialMedia>();

            if (!string.IsNullOrEmpty(employeeUpdateVM.TwitterUrl))
            {
                SocialMedia twitter = new() { Name = "Twitter", Url = employeeUpdateVM.TwitterUrl, EmployeeId = existEmployee.Id };
                await _context.SocialMedias.AddAsync(twitter);
            }

            if (!string.IsNullOrEmpty(employeeUpdateVM.FacebookUrl))
            {
                SocialMedia facebook = new() { Name = "Facebook", Url = employeeUpdateVM.FacebookUrl, EmployeeId = existEmployee.Id };
                await _context.SocialMedias.AddAsync(facebook);
            }

            if (!string.IsNullOrEmpty(employeeUpdateVM.InstagramUrl))
            {
                SocialMedia instagram = new() { Name = "Instagram", Url = employeeUpdateVM.InstagramUrl, EmployeeId = existEmployee.Id };
                await _context.SocialMedias.AddAsync(instagram);
            }

            if (!string.IsNullOrEmpty(employeeUpdateVM.LinkedinUrl))
            {
                SocialMedia linkedin = new() { Name = "Linkedin", Url = employeeUpdateVM.LinkedinUrl, EmployeeId = existEmployee.Id };
                await _context.SocialMedias.AddAsync(linkedin);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null || id < 1) return BadRequest();

            Employee? employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null) return NotFound();

            employee.ImageUrl.DeleteFile(_env.WebRootPath, "assets", "img");

            _context.Remove(employee);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Detail(int? id)
        {
            if (id is null || id < 1) return BadRequest();

            Employee? employee = await _context.Employees.Include(e => e.SocialMedias).FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);

            if (employee == null) return NotFound();

            return View(employee);
        }
    }
}
