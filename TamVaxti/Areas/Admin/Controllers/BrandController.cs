using TamVaxti.Data;
using TamVaxti.Models;
using TamVaxti.Services.Interfaces;
using TamVaxti.ViewModels.Categories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TamVaxti.Helpers.Extensions;
using TamVaxti.ViewModels.Company;
using Bogus.DataSets;

namespace TamVaxti.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class BrandController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IBrandService _brandService;
        private readonly IWebHostEnvironment _webHostEnvironment;

       

        public BrandController(AppDbContext context,IBrandService brandService, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _brandService = brandService;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string searchString)
        {
            var brands = await _brandService.GetAllActiveAsync();

            if (!string.IsNullOrEmpty(searchString))
            {
                brands = brands.Where(b => b.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            ViewData["CurrentFilter"] = searchString;
            return View(brands);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var brand = await _brandService.GetByIdAsync(id);
            if (brand is null) return NotFound();
            return View(brand);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Brand brand)
        {
            if (!ModelState.IsValid)
            {
                return View(brand);
            }

            var existingBrand = await _brandService.GetByNameAsync(brand.Name);

            if (existingBrand !=null)
            {
                ModelState.AddModelError("Name", "This brand already exists");
                return View(brand);
            }
            if (brand.BrandImage != null)
            {
                    brand.BrandImageURL = await SaveFile(brand.BrandImage);
            }
            await _brandService.CreateAsync(brand);
            TempData["messageType"] = "success";
            TempData["message"] = "Category Created Successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {

            var brand = await _brandService.GetByIdAsync(id);

            if (brand is null) return NotFound();

            // remove the image.
            RemoveFile(brand.BrandImageURL);
            await _brandService.DeleteAsync(brand);
            TempData["messageType"] = "error";
            TempData["message"] = "Brand Deleted Successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var brand = await _brandService.GetByIdAsync(id);
            if (brand is null) return NotFound();
            return View(brand);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Brand request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }
            var existingBrand = await _brandService.GetByIdAsync(id);
            if (existingBrand is null)
            {
                return NotFound();
            }
            _context.Entry(existingBrand).State = EntityState.Detached;

            if (request.BrandImage != null)
            {
                if (request.BrandImageURL != null)
                {
                    RemoveFile(request.BrandImageURL);
                }

                request.BrandImageURL = await SaveFile(request.BrandImage);
            }

            await _brandService.UpdateAsync(request);
            TempData["messageType"] = "success";
            TempData["message"] = "Brand Updated Successfully.";

            return RedirectToAction(nameof(Index));
                        }

        private async Task<string> SaveFile(IFormFile file)
        {
            var fileName = "";
            try
            {
                // Write Upload code over here.
                if (file != null)
                {
                    fileName = $"{Guid.NewGuid()}_{file.FileName}";
                    var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "img","brand", fileName);
                    await FileExtensions.SaveFileToLocalAsync(file, filePath);
                }

            }
            catch { }
            return fileName;
        }

        private bool RemoveFile(string fileName)
        {
            try
            {
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "img","brand", fileName);
                FileExtensions.DeleteFileFromLocal(filePath);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }

}
