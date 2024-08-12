using TamVaxti.Helpers;
using TamVaxti.Models;

namespace TamVaxti.ViewModels.Products
{
    public class ProductListVM
    {
        public List<CategoryFilterVM> Categories { get; set; }
        public PaginatedList<ProductSkuListVM> Products { get; set; }
    }
}
