using TamVaxti.Data;
using TamVaxti.Models;
using TamVaxti.Services.Interfaces;
using TamVaxti.ViewModels;
using TamVaxti.ViewModels.Categories;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace TamVaxti.Services
{

    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _context;

        public CategoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(CategoryCreateVM category)
        {
            await _context.Categories.AddAsync(new Category { Name = category.Name , 
                                                CategoryImage = category.CategoryImage ,
                                                IsPublished = category.IsPublished , 
                                                ShowInMenu = category.ShowInMenu,
                                                ShowOnCategoryHomePage = category.ShowOnCategoryHomePage,
                                                ShowOnTrendingHomePage = category.ShowOnTrendingHomePage  });
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Category category)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }

        public async Task EditAsync(Category category, CategoryEditVM categoryEdit)
        {
            category.Name = categoryEdit.Name;
            category.IsPublished = categoryEdit.IsPublished;
            category.ShowInMenu = categoryEdit.ShowInMenu;
            category.ShowOnCategoryHomePage = categoryEdit.ShowOnCategoryHomePage;
            category.ShowOnTrendingHomePage = categoryEdit.ShowOnTrendingHomePage;

            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistAsync(string name)
        {
            return await _context.Categories.AnyAsync(m => m.Name == name.Trim() && !m.SoftDeleted );
        }

        public async Task<List<Category>> GetCategoryForSubCategories()
        {
            var categories = await _context.Categories.Where(c => !c.SoftDeleted).ToListAsync();
            return categories;
        }
        public async Task<List<Category>> GetAllAsync()
        {
            var categories = await _context.Categories
     .Where(c => !c.SoftDeleted)
     .Include(c => c.Subcategories)
     .ToListAsync();

            var filteredCategories = categories
                .Select(c => new
                {
                    Category = c,
                    Subcategories = c.Subcategories
                        .Where(sc => sc.IsPublished && !sc.SoftDeleted)
                        .ToList()
                })
                .Where(c => c.Subcategories.Any())
                .Select(c =>
                {
                    c.Category.Subcategories = c.Subcategories;
                    return c.Category;
                })
                .ToList();

            return filteredCategories;
        }

        public async Task<List<CategoryFilterVM>> GetAllAsFilterAsync()
        {
            return await _context.Categories.Where(m => !m.SoftDeleted && _context.Products.Any(p => p.CategoryId == m.Id && !p.SoftDeleted)).Include(c => c.Subcategories).Select(x=> new CategoryFilterVM()
            {
                Id = x.Id,
                Name = x.Name,
                Subcategories = x.Subcategories,
                SoftDeleted = x.SoftDeleted,
                CategoryImage = x.CategoryImage,
                ShowInMenu = x.ShowInMenu,
                ShowOnCategoryHomePage = x.ShowOnCategoryHomePage,
                ShowOnTrendingHomePage = x.ShowOnTrendingHomePage,
                IsPublished = x.IsPublished,
                IsSelected = false
            }).ToListAsync();
        }

        public async Task<SelectList> GetAllBySelectedAsync()
        {
            var categories = await _context.Categories.ToListAsync();
            return new SelectList(categories, "Id", "Name");    
        }

        public async Task<List<CategoryVM>> GetAllOrderByDescendingAsync()
        {
            List<Category> categories = await _context.Categories
                                                     .Where(m => !m.SoftDeleted)
                                                     .OrderByDescending(m => m.Id)
                                                     .ToListAsync();

            return categories.Select(m => new CategoryVM {      Id = m.Id, Name = m.Name , 
                                                                CategoryImage = m.CategoryImage , 
                                                                IsPublished = m.IsPublished,
                                                                ShowInMenu = m.ShowInMenu,
                                                                ShowOnCategoryHomePage = m.ShowOnCategoryHomePage,
                                                                ShowOnTrendingHomePage = m.ShowOnTrendingHomePage
                                                                }).ToList();
        }

        public async Task<Category> GetByIdAsync(int id)
        {
            return await _context.Categories.Where(m => m.Id == id).FirstOrDefaultAsync();
        }


        public async Task<Category> GetWithProductsAsync(int id)
        {
            return await _context.Categories.Where(m => m.Id == id).FirstOrDefaultAsync();
        }

    }
}
