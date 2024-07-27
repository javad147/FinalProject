using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace TamVaxti.Models
{
    public class Coupon
    {
        public int Id { get; set; }
        [Required]
        public string CouponTitle { get; set; }
        [Required]
        public string CouponCode { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        public bool AllowFreeShipping { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public string DiscountType { get; set; } // e.g., Percent or Fixed
        [Required]
        public decimal DiscountValue { get; set; }
        public bool IsEnabled { get; set; }
        public string Products { get; set; } // Consider using a list or relation if needed
        public string Category { get; set; }
        public decimal MinimumSpend { get; set; }
        public decimal MaximumSpend { get; set; }
        public int PerLimit { get; set; }
        public int PerCustomer { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedDate { get; set; }
    }



}

