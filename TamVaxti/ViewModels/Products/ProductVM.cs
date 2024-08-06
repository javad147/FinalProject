namespace TamVaxti.ViewModels.Products
{
    public class ProductVM
    {
        public int Id { get; set; }
        public long SkuId { get; set; }
        public string Image { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public int SubCategoryId { get; set; }
        public double Rating { get; set; }
        public int Quantity { get; set; }
        public decimal Amount { get; set; }


        // public decimal DiscountedPrice { get; set; }
    }
}
