using TamVaxti.Data;
using TamVaxti.Helpers.Extensions;
using TamVaxti.Models;
using TamVaxti.ViewModels.Sliders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authorization;
using TamVaxti.Helpers.Enums;

namespace TamVaxti.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class SliderController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        public SliderController(AppDbContext context,
                                IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string searchString)
        {
            var query = _context.Sliders
                .Where(s => !s.SoftDeleted);

            if (!string.IsNullOrEmpty(searchString))
            {
                string lowerSearchString = searchString.ToLower();

                query = query.Where(s => s.SliderTitle.ToLower().Contains(lowerSearchString) ||
                                          s.SliderDescription.ToLower().Contains(lowerSearchString));
            }

            List<SliderVM> sliders = await query
                .Select(s => new SliderVM
                {
                    Id = s.Id,
                    SliderTitle = s.SliderTitle,
                    SliderName = s.SliderName,
                    SliderDescription = s.SliderDescription,
                    Image = s.Image
                }).ToListAsync();

            ViewData["CurrentFilter"] = searchString;
            return View(sliders);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SliderCreateVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string uniqueFileName = null;

            if (model.Image != null)
            {
                try
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/sliders");

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Image.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.Image.CopyToAsync(fileStream);
                    }
                }
                catch
                {
                    ModelState.AddModelError("", "File upload failed. Please try again.");
                    return View(model);
                }
            }

            var newSlider = new Slider
            {
                SliderTitle = model.SliderTitle,
                //SliderName = model.SliderName,
                SliderDescription = model.SliderDescription,
                //SliderNumber = model.SliderNumber,
                Image = uniqueFileName,
                SoftDeleted = false
            };

            _context.Sliders.Add(newSlider);
            await _context.SaveChangesAsync();

            TempData["messageType"] = "success";
            TempData["message"] = "Slider created successfully!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var slider = await _context.Sliders.FindAsync(id);
            if (slider == null)
            {
                return NotFound();
            }

            var viewModel = new SliderEditVM
            {
                Id = slider.Id,
                SliderTitle = slider.SliderTitle,
                //SliderName = slider.SliderName,
                //SliderNumber = slider.SliderNumber,
                SliderDescription = slider.SliderDescription,
                ExistingImage = slider.Image
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SliderEditVM model)
        {
            if (ModelState.IsValid)
            {
                var slider = await _context.Sliders.FindAsync(model.Id);
                if (slider == null)
                {
                    return NotFound();
                }

                slider.SliderTitle = model.SliderTitle;
                //slider.SliderName = model.SliderName;
                slider.SliderDescription = model.SliderDescription;
                //slider.SliderNumber = model.SliderNumber;

                if (model.Image != null)
                {
                    // Delete existing image
                    if (!string.IsNullOrEmpty(model.ExistingImage))
                    {
                        var imagePath = Path.Combine(_env.WebRootPath, "img/sliders", model.ExistingImage);
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }

                    // Save new image
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Image.FileName;
                    string uploadsFolder = Path.Combine(_env.WebRootPath, "img/sliders");
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.Image.CopyToAsync(fileStream);
                    }
                    slider.Image = uniqueFileName;
                }

                _context.Sliders.Update(slider);
                await _context.SaveChangesAsync();
                TempData["messageType"] = "success";
                TempData["message"] = "Slider Updated Successfully.";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var slider = await _context.Sliders
                .FirstOrDefaultAsync(m => m.Id == id);

            if (slider == null)
            {
                return NotFound();
            }

            return View(slider);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var slider = await _context.Sliders.FindAsync(id);
            if (slider != null)
            {
                //_context.Sliders.Remove(slider);
                slider.SoftDeleted = true;

                _context.Sliders.Update(slider);
                await _context.SaveChangesAsync();
            }

            TempData["messageType"] = "error";
            TempData["message"] = "Slider Deleted Successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
