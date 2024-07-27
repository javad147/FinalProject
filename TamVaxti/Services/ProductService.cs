using TamVaxti.Data;
using TamVaxti.Helpers.Extensions;
using TamVaxti.Models;
using TamVaxti.Services.Interfaces;
using TamVaxti.ViewModels.Products;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace TamVaxti.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;


        public ProductService(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await _context.Products.Where(m => !m.SoftDeleted).Include(m => m.ProductImages).Include(m => m.Category).ToListAsync();
        }

        public async Task<List<Product>> GetAllPaginatedAsync(int page, int take = 10)
        {
            return await _context.Products.Where(m => !m.SoftDeleted)
                                          .Include(m => m.SKUs)
                                          //.Include(m => m.ProductImages)
                                          .Include(m => m.Category)
                                          .Skip((page - 1) * take)
                                          .Take(take).OrderByDescending(m => m.Id)
                                          .ToListAsync();

        }

        public async Task<List<Product>> GetAllWithImagesAsync()
        {
            return await _context.Products.Where(m => !m.SoftDeleted).Include(m => m.ProductImages).ToListAsync();
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            return await _context.Products.Where(m => !m.SoftDeleted)
                                                     .Include(m => m.ProductImages)
                                                     .Include(m => m.Category)
                                                     .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<int> GetCountAsync()
        {
            return await _context.Products.CountAsync();
        }

        public List<ProductVM> GetMappedDatas(List<Product> products)
        {
            return products.Select(m => new ProductVM
            {
                Id = m.Id,
                Name = m.Name,
                Image = m.MainImage,//m.ProductImages.FirstOrDefault(m => m.IsMain).ImageUrl,
                Description = m.Description,
                //Price = m.Price,
                Category = m.Category.Name,
                //DiscountedPrice = m.DiscountPrice
            }).ToList();
        }

        public async Task CreateAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Product product)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        public async Task<ProductImage> ProductImageByIdAsync(int id)
        {
            return await _context.ProductImages.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task DeleteProductImageAsync(ProductImage productImage)
        {
            _context.ProductImages.Remove(productImage);
            await _context.SaveChangesAsync();
        }

        public async Task ChangeMainImage(Product product, int id)
        {
            var images = product.ProductImages.Where(m => m.IsMain == true);

            foreach (var item in images)
            {
                item.IsMain = false;
            }

            product.ProductImages.FirstOrDefault(m => m.Id == id).IsMain = true;

            await _context.SaveChangesAsync();
        }


        public async Task EditProduct(Product product, ProductEditVM productEdit, List<ProductImage> images)
        {
            product.Name = productEdit.Name;
            product.Description = productEdit.Description;
            //product.Price = productEdit.Price;
            product.CategoryId = productEdit.CategoryId;
            //product.ProductImages = images;

            await _context.SaveChangesAsync();
        }

        public async Task<List<Attributes>> GetAttributes()
        {
            return await _context.Attributes.ToListAsync();
        }

        public async Task<List<AttributeOption>> GetAttributeOptions()
        {
            return await _context.AttributeOptions.ToListAsync();
        }
        public int GetSkuStockSum(long Id)
        {
            return _context.SkuStocks.Where(e => e.SkuId == Id).Sum(e => e.Quantity);
        }

        public async Task<SKU> GetSkuByIdAsync(int id)
        {
            return await _context.SKUs.Where(m => m.Id == id)
                                        .Include(m => m.AttributeOptionSKUs)
                                        .Include(m => m.SkuStock)
                                        .FirstOrDefaultAsync();
        }

        public async Task<AttributeOption> GetAttributeOptionsById(long id)
        {
            return await _context.AttributeOptions.Where(ao => ao.Id == id).FirstOrDefaultAsync();
        }
        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _context.Products.Where(m => !m.SoftDeleted)
                                    .Include(p => p.SKUs)
                                    .ThenInclude(s => s.AttributeOptionSKUs)
                                    .ThenInclude(s => s.AttributeOption)
                                    .Include(p => p.SKUs)
                                    .ThenInclude(s => s.SkuStock)
                                    .Include(x=>x.ProductImages)
                                    .FirstOrDefaultAsync(m => m.Id == id);
        }
        public async Task DeleteSkuAsync(SKU sku)
        {
            _context.Remove(sku);
            await _context.SaveChangesAsync();
        }
    }
}
