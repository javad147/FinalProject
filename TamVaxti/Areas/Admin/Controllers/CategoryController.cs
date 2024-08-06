using TamVaxti.Data;
using TamVaxti.Models;
using TamVaxti.Services.Interfaces;
using TamVaxti.ViewModels.Categories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TamVaxti.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ICategoryService _categoryService;
        private readonly IWebHostEnvironment _webHostEnvironment;



        public CategoryController(AppDbContext context,ICategoryService categoryService, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _categoryService = categoryService;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string searchString)
        {
            var categories = await _categoryService.GetAllOrderByDescendingAsync();

            if (!string.IsNullOrEmpty(searchString))
            {
                categories = categories.Where(c => c.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            
            var categoryVMs = categories.Select(c => new CategoryVM
            {
                Id = c.Id,
                Name = c.Name,
                CategoryImage = c.CategoryImage,
                IsPublished = c.IsPublished,
                ShowInMenu = c.ShowInMenu,
                ShowOnCategoryHomePage = c.ShowOnCategoryHomePage,
                ShowOnTrendingHomePage = c.ShowOnTrendingHomePage
            }).ToList();

            ViewData["CurrentFilter"] = searchString;
            return View(categoryVMs);
        }



        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id is null) return BadRequest();

            Category category = await _categoryService.GetWithProductsAsync((int)id);

            if (category is null) return NotFound();

            CategoryDetailVM model = new()
            {
                Name = category.Name,
                CategoryImage = category.CategoryImage,
            };

            return View(model);
        }



        [HttpGet]
        //[Authorize(Roles = "SuperAdmin")]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryCreateVM category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }

            bool existCategory = await _categoryService.ExistAsync(category.Name);

            if (existCategory)
            {
                ModelState.AddModelError("Name", "This category already exists");
                return View(category);
            }

            string imagePath = null;
            if (category.ImageFile != null)
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img" , "categories");
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + category.ImageFile.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await category.ImageFile.CopyToAsync(fileStream);
                }

                imagePath = uniqueFileName; 
            }

            var newCategory = new CategoryCreateVM
            {
                Name = category.Name,
                CategoryImage = imagePath,
                IsPublished = category.IsPublished,
                ShowInMenu = category.ShowInMenu,
                ShowOnCategoryHomePage = category.ShowOnCategoryHomePage,
                ShowOnTrendingHomePage = category.ShowOnTrendingHomePage,
            };

            await _categoryService.CreateAsync(newCategory);
            TempData["messageType"] = "success";
            TempData["message"] = "Category Created Successfully.";
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return BadRequest();

            // agaar id --> subcateg! also and product !

            SubCategory sub = await _context.SubCategories.Where(sub => sub.CategoryId == id).FirstOrDefaultAsync();
            if(sub!= null)
            {
                TempData["messageType"] = "error";
                TempData["message"] = "SubCategory for this Category exist please delete the SubCategory of Category from SubCategory menu.";
                return RedirectToAction(nameof(Index));
            }

            Product pro = await _context.Products.Where(pro => pro.CategoryId == id).FirstOrDefaultAsync();
            if (pro != null)
            {
                TempData["messageType"] = "error";
                TempData["message"] = "Product  for this Category exist please delete the Product of Category from Product menu.";
                return RedirectToAction(nameof(Index));
            }


            Category category = await _context.Categories.FindAsync(id);

            if (category is null) return NotFound();

            if (!string.IsNullOrEmpty(category.CategoryImage))
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img", "categories");
                var imagePath = Path.Combine(uploadsFolder, category.CategoryImage);

                // Check if the file exists and delete it
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }


            await _categoryService.DeleteAsync(category);
            TempData["messageType"] = "error";
            TempData["message"] = "Category Deleted Successfully.";
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null) return BadRequest();

            Category category = await _categoryService.GetByIdAsync((int)id);

            if (category is null) return NotFound();

            return View(new CategoryEditVM
            {
                Id = category.Id,
                Name = category.Name,
                CategoryImage = category.CategoryImage,
                IsPublished = category.IsPublished,
                ShowInMenu = category.ShowInMenu,
                ShowOnCategoryHomePage = category.ShowOnCategoryHomePage,
                ShowOnTrendingHomePage = category.ShowOnTrendingHomePage    

            });
        }



            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Edit(int? id, CategoryEditVM category)
            {
                if (!ModelState.IsValid)
                {
                    return View(category);
                }

                if (id is null) return BadRequest();

                Category existCategory = await _categoryService.GetByIdAsync((int)id);

                if (existCategory is null) return NotFound();

                if (category.RemoveImage)
                {
                    // Delete the old image
                    if (!string.IsNullOrEmpty(existCategory.CategoryImage))
                    {
                        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img", "categories");
                        var oldFilePath = Path.Combine(uploadsFolder, existCategory.CategoryImage);
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                          existCategory.CategoryImage = null;
                        }
                        else if (category.ImageFile != null)
                        {
                            // Handle image upload
                            var uniqueFileName = $"{Guid.NewGuid()}_{category.ImageFile.FileName}";
                            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img", "categories");
                            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await category.ImageFile.CopyToAsync(fileStream);
                            }

                            // Delete the old image if necessary
                            if (!string.IsNullOrEmpty(existCategory.CategoryImage))
                            {
                                var oldFilePath = Path.Combine(uploadsFolder, existCategory.CategoryImage);
                                if (System.IO.File.Exists(oldFilePath))
                                {
                                    System.IO.File.Delete(oldFilePath);
                                }
                            }

                                existCategory.CategoryImage = uniqueFileName;
                            }

                            existCategory.Name = category.Name;


                            await _categoryService.EditAsync(existCategory, category);
            TempData["messageType"] = "success";
            TempData["message"] = "Category Updated Successfully.";

            return RedirectToAction(nameof(Index));
                        }
        }
}
