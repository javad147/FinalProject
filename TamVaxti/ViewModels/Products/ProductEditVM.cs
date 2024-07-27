using TamVaxti.Models;

namespace TamVaxti.ViewModels.Products
{
    public class ProductEditVM
    {

        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; } 
        public List<ProductImage> Images { get; set; }
        public List<IFormFile> NewImages { get; set; }
    }
}
