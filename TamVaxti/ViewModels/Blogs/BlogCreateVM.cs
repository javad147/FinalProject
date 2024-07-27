using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TamVaxti.ViewModels.Blogs
{
    public class BlogCreateVM
    {
        [Required(ErrorMessage = "Title field can`t be empty")]
        [StringLength(200, ErrorMessage = "String length must be less than 20")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description field can`t be empty")]
        
        public string Description { get; set; }

        [Required(ErrorMessage = "Image is required")]
        [NotMapped]
        public IFormFile ImageFile { get; set; }

        public string Image { get; set; }
        public string Uname { get; set; }
        public DateTime Date { get; set; }
    }
}
