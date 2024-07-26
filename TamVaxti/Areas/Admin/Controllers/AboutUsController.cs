using TamVaxti.Data;
using TamVaxti.Models;
using TamVaxti.Services.Interfaces;
using TamVaxti.ViewModels.AboutUs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TamVaxti.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AboutUsController : Controller
    {

        private readonly IAboutUsService _aboutUsService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly AppDbContext _context;

        public AboutUsController(IAboutUsService aboutUsService, IWebHostEnvironment webHostEnvironment, AppDbContext context)
        {
            _aboutUsService = aboutUsService;
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }

        public IActionResult Index()
        {
            var model = _aboutUsService.GetAllAboutUs();
            return View(model);
        }

        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(AboutUsCreateVM model)
        {
            if (ModelState.IsValid)
            {
                await _aboutUsService.CreateAsync(model);
                return RedirectToAction("Index");
            }

            return View(model);
        }




        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            // Validate the ID
            if (id <= 0)
            {
                return BadRequest("Invalid ID.");
            }

            // Find the AboutUs record
            var aboutUs = await _context.About.FindAsync(id);
            if (aboutUs == null)
            {
                return NotFound("AboutUs record not found.");
            }

            try
            {
                // Find and remove related history records
                var aboutUsHistory = _context.AboutUsHistory.Where(h => h.AboutId == id).ToList();
                if (aboutUsHistory != null && aboutUsHistory.Any())
                {
                    _context.AboutUsHistory.RemoveRange(aboutUsHistory);
                }

                // Find and remove related team records
                var aboutUsTeam = _context.AboutUsTeam.Where(t => t.AboutId == id).ToList();
                if (aboutUsTeam != null && aboutUsTeam.Any())
                {
                    _context.AboutUsTeam.RemoveRange(aboutUsTeam);
                }



                // Save changes to the database
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Log and handle the exception
                // Example: Log.Error(ex, "Error deleting records.");
                return StatusCode(500, "Internal server error occurred.");
            }
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var aboutUs = await _aboutUsService.GetAboutUsByIdAsync(id);
            if (aboutUs == null)
            {
                return NotFound();
            }

            var model = new AboutUsCreateVM
            {
                Title = aboutUs.Title,
                Description = aboutUs.Description,
                ImageName = aboutUs.ImageName,
                AboutHistory = aboutUs.AboutHistory.Select(h => new AboutUsHistoryVM
                {
                    Id = h.Id,
                    Title = h.Title,
                    Description = h.Description,
                    ImageName = h.ImageName
                }).ToList(),
                AboutUsTeam = aboutUs.AboutUsTeam.Select(t => new AboutUsTeamVM
                {
                    Id= t.Id,
                    Title = t.Title,
                    Role = t.Role,
                    ImageName = t.ImageName
                }).ToList()
            };

            return View(model);
        }


        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Edit(AboutUsCreateVM model)
        {
          
                await _aboutUsService.UpdateAsync(model.Id, model);
                return RedirectToAction("Index");
            
        }
    }
}
