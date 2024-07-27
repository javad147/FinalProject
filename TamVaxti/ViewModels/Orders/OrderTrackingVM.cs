namespace TamVaxti.ViewModels.Orders
{
    public class OrderTrackingVM
    {
        public int OrderId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string Location { get; set; }
        public string Notes { get; set; }
    }
}
