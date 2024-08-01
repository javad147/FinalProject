using TamVaxti.Models;
using TamVaxti.ViewModels;
using TamVaxti.ViewModels.Categories;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TamVaxti.Services.Interfaces
{
    public interface IBrandService
    {
        Task<List<Brand>> GetAllAsync();
        Task<List<Brand>> GetAllActiveAsync();
        Task<Brand> GetByIdAsync(int id);
        Task<Brand> GetByNameAsync(string name);
        Task CreateAsync(Brand brand);
        Task DeleteAsync(Brand brand);
        Task UpdateAsync(Brand brand);
    }
}
