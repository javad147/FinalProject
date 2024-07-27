using TamVaxti.Models;
using TamVaxti.ViewModels.SubCategory;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TamVaxti.Services.Interfaces
{

        public interface ISubcategoryService
        {
        //Task<List<SubcategoryVM>> GetAllOrderByDescendingAsync();
        //Task<Subcategory> GetByIdAsync(int id);
        //Task<Subcategory> GetWithCategoryAsync(int id);
        //Task<bool> ExistAsync(string name);
        //Task CreateAsync(Subcategory subcategory);
        //Task EditAsync(Subcategory existSubcategory, SubcategoryEditVM subcategory);
        //Task DeleteAsync(Subcategory subcategory);


        Task<List<SubCategory>> GetAllAsync();
        Task<List<SubCategoryVM>> GetAllOrderByDescendingAsync();
        Task<bool> ExistAsync(string name);
        Task CreateAsync(SubCategoryCreateVM subCategory);
        Task<SubCategory> GetWithProductsAsync(int id);
        Task DeleteAsync(SubCategory subCategory);
        Task<SubCategory> GetByIdAsync(int id);
        Task EditAsync(SubCategory subCategory);
        Task<SelectList> GetAllBySelectedAsync();

        Task<SubCategory> GetWithCategoryAsync(int id);



    }
}
