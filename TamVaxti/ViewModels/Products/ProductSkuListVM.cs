using TamVaxti.Models;

namespace TamVaxti.ViewModels.Products
{
    public class ProductSkuListVM
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string MainImage { get; set; }
        public long SkuId { get; set; }
        public string SkuCode { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int Rating { get; set; }
        public int RatingCount { get; set; }
        public int CategoryId { get; set; }
        public List<string> ImageUrls { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public List<RelatedSkuVM> RelatedSku { get; set; }
        public List<RelatedSizeVM> RelatedSizes { get; set; }
        public List<ProductReviewVM> ProductReview { get; set; }
    }
}
