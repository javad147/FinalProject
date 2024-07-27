using System.ComponentModel.DataAnnotations;

namespace TamVaxti.Models
{
    public class AttributeOption 
    {
        [Key]
        public long Id { get; set; }
        public int AttributeId { get; set; }
        public string Value { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }

        public Attributes Attribute { get; set; }

        public string Color { get; set; }

        public string ImageUrl { get; set; }

        public ICollection<AttributeOptionSKU> AttributeOptionSKUs { get; set; }
    }
}
