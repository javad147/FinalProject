namespace TamVaxti.ViewModels.Orders
{
    public class OrderEditVM
    {
        public int OrderId { get; set; }
        public string DeliveryStatus { get; set; }
        public DateTime? EstimatedDeliveryDate { get; set; }
        public DateTime? ActualDeliveryDate { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
    }
}
