using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TamVaxti.Models
{
    public class OrderProductDetail
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public long SkuId { get; set; }

        [Required]
        public int Quantity { get; set; }

        public decimal Amount { get; set; }

        [ForeignKey("OrderId")]
        public Order Order { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        [ForeignKey("SkuId")]
        public SKU SKU { get; set; }
    }
}
