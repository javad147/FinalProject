namespace TamVaxti.ViewModels.Products
{
    public class ProductReviewVM
    {
        public int ReviewId { get; set; }
        public string CustomerName { get; set; }
        public string ProductName { get; set; }
        public int Rating { get; set; }
        public string ReviewDescription { get; set; }
        public string PublishStatus { get; set; }
        public string ShortProductName
        {
            get
            {
                if (string.IsNullOrEmpty(ProductName))
                {
                    return string.Empty;
                }
                return ProductName.Length >= 20 ? ProductName.Substring(0, 20) + "..." : ProductName;
            }
        }
    }
}
