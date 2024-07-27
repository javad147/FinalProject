using Microsoft.AspNetCore.Mvc.Rendering;

namespace TamVaxti.ViewModels.AttributeOption
{
    public class EditAttributeOptionVM
    {
        public long Id { get; set; }
        public string Value { get; set; }
        public int SelectedCategoryId { get; set; }
        public string Color { get; set; }
        public IFormFile ImageUrl { get; set; }
        public string ExistingImageUrl { get; set; } // To store the existing image URL
        public List<SelectListItem> Attributes { get; set; }
    }
}