using System;
using System.ComponentModel.DataAnnotations;

namespace TamVaxti.Models
{
    public class ProductReview
    {
        public int ReviewId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public long SkuId { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        public string ReviewDescription { get; set; }

        public DateTime ReviewDate { get; set; }

        public bool Status { get; set; }
        public virtual AppUser User { get; set; }
        public virtual Product Product { get; set; }
        public virtual SKU SKU { get; set; }
    }
}
