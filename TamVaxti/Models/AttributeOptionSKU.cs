using System.ComponentModel.DataAnnotations.Schema;

namespace TamVaxti.Models
{
    public class AttributeOptionSKU
    {
        public long SkuId { get; set; }
        public long AttributeOptionId { get; set; }

        [ForeignKey("SkuId")]
        public SKU SKU { get; set; }

        [ForeignKey("AttributeOptionId")]
        public AttributeOption AttributeOption { get; set; }

        [NotMapped]
        public int AttributeId { get; set; }
    }
}
