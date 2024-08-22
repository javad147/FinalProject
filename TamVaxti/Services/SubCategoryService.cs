using TamVaxti.Data;
using TamVaxti.Models;
using TamVaxti.Services.Interfaces;
using TamVaxti.ViewModels.Categories;
using TamVaxti.ViewModels.SubCategory;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace TamVaxti.Services
{
    public class SubCategoryService : ISubcategoryService
    {
        private readonly AppDbContext _context;

        public SubCategoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(SubCategoryCreateVM subCategory)
        {
            var entity = new SubCategory
            {
                Name = subCategory.Name,
                CategoryId = subCategory.CategoryId,
                SubcategoryImage = subCategory.SubcategoryImage,
                IsPublished = subCategory.IsPublished
            };
            await _context.SubCategories.AddAsync(entity);
            await _context.SaveChangesAsync();
        }


        public async Task DeleteAsync(SubCategory subCategory)
        {
            _context.SubCategories.Remove(subCategory);
            await _context.SaveChangesAsync();
        }

        public async Task EditAsync(SubCategory subCategory)
        {
            _context.SubCategories.Update(subCategory);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> ExistAsync(string name)
        {
            return await _context.SubCategories.AnyAsync(m => m.Name == name.Trim());
        }

        public async Task<List<SubCategory>> GetAllAsync()
        {
            return await _context.SubCategories.Where(sc => !sc.SoftDeleted).ToListAsync();
        }

        public async Task<SelectList> GetAllBySelectedAsync()
        {
            var subCategories = await _context.SubCategories.ToListAsync();
            return new SelectList(subCategories, "Id", "Name");
        }

        public async Task<List<SubCategoryVM>> GetAllOrderByDescendingAsync()
        {
            var subCategories = await _context.SubCategories
                .OrderByDescending(m => m.Id)
                .ToListAsync();

            return subCategories.Select(m => new SubCategoryVM
            {
                Id = m.Id,
                Name = m.Name,
                SubcategoryImage = m.SubcategoryImage,
                IsPublished = m.IsPublished,    
            }).ToList();
        }

        public async Task<SubCategory> GetByIdAsync(int id)
        {
            return await _context.SubCategories.FindAsync(id);
        }

        public async Task<SubCategory> GetWithProductsAsync(int id)
        {
            return await _context.SubCategories.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<SubCategory> GetWithCategoryAsync(int id)
        {
            return await _context.SubCategories
                .Include(sc => sc.Category) 
                .FirstOrDefaultAsync(sc => sc.Id == id);
        }

    }
}
