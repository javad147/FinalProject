using System.ComponentModel.DataAnnotations;

namespace TamVaxti.Models
{
    public class SKU
    {
        [Key]
        public long Id { get; set; }
        public int ProductId { get; set; }
        public string SkuCode { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl1 { get; set; }
        public string ImageUrl2 { get; set; }
        public string ImageUrl3 { get; set; }
        public string ImageUrl4 { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; } = DateTime.Now;
        public Product Product { get; set; }
        public List<AttributeOptionSKU> AttributeOptionSKUs { get; set; }
        public List<SkuStock> SkuStock { get; set; }
        public int Quantity { get; set; }
        public int TotalQuantity => SkuStock?.Sum(ss => ss.Quantity) ?? 0;
        public IFormFile ImageUrl1File { get; set; }
        public IFormFile ImageUrl2File { get; set; }
        public IFormFile ImageUrl3File { get; set; }
        public IFormFile ImageUrl4File { get; set; }
        public List<OrderProductDetail> OrderProductDetail { get; set; }
    }
}
