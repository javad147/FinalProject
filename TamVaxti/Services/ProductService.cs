﻿using TamVaxti.Data;
using TamVaxti.Helpers.Extensions;
using TamVaxti.Models;
using TamVaxti.Services.Interfaces;
using TamVaxti.ViewModels.Products;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TamVaxti.Helpers;
using System.Linq;

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
                SkuCodes = string.Join(", ", m.SKUs.Select(p => p.SkuCode))
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
            return await _context.Attributes.Where(a => !a.SoftDeleted).ToListAsync();
        }

        public async Task<List<AttributeOption>> GetAttributeOptions()
        {
            return await _context.AttributeOptions.Where(a => !a.SoftDeleted).ToListAsync();
        }
        public int GetSkuStockSum(long Id)
        {
            return _context.SkuStocks.Where(e => e.SkuId == Id).Sum(e => e.Quantity);
        }

        public async Task<SKU> GetSkuByIdAsync(long id)
        {
            return await _context.SKUs.Where(m => m.Id == id && !m.SoftDeleted)
                                        .Include(m => m.AttributeOptionSKUs)
                                        .Include(m => m.SkuStock)
                                        .FirstOrDefaultAsync();
        }

        public async Task<AttributeOption> GetAttributeOptionsById(long id)
        {
            return await _context.AttributeOptions.Where(ao => ao.Id == id && !ao.SoftDeleted).FirstOrDefaultAsync();
        }
        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _context.Products.Where(m => !m.SoftDeleted)
                                    .Include(p => p.SKUs.Where(s => !s.SoftDeleted))
                                    .ThenInclude(s => s.AttributeOptionSKUs)
                                    .ThenInclude(s => s.AttributeOption)
                                    .ThenInclude(ao => ao.Attribute)
                                    .Include(p => p.SKUs.Where(s => !s.SoftDeleted))
                                    .ThenInclude(s => s.SkuStock)
                                    .FirstOrDefaultAsync(m => m.Id == id);
        }
        public async Task DeleteSkuAsync(SKU sku)
        {
            _context.Remove(sku);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ProductReview>> GetProductReviewById(string Id, int ProductId, long SkuId)
        {
            var res = await _context.Product_Reviews.Where(r => r.UserId == Id && r.ProductId == ProductId && r.SkuId == SkuId).ToListAsync();
            return res;
        }

        public async Task<List<ProductReview>> GetPublishedProductReviewByOfProduct(int ProductId)
        {
            var res = await _context.Product_Reviews.Include(r => r.User).Where(r => r.ProductId == ProductId && r.Status == true).ToListAsync();
            return res;
        }

        public async Task<bool> AddProductReview(ProductReview model)
        {
            _context.Product_Reviews.Add(model);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<Product>> GetAllWithSkusAsync()
        {
            var products = await _context.Products
                .Where(p => !p.SoftDeleted)
                .Include(p => p.SKUs)
                    .ThenInclude(s => s.AttributeOptionSKUs)
                        .ThenInclude(aos => aos.AttributeOption)
                            .ThenInclude(ao => ao.Attribute)
                .Include(p => p.SKUs)
                    .ThenInclude(s => s.SkuStock)
                .Include(p => p.SKUs)
                    .ThenInclude(s => s.ProductReviews)
                .OrderByDescending(p => p.Id)
                .AsNoTracking()
                .ToListAsync();

            var filteredProducts = products
                .Select(p => new Product
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    MainImage = p.MainImage,
                    Price = p.Price,
                    CategoryId = p.CategoryId,
                    SubcategoryId = p.SubcategoryId,
                    BrandId = p.BrandId,
                    SoftDeleted = p.SoftDeleted,
                    SKUs = p.SKUs
                        .Where(s => !s.SoftDeleted)
                        .Select(s => new SKU
                        {
                            Id = s.Id,
                            SkuCode = s.SkuCode,
                            Price = s.Price,
                            SalePrice = s.SalePrice,
                            ImageUrl1 = s.ImageUrl1,
                            ImageUrl2 = s.ImageUrl2,
                            ImageUrl3 = s.ImageUrl3,
                            ImageUrl4 = s.ImageUrl4,
                            SoftDeleted = s.SoftDeleted,
                            AttributeOptionSKUs = s.AttributeOptionSKUs
                                .Where(aos => !aos.AttributeOption.SoftDeleted)
                                .Select(aos => new AttributeOptionSKU
                                {
                                    AttributeOptionId = aos.AttributeOptionId,
                                    SkuId = aos.SkuId,
                                    // Include other properties as needed
                                    AttributeOption = new AttributeOption
                                    {
                                        Id = aos.AttributeOption.Id,
                                        AttributeId = aos.AttributeOption.AttributeId,
                                        Value = aos.AttributeOption.Value,
                                        Color = aos.AttributeOption.Color,
                                        // Include other properties as needed
                                        Attribute = new Attributes
                                        {
                                            Id = aos.AttributeOption.Attribute.Id,
                                            Name = aos.AttributeOption.Attribute.Name
                                        }
                                    }
                                })
                                .ToList(),
                            SkuStock = s.SkuStock
                                .ToList(),
                            ProductReviews = s.ProductReviews
                                .Where(r => r.Status) // Assuming 'Status' is a boolean
                                .ToList()
                        })
                        .ToList()
                }).ToList();
            return filteredProducts;
            //return await _context.Products
            //    .Where(p => !p.SoftDeleted)
            //    .Include(p => p.SKUs.Where(s => !s.SoftDeleted))
            //    .ThenInclude(s => s.AttributeOptionSKUs)
            //    .ThenInclude(aos => aos.AttributeOption)
            //    .ThenInclude(ao => ao.Attribute)
            //    .Include(p => p.SKUs.Where(s => !s.SoftDeleted))
            //    .ThenInclude(s => s.SkuStock)
            //    .Include(p => p.SKUs.Where(s => !s.SoftDeleted))
            //    .ThenInclude(s => s.ProductReviews.Where(r => r.Status))
            //    .OrderByDescending(p => p.Id)
            //    .AsNoTracking() // Optional: Add this if you don't need tracking
            //    .ToListAsync();

            //return await _context.Products.Where(m => !m.SoftDeleted)
            //                        .Include(p => p.SKUs.Where(s => !s.SoftDeleted))
            //                        .ThenInclude(s => s.AttributeOptionSKUs)
            //                        .ThenInclude(s => s.AttributeOption)
            //                        .ThenInclude(ao => ao.Attribute)
            //                        .Include(p => p.SKUs.Where(s => !s.SoftDeleted))
            //                        .ThenInclude(s => s.SkuStock)
            //                        .Include(p => p.SKUs.Where(s => !s.SoftDeleted))
            //                        .ThenInclude(s => s.ProductReviews.Where(r => r.Status))
            //                        .OrderByDescending(o => o.Id)
            //                        .ToListAsync();


        }

        public async Task<int> GetProductIdBySkuIdAsync(long SkuId)
        {
            var res = await _context.SKUs.Where(s => s.Id == SkuId).Select(s => s.ProductId).FirstOrDefaultAsync();
            return res;
        }

        public async Task<Product> GetProductBySkuIdAsync(long skuId)
        {
            var sku = await _context.SKUs
                                .Where(s => s.Id == skuId && !s.SoftDeleted)
                                .Include(s => s.Product)
                                    .ThenInclude(p => p.SKUs.Where(s => !s.SoftDeleted))
                                    .ThenInclude(s => s.SkuStock)
                                .Include(s => s.Product)
                                    .ThenInclude(p => p.SKUs.Where(s => !s.SoftDeleted))
                                    .ThenInclude(s => s.AttributeOptionSKUs)
                                    .ThenInclude(aos => aos.AttributeOption)
                                    .ThenInclude(ao => ao.Attribute)
                                .Include(s => s.Product)
                                    .ThenInclude(p => p.SKUs.Where(s => !s.SoftDeleted))
                                    .ThenInclude(s => s.ProductReviews.Where(r => r.Status))
                                .FirstOrDefaultAsync();

            return sku?.Product;
        }

        public List<ProductSkuListVM> GetProductSkuListVM(List<Product> products)
        {
            var result = products.SelectMany(product =>
                product.SKUs.Select(sku => new ProductSkuListVM
                {
                    ProductId = product.Id,
                    Name = product.Name + " (" + sku.SkuCode + ")",
                    Description = product.Description,
                    MainImage = !string.IsNullOrEmpty(sku.ImageUrl1) ? sku.ImageUrl1 : !string.IsNullOrEmpty(sku.ImageUrl2) ? sku.ImageUrl2
                                : !string.IsNullOrEmpty(sku.ImageUrl3) ? sku.ImageUrl3 : !string.IsNullOrEmpty(sku.ImageUrl4) ? sku.ImageUrl4 : product.MainImage,
                    CategoryId = product.CategoryId,
                    SubcategoryId = product.SubcategoryId,
                    SkuId = sku.Id,
                    SkuCode = sku.SkuCode,
                    Price = sku.Price,
                    SalePrice = sku.SalePrice,
                    Quantity = sku.SkuStock.Sum(s => s.Quantity),
                    Rating = (int)(sku.ProductReviews.Count() > 0 ? Math.Round(sku.ProductReviews.Average(s => s.Rating)) : 0),
                    RatingCount = sku.ProductReviews.Count(),
                    Color = sku.AttributeOptionSKUs.FirstOrDefault(aos => aos.AttributeOption.Attribute.Name == "Color")?
                                    .AttributeOption.Color,
                    Size = sku.AttributeOptionSKUs.FirstOrDefault(aos => aos.AttributeOption.Attribute.Name == "Size")?
                                    .AttributeOption.Value,
                    RelatedSku = product.SKUs.Select(s => new RelatedSkuVM
                    {
                        SkuId = s.Id,
                        Color = s.AttributeOptionSKUs.FirstOrDefault(aos => aos.AttributeOption.Attribute.Name == "Color")?
                                    .AttributeOption.Color
                    }).DistinctBy(r => r.Color).OrderBy(s => sku.Id).ToList()
                })).ToList();

            return result;
        }

        public PaginatedList<ProductSkuListVM> GetProductSkuListVMPaginated(List<ProductSkuListVM> products, int pageIndex, int pageSize, string sortOrder)
        {
            IQueryable<ProductSkuListVM> result = products.AsQueryable();

            switch (sortOrder)
            {
                case "Date":
                    result = result.OrderBy(p => p.SkuId);
                    break;
                case "Name":
                    result = result.OrderBy(p => p.Name);
                    break;
                case "name_desc":
                    result = result.OrderByDescending(p => p.Name);
                    break;
                case "Price":
                    result = result.OrderBy(p => p.Price);
                    break;
                case "price_desc":
                    result = result.OrderByDescending(p => p.Price);
                    break;
                default:
                    result = result.OrderByDescending(p => p.SkuId);
                    break;
            }

            int totalCount = result.Count();

            result = result.Skip((pageIndex - 1) * pageSize).Take(pageSize);

            var paginatedResult = result.ToList();

            return new PaginatedList<ProductSkuListVM>(paginatedResult, totalCount, pageIndex, pageSize);

        }

        public async Task SoftDeleteSku(long skuId)
        {
            var sku = await _context.SKUs.FindAsync(skuId);
            if (sku != null)
            {
                sku.SoftDeleted = true;
                sku.UpdatedOn = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
    }
}
