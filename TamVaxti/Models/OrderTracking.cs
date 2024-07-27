using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TamVaxti.Models
{
    public class OrderTracking
    {
        [Key]
        public int TrackingId { get; set; }
        public int OrderId { get; set; }
        public DateTime Timestamp { get; set; }
        public string Location { get; set; }
        public string? Status { get; set; } = "Pending";
        public string Notes { get; set; }
        public bool? LocationCompleted { get; set; } = true;

        [ForeignKey("OrderId")]
        public Order Order { get; set; }
    }

}
