using TamVaxti.Models;
using TamVaxti.ViewModels.Products;

namespace TamVaxti.ViewModels.Baskets
{
    public class BasketVM
    {
        public ProductSkuListVM Product { get; set; }
        public int Quantity { get; set; }

        public decimal SubTotal { get; set; }
    }
}
