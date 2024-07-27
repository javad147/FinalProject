using TamVaxti.Models;

namespace TamVaxti.ViewModels.Baskets
{
    public class BasketVM
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }

        public decimal SubTotal { get; set; }
    }
}
