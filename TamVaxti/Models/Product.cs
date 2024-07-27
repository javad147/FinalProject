using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TamVaxti.Models
{
    public class Product : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } 
        public decimal Price { get; set; }
        
        public decimal DiscountPrice { get; set; }
        public int CategoryId { get; set; }
        public int StockStatusId { get; set; }
        public string MainImage { get; set; }
        public Category Category { get; set; }
        public ICollection<ProductImage> ProductImages { get; set; }
        public ICollection<OrderProductDetail> OrderProductDetails { get; set; }
        public List<SKU> SKUs { get; set; }

        [NotMapped]
        public IFormFile MainImageFile { get; set; }
    }
}
