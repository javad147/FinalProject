using TamVaxti.Models;
using TamVaxti.ViewModels.Baskets;

namespace TamVaxti.ViewModels
{
    public class CheckoutVM
    {
        public List<BasketVM> Products { get; set; }

        public UserShippingAddress UserShippingAddress { get; set; }

    }
}
