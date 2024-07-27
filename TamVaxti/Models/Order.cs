using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TamVaxti.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string OrderNumber { get; set; }

        [Required]
        public string CustomerId { get; set; }

        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}", ApplyFormatInEditMode = true)]
        public DateTime OrderDate { get; set; }

        [Required]
        public decimal Subtotal { get; set; }

        [Required]
        public decimal ShippingCharges { get; set; }

        [Required]
        public decimal Tax { get; set; }

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal TotalAmount => Subtotal + ShippingCharges + Tax;

        [Required]
        [StringLength(255)]
        public string ShippingAddress { get; set; }

        [Required]
        [StringLength(255)]
        public string DeliveryAddress { get; set; }

        [Required]
        public string DeliveryStatus { get; set; }

        public DateTime? EstimatedDeliveryDate { get; set; }
        public DateTime? ActualDeliveryDate { get; set; }
       
        public DateTime? ExpectedDeliveryDate { get; set; }

        [ForeignKey("CustomerId")]
        public AppUser Users { get; set; }
        public List<Payment> Payments { get; set; }
        public List<OrderProductDetail> OrderProductDetails { get; set; }
        public ICollection<OrderTracking> OrderTracking { get; set; }

        [StringLength(255)]
        public string CouponCode { get; set; }

        public decimal? CouponDiscountAmount { get; set; }
        public decimal? CouponDiscountPercentage { get; set; }
    }

}
