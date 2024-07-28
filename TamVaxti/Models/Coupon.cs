using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace TamVaxti.Models
{
    public class Coupon
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string CouponTitle { get; set; }

        [Required]
        [StringLength(255)]
        public string CouponCode { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public bool AllowFreeShipping { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [StringLength(50)]
        public string DiscountType { get; set; }

        [Required]
        [Range(0, 9999999999999999.99)]
        public decimal DiscountValue { get; set; }

        [Required]
        public bool IsEnabled { get; set; }

        public string Products { get; set; }

        [StringLength(50)]
        public string Category { get; set; }

        [Range(0, 9999999999999999.99)]
        public decimal? MinimumSpend { get; set; }

        [Range(0, 9999999999999999.99)]
        public decimal? MaximumSpend { get; set; }

        public int? PerLimit { get; set; }

        public int? PerCustomer { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        [Required]
        public bool IsDeleted { get; set; }

        public DateTime? DeletedDate { get; set; }
    }

}

