using TamVaxti.Models;
using System.ComponentModel.DataAnnotations;

namespace TamVaxti.ViewModels.SubCategory
{
    public class SubCategoryCreateVM
    {
        [Required(ErrorMessage = "*Required")]
        [StringLength(20, ErrorMessage = "String length must be less than 20")]
        public string Name { get; set; }

        public string SubcategoryImage { get; set; } 

        public IFormFile ImageFile { get; set; } 

        [Required(ErrorMessage = "Please select a Category")]
        public int CategoryId { get; set; }

        public bool IsPublished { get; set; }

    }
}
