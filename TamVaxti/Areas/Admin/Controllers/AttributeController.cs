using TamVaxti.Data;
using TamVaxti.Models;
using TamVaxti.Services;
using TamVaxti.Services.Interfaces;
using TamVaxti.ViewModels.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TamVaxti.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AttributeController : Controller
    {

        private readonly AppDbContext _context;
        private readonly IAttributeService _attributeService;

        public AttributeController(AppDbContext context,IAttributeService attributeService)
        {
            _context = context;
            _attributeService = attributeService;
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Index(string searchString)
        {
            var attributes = await _attributeService.GetAllAsync(searchString);
            var attributeVMs = attributes.Select(a => new AttributeVM
            {
                Id = a.Id,
                Name = a.Name,
                Type = a.Type,
            }).ToList();

            ViewData["CurrentFilter"] = searchString;
            return View(attributeVMs);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAttributeVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var attribute = new Attributes
            {
                Name = model.Name,
                Type = model.Type,
                CreatedOn = DateTime.Now
            };

            // Save the attribute to the database
            await _attributeService.CreateAsync(attribute);

            TempData["messageType"] = "success";
            TempData["message"] = "Attribute Created Successfully.";
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var attribute = await _attributeService.GetByIdAsync(id);
            if (attribute == null)
            {
                return NotFound();
            }

            var model = new AttributeVM
            {
                Id = attribute.Id,
                Name = attribute.Name,
                Type = attribute.Type
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AttributeVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var attribute = await _attributeService.GetByIdAsync(model.Id);
            if (attribute == null)
            {
                return NotFound();
            }

            attribute.Name = model.Name;
            attribute.Type = model.Type;
            attribute.UpdatedOn = DateTime.Now;

            await _attributeService.UpdateAsync(attribute);

            TempData["messageType"] = "success";
            TempData["message"] = "Attribute Updated Successfully.";
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var attribute = await _attributeService.GetByIdAsync(id);
            if (attribute == null)
            {
                return NotFound();
            }

            await _attributeService.DeleteAsync(attribute);
            TempData["messageType"] = "error";
            TempData["message"] = "Attribute Deleted Successfully.";
            return RedirectToAction(nameof(Index));
        }

    }
}
