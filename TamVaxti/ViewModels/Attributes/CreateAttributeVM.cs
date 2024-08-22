

using System.ComponentModel.DataAnnotations;

namespace TamVaxti.ViewModels.Attributes
{
    public class CreateAttributeVM
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Type { get; set; }

    }
}
