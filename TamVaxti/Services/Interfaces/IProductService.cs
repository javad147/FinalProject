using TamVaxti.Models;
using TamVaxti.ViewModels.Products;

namespace TamVaxti.Services.Interfaces
{
    public interface IProductService
    {
        Task<List<Product>> GetAllWithImagesAsync();
        Task<List<Product>> GetAllAsync();
        Task<Product> GetByIdAsync(int id);
        List<ProductVM> GetMappedDatas(List<Product> products);
        Task<List<Product>> GetAllPaginatedAsync(int page,int take = 10);
        Task<int> GetCountAsync();
        Task CreateAsync(Product product);
        Task DeleteAsync(Product product);
        Task<ProductImage> ProductImageByIdAsync(int id);
        Task DeleteProductImageAsync(ProductImage productImage);
        Task ChangeMainImage(Product product,int id);
        Task EditProduct(Product product, ProductEditVM productEdit, List<ProductImage> images);
        public Task<List<Attributes>> GetAttributes();
        public Task<List<AttributeOption>> GetAttributeOptions();
        public Task<AttributeOption> GetAttributeOptionsById(long id);
        public int GetSkuStockSum(long Id);
        public Task<Product> GetProductByIdAsync(int id);
        public Task<SKU> GetSkuByIdAsync(int id);
        public Task DeleteSkuAsync(SKU sku);
    }
}
