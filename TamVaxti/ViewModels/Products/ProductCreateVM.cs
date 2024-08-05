using TamVaxti.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace TamVaxti.ViewModels.Products
{
    public class ProductCreateVM
    {
        public int? ProductId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required(ErrorMessage = "Price can't be empty")]
        //public string Price { get; set; }
        public int CategoryId { get; set; }
        public int SubcategoryId { get; set; }

        //public List<IFormFile> Images { get; set; }
        [Required(ErrorMessage = "Main image is required")]
        public IFormFile MainImage { get; set; }
        public List<ProductSkuVM> SKUs { get; set; } = new List<ProductSkuVM> { new ProductSkuVM() };

    }
}
