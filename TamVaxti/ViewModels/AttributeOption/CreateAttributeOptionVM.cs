using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace TamVaxti.ViewModels.AttributeOption
{
    public class CreateAttributeOptionVM
    {
        public long Id { get; set; }

        public string Value { get; set; }
        public int SelectedCategoryId { get; set; }
        public IEnumerable<SelectListItem> Attributes { get; set; }

        public IFormFile ImageUrl { get; set; }
        public string SelectedCategoryType { get; set; }
        public string Color { get; set; }

    }
}
