using System;
namespace TamVaxti.ViewModels.Products
{

    public class ProductMainVM
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long SkuId { get; set; }
        public string Price { get; set; }
        public string Percentage { get; set; }
        public decimal SalePrice { get; set; }
        public int Ratings { get; set; }
        public int RatingCount { get; set; }
        public string StockStatus { get; set; }
        public string CurrencySymbol { get; set; }
        public int Quantity { get; set; }
        public List<string> PictureURL { get; set; }
        public List<ProductVariationVM> ProductVariation { get; set; }
    }
    public class ProductVariationVM
    {
        public string Attribute { get; set; }
        public string AttributeType { get; set; }
        public List<VariationAttributeOptionVM> Options { get; set; }
    }

    public class VariationAttributeOptionVM
    {
        public long AttributeOptionId { get; set; }
        public string AttributeOption { get; set; }
        public string ColorCode { get; set; }
        public bool IsSelected { get; set; }
        public bool Enabled { get; set; }
    }
}

