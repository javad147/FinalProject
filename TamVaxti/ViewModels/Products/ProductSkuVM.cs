using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TamVaxti.Models;
using TamVaxti.ViewModels.AttributeOptionSKU;

namespace TamVaxti.ViewModels.Products
{

    public class ProductSkuVM
    {
        [Required]
        [StringLength(255)]
        public string SkuCode { get; set; }
        public int? ProductId { get; set; }

        [Required]
        [Range(0.01, 10000.00, ErrorMessage = "Please enter a valid price")]
        public decimal Price { get; set; }

        public decimal SalePrice { get; set; } = 0m;

        [Required]
        public int Quantity { get; set; }
        public List<AttributeOptionSKUVM> SKUAttributes { get; set; } = new List<AttributeOptionSKUVM>();

        public IFormFile ImageUrl1 { get; set; }
        public IFormFile ImageUrl2 { get; set; }
        public IFormFile ImageUrl3 { get; set; }
        public IFormFile ImageUrl4 { get; set; }

    }
}
