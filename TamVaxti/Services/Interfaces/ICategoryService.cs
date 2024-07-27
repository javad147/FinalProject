using TamVaxti.Models;
using TamVaxti.ViewModels;
using TamVaxti.ViewModels.Categories;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TamVaxti.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<List<Category>> GetAllAsync();
        Task<List<CategoryVM>> GetAllOrderByDescendingAsync();
        Task<bool> ExistAsync(string name);
        Task CreateAsync(CategoryCreateVM category);
        Task<Category> GetWithProductsAsync(int id);
        Task DeleteAsync(Category category);
        Task<Category> GetByIdAsync(int id);
        Task EditAsync(Category category,CategoryEditVM categoryEdit);
        Task<SelectList> GetAllBySelectedAsync();
        Task<List<CategoryFilterVM>> GetAllAsFilterAsync();
    }
}
