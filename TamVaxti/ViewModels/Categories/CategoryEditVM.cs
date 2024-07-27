using System.ComponentModel.DataAnnotations;

namespace TamVaxti.ViewModels.Categories
{
    public class CategoryEditVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "*Required")]
        [StringLength(20, ErrorMessage = "String length must less than 20")]
        public string Name { get; set; }

        public string CategoryImage { get; set; }


        [Display(Name = "Upload Image")]
        public IFormFile ImageFile { get; set; }

        public bool IsPublished { get; set; }

        public bool RemoveImage { get; set; }


    }
}
