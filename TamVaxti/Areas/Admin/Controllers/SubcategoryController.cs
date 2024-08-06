using TamVaxti.Data;
using TamVaxti.Models;
using TamVaxti.Services;
using TamVaxti.Services.Interfaces;
using TamVaxti.ViewModels.Categories;
using TamVaxti.ViewModels.SubCategory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using TamVaxti.Helpers.Enums;

namespace TamVaxti.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class SubcategoryController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ICategoryService _categoryService;
        private readonly ISubcategoryService _subcategoryService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SubcategoryController(AppDbContext context, 
                                        ISubcategoryService subcategoryService, 
                                        IWebHostEnvironment webHostEnvironment,
                                        ICategoryService categoryService)
        {
            _context = context;
            _subcategoryService = subcategoryService;
            _webHostEnvironment = webHostEnvironment;
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string searchString)
        {
            
            var subcategories = await _subcategoryService.GetAllAsync();
            var categories = await _categoryService.GetAllAsync(); 

            var subcategoryVMs = subcategories
                .Join(categories,
                      s => s.CategoryId,
                      c => c.Id,
                      (s, c) => new { Subcategory = s, CategoryName = c.Name })
                .Select(sc => new SubCategoryVM
                {
                    Id = sc.Subcategory.Id,
                    Name = sc.Subcategory.Name,
                    SubcategoryImage = sc.Subcategory.SubcategoryImage,
                    IsPublished = sc.Subcategory.IsPublished,
                    CategoryName = sc.CategoryName 
                })
                .OrderByDescending(sc => sc.Id)
                .ToList();

            if (!string.IsNullOrEmpty(searchString))
            {
                subcategoryVMs = subcategoryVMs
                    .Where(sc => sc.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            ViewData["CurrentFilter"] = searchString;
            return View(subcategoryVMs);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var categories = await _context.Categories.ToListAsync();
            ViewBag.Categories = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();

            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id is null) return BadRequest();

            SubCategory subcategory = await _subcategoryService.GetWithCategoryAsync((int)id);

            if (subcategory is null) return NotFound();

            SubcategoryDetailVM model = new()
            {
                Name = subcategory.Name,
                CategoryId = subcategory.CategoryId,
                SubcategoryImage = subcategory.SubcategoryImage,
                CategoryName = subcategory.Category.Name 
            };

            return View(model);
        }


    [HttpPost]
    [ValidateAntiForgeryToken]
public async Task<IActionResult> Create(SubCategoryCreateVM subcategory)
{
    if (!ModelState.IsValid)
    {
        return View(subcategory);
    }

    bool existSubcategory = await _subcategoryService.ExistAsync(subcategory.Name);

    if (existSubcategory)
    {
        ModelState.AddModelError("Name", "This subcategory already exists");
         ViewBag.Categories = await _context.Categories.Select(c => new { Value = c.Id, Text = c.Name }).ToListAsync();
         return View(subcategory);
    }

    string imagePath = null;
    if (subcategory.ImageFile != null)
    {
        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img" , "subcategories");
        var uniqueFileName = Guid.NewGuid().ToString() + "_" + subcategory.ImageFile.FileName;
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await subcategory.ImageFile.CopyToAsync(fileStream);
        }

        imagePath = uniqueFileName; 
    }

    var subCategoryCreateVM = new SubCategoryCreateVM
    {
        Name = subcategory.Name,
        SubcategoryImage = imagePath,
        CategoryId = subcategory.CategoryId,
        IsPublished = subcategory.IsPublished,
    };

    await _subcategoryService.CreateAsync(subCategoryCreateVM);
            TempData["messageType"] = "success";
            TempData["message"] = "SubCategory Created Successfully.";
            return RedirectToAction(nameof(Index));
}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return BadRequest();

            SubCategory subcategory = await _subcategoryService.GetByIdAsync((int)id);

            if (subcategory is null) return NotFound();

            Product pro = await _context.Products.Where(pro => pro.SubcategoryId == id).FirstOrDefaultAsync();

            if (pro != null)
            {
                TempData["messageType"] = "error";
                TempData["message"] = "Product  for this SubCategory exist please delete the Product of SubCategory from Product menu.";
                return RedirectToAction(nameof(Index));
            }


            if (!string.IsNullOrEmpty(subcategory.SubcategoryImage))
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img", "subcategories");
                var imagePath = Path.Combine(uploadsFolder, subcategory.SubcategoryImage);

                
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            await _subcategoryService.DeleteAsync(subcategory);
            TempData["messageType"] = "error";
            TempData["message"] = "SubCategory Deleted Successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null) return BadRequest();

            SubCategory subcategory = await _subcategoryService.GetByIdAsync((int)id);

            if (subcategory is null) return NotFound();

            IEnumerable<Category> categories = await _context.Categories.ToListAsync();

            List<SelectListItem> categorySelectList = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();

            return View(new SubCategoryEditVM
            {
                Id = subcategory.Id,
                Name = subcategory.Name,
                SubcategoryImage = subcategory.SubcategoryImage,
                CategoryId = subcategory.CategoryId,
                IsPublished = subcategory.IsPublished,
                Categories = categorySelectList 
            });

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, SubCategoryEditVM subcategory)
        {
            if (!ModelState.IsValid)
            {
                return View(subcategory);
            }
            if (id is null) return BadRequest();

            SubCategory existSubcategory = await _subcategoryService.GetByIdAsync((int)id);

            if (existSubcategory is null) return NotFound();

            if (subcategory.RemoveImage)
            {
                // Delete the old image
                if (!string.IsNullOrEmpty(existSubcategory.SubcategoryImage))
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img", "subcategories");
                    var oldFilePath = Path.Combine(uploadsFolder, existSubcategory.SubcategoryImage);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                existSubcategory.SubcategoryImage = null;
            }
            else if (subcategory.ImageFile != null)
            {
                // Handle image upload
                var uniqueFileName = $"{Guid.NewGuid()}_{subcategory.ImageFile.FileName}";
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img", "subcategories");
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await subcategory.ImageFile.CopyToAsync(fileStream);
                }

                // Delete the old image if necessary
                if (!string.IsNullOrEmpty(existSubcategory.SubcategoryImage))
                {
                    var oldFilePath = Path.Combine(uploadsFolder, existSubcategory.SubcategoryImage);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                existSubcategory.SubcategoryImage = uniqueFileName;
            }

            existSubcategory.Name = subcategory.Name;
            existSubcategory.CategoryId = subcategory.CategoryId;
            existSubcategory.IsPublished = subcategory.IsPublished;

            await _subcategoryService.EditAsync(existSubcategory);
            TempData["messageType"] = "success";
            TempData["message"] = "SubCategory Updated Successfully.";

            return RedirectToAction(nameof(Index));
        }
    }
}
