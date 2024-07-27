using System.ComponentModel.DataAnnotations;

namespace TamVaxti.ViewModels.Categories
{
    public class CategoryCreateVM
    {
        [Required(ErrorMessage = "*Required")]
        [StringLength(20, ErrorMessage = "String length must less than 20")]
        public string Name { get; set; }

        [Display(Name = "Upload Image")]
        public string CategoryImage { get; set; } // Store image file name or path

        public IFormFile ImageFile { get; set; } // For file upload

        public bool IsPublished { get; set; }
    }
}
