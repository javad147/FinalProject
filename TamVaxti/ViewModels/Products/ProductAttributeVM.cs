using TamVaxti.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TamVaxti.ViewModels.Products
{
    public class ProductAttributeVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<SelectListItem> AttributeOptions { get; set; }
    }
}
