namespace TamVaxti.Models
{
    public class OrderViewModel
    {
        public int OrderId { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; } // New property for full name
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string DeliveryStatus { get; set; }
        public List<Payment> Payments { get; set; }
    }

}
