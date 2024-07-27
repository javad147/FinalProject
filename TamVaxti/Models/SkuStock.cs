using System.ComponentModel.DataAnnotations;

namespace TamVaxti.Models
{
    public class SkuStock
    {
        [Key]
        public int Id { get; set; }
        public long SkuId { get; set; }
        public int Quantity { get; set; }
        public SKU SKU { get; set; }
    }
}
