using TamVaxti.ViewModels.Attributes;
using System.ComponentModel.DataAnnotations;

namespace TamVaxti.Models
{
    public class Attributes 
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public ICollection<AttributeOption> AttributeOptions { get; set; }
    }
}
