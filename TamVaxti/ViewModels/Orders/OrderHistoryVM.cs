using TamVaxti.ViewModels.Products;

namespace TamVaxti.ViewModels.Orders
{
    public class OrderHistoryVM
    {
        public string OrderNumber { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime? ActualDeliveryDate { get; set; }
        public List<ProductVM> Products { get; set; }
    }
}
