using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TamVaxti.Models
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }

        [Required]
        public int OrderId { get; set; }

        public DateTime PaymentDate { get; set; }

        [Required]
        public string PaymentStatus { get; set; }

        [Required]
        public decimal PaymentAmount { get; set; }

        [Required]
        public string PaymentMode { get; set; }

        [ForeignKey("OrderId")]
        public Order Order { get; set; }
    }
}
