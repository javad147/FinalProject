using TamVaxti.Models;

namespace TamVaxti.ViewModels.Products
{
    public class ProductListVM
    {
        public List<CategoryFilterVM> Categories { get; set; }
        public List<Product> Products { get; set; }
    }
}
