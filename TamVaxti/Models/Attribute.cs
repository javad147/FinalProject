using System.ComponentModel.DataAnnotations;

namespace TamVaxti.Models
{
    public class Attribute
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public ICollection<AttributeOption> AttributeOptions { get; set; }
        //public List<AttributeOption> Options { get; set; } = new List<AttributeOption>();

    }

}
