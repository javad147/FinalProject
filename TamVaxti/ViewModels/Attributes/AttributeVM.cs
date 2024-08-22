using System.ComponentModel.DataAnnotations;

namespace TamVaxti.ViewModels.Attributes
{
    public class AttributeVM
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Type { get; set; }

    }
}
