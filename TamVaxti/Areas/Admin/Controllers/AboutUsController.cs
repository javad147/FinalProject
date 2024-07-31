using TamVaxti.Data;
using TamVaxti.Models;
using TamVaxti.Services.Interfaces;
using TamVaxti.ViewModels.AboutUs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using TamVaxti.Helpers.Enums;

namespace TamVaxti.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin,Admin")]
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

            var isEntryExixts = _context.About.FirstOrDefaultAsync();
            if (isEntryExixts != null)
            {
                return RedirectToAction("Index");
            }


            if (ModelState.IsValid)
            {
                await _aboutUsService.CreateAsync(model);
                return RedirectToAction("Index");
            }

            return View(model);
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
                    Id = t.Id,
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
            
            var result = await _aboutUsService.UpdateAsync(model.Id, model);
            Console.WriteLine(result);
            if (result)
                return Json(new { success = true, message = "Data processed successfully." });
            else
                return Json(new { success = false, message = "Error occurred while saving, Make sure all mandatory fields are entered" });
            //TempData["messageType"] = "success";
            //TempData["message"] = " Updated Successfully.";

            //return RedirectToAction(nameof(Index));

        }
    }
}
