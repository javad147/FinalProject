using TamVaxti.ViewModels.SubCategory;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TamVaxti.Models
{
    public class Brand : BaseEntity
    {
        [Required]
        public string Name { get; set; }
        [NotMapped]
        public IFormFile BrandImage { get; set; }
        public string BrandImageURL { get; set; }
        public bool IsPublished { get; set; }
    }
}
