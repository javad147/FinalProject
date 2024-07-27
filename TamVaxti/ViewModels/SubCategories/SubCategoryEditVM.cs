using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace TamVaxti.ViewModels.SubCategory
{
    public class SubCategoryEditVM
    {

            public int Id { get; set; }

            [Required(ErrorMessage = "*Required")]
            [StringLength(20, ErrorMessage = "String length must be less than 20")]
            public string Name { get; set; }

            public string SubcategoryImage { get; set; }


            [Display(Name = "Upload Image")]
            public IFormFile ImageFile { get; set; }

            [Required(ErrorMessage = "Please select a Category")]
            public int CategoryId { get; set; } // To link the subcategory to a category

            public IEnumerable<SelectListItem> Categories { get; set; } // List of categories for dropdown

             public bool IsPublished { get; set; }

             public bool RemoveImage { get; set; }

    }
}
