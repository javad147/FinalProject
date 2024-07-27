using TamVaxti.Data;
using TamVaxti.Models;
using TamVaxti.Services;
using TamVaxti.Services.Interfaces;
using TamVaxti.ViewModels.AttributeOption;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace TamVaxti.Areas.Admin.Controllers
{

    [Area("Admin")]
    public class AttributeOptionController : Controller
    {

        private readonly AppDbContext _context;
        private readonly IAttributeOptionService _attributeOptionService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AttributeOptionController(AppDbContext context, IAttributeOptionService attributeOptionService, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _attributeOptionService = attributeOptionService;
            _webHostEnvironment = webHostEnvironment;
        }



        [HttpGet]
        public async Task<IActionResult> Create()
        {

            var attributes = await _context.Attributes.ToListAsync();
            var viewModel = new CreateAttributeOptionVM
            {
                Attributes = attributes.Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = a.Name,
                    Group = new SelectListGroup { Name = a.Type } // Assuming Type is needed
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAttributeOptionVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var attributeOption = new AttributeOption
            {
                Value = model.Value,
                AttributeId = model.SelectedCategoryId,
                CreatedOn = DateTime.UtcNow,
            };

            // Handle file upload for image
            if (model.ImageUrl != null && model.ImageUrl.Length > 0)
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img", "attributes");
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImageUrl.FileName;

                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageUrl.CopyToAsync(fileStream);
                }

                attributeOption.ImageUrl = uniqueFileName;

            }
            else
            {
                attributeOption.ImageUrl = null;
            }

            // Set color if selected
            if (!string.IsNullOrEmpty(model.Color))
            {
                attributeOption.Color = model.Color;
            }

            await _attributeOptionService.CreateAsync(attributeOption);

            TempData["messageType"] = "success";
            TempData["message"] = "Attribute Option Created Successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var attributeOptions = await _context.AttributeOptions
                .Include(a => a.Attribute) // Assuming Attribute contains the category information
                .Select(a => new AttributeOptionVM
                {
                    Id = a.Id,
                    Value = a.Value,
                    CategoryName = a.Attribute != null ? a.Attribute.Type : null, // Use ternary operator instead of null-propagating operator
                }).ToListAsync();


            return View(attributeOptions);
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long id)
        {
            var attribute = await _attributeOptionService.GetByIdAsync((int)id);
            if (attribute == null)
            {
                return NotFound();
            }

            await _attributeOptionService.DeleteAsync(attribute);
            TempData["messageType"] = "error";
            TempData["message"] = "Attribute Option Deleted Successfully.";
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var attributeOption = await _context.AttributeOptions
                .Include(a => a.Attribute)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (attributeOption == null)
            {
                return NotFound();
            }

            var attributes = await _context.Attributes.ToListAsync();

            var viewModel = new EditAttributeOptionVM
            {
                Id = (int)attributeOption.Id,
                Value = attributeOption.Value,
                SelectedCategoryId = attributeOption.AttributeId,
                Color = attributeOption.Color,
                ExistingImageUrl = attributeOption.ImageUrl,
                Attributes = attributes.Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = a.Name,
                    Group = new SelectListGroup { Name = a.Type } // Assuming Type is needed
                }).ToList()
            };

            return View(viewModel);
        }

        




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditAttributeOptionVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var attributeOption = await _context.AttributeOptions.FindAsync(model.Id);
            if (attributeOption == null)
            {
                return NotFound();
            }

            attributeOption.Value = model.Value;
            attributeOption.AttributeId = model.SelectedCategoryId;
            attributeOption.UpdatedOn = DateTime.UtcNow;

            // Handle file upload for image
            if (model.ImageUrl != null && model.ImageUrl.Length > 0)
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img", "attributes");
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImageUrl.FileName;

                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageUrl.CopyToAsync(fileStream);
                }

                attributeOption.ImageUrl = uniqueFileName;
            }
            else if (!string.IsNullOrEmpty(model.ExistingImageUrl))
            {
                attributeOption.ImageUrl = model.ExistingImageUrl;
            }
            else
            {
                attributeOption.ImageUrl = null;
            }

            // Set color if selected
            if (!string.IsNullOrEmpty(model.Color))
            {
                attributeOption.Color = model.Color;
            }

            _context.Update(attributeOption);
            await _context.SaveChangesAsync();

            TempData["messageType"] = "success";
            TempData["message"] = "Attribute Option Updated Successfully.";
            return RedirectToAction(nameof(Index));
        }


    }
}
