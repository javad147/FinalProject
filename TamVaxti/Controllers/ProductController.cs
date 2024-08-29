using TamVaxti.Data;
using TamVaxti.Models;
using TamVaxti.Services.Interfaces;
using TamVaxti.ViewModels.Products;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace TamVaxti.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        readonly UserManager<AppUser> _userManager;
        private readonly IUserWishList _userWishList;
        private readonly ICompanyService _companyService;
        private readonly AppDbContext _context;
        public ProductController(IProductService productService, UserManager<AppUser> userManager, IUserWishList userWishList, ICompanyService companyService, AppDbContext context)
        {
            _productService = productService;
            _userManager = userManager;
            _userWishList = userWishList;
            _companyService = companyService;
            _context = context;
        }
        private async Task<List<ProductVariationVM>> GetProductVariationAsync(long id)
        {
            var productVariations = from p in _context.Products
                                    join s in _context.SKUs on p.Id equals s.ProductId
                                    join asmap in _context.AttributeOptionSKUs on s.Id equals asmap.SkuId
                                    join aop in _context.AttributeOptions on asmap.AttributeOptionId equals aop.Id
                                    join attr in _context.Attributes on aop.AttributeId equals attr.Id
                                    where p.Id == id
                                    select new
                                    {
                                        SkuId = s.Id,
                                        AttributeId = attr.Id,
                                        Attribute = attr.Name, // Adjust according to your attribute naming
                                        AttributeType = attr.Type, // If you have a type field in your attributes
                                        AttributeOptionId = aop.Id,
                                        AttributeOption = aop.Value,
                                        ColorCode = aop.Color // Ensure this field exists in your AttributeOptions model
                                    };

            var groupedData = productVariations
            .GroupBy(p => new { p.AttributeId, p.Attribute, p.AttributeType })
            .Select(g => new ProductVariationVM
            {
                Attribute = g.Key.Attribute,
                AttributeType = g.Key.AttributeType,
                Options = g.Select(p => new VariationAttributeOptionVM
                {
                    AttributeOptionId = p.AttributeOptionId,
                    AttributeOption = p.AttributeOption,
                    ColorCode = p.ColorCode
                }).Distinct().ToList()
            });

            return await groupedData.ToListAsync(); // Use await with ToListAsync() for async execution
        }

        public List<string> GetPictureURLs(dynamic sku, string mainImage)
        {
            // List of image URLs
            var imageUrls = new List<string>
        {
            sku.ImageUrl1,
            sku.ImageUrl2,
            sku.ImageUrl3,
            sku.ImageUrl4
        };

            // Find the first non-empty image URL
            var selectedImage = imageUrls.Where(url => !string.IsNullOrWhiteSpace(url)).ToList();

            // If no image URLs have values, use the main image
            if (selectedImage.Count() <= 0)
            {
                selectedImage.Add(mainImage);
            }
            return selectedImage;
        }
        [HttpGet]
        public async Task<IActionResult> GetProductByVariation(int productId, string attributeOptionIds)
        {
            // Parse the comma-separated attributeOptionIds into a list of long IDs
            var attributeOptionIdList = attributeOptionIds
                .Split(',')
                .Select(id => long.Parse(id.Trim()))
                .Where(id => id != 0)
                .ToList();


            // Fetching product variations asynchronously
            var productVariations = (from p in _context.Products
                                     join s in _context.SKUs on p.Id equals s.ProductId
                                     join asmap in _context.AttributeOptionSKUs on s.Id equals asmap.SkuId
                                     join aop in _context.AttributeOptions on asmap.AttributeOptionId equals aop.Id
                                     join attr in _context.Attributes on aop.AttributeId equals attr.Id
                                     where p.Id == productId
                                     select new
                                     {
                                         SkuId = s.Id,
                                         AttributeId = attr.Id,
                                         Attribute = attr.Name, // Adjust according to your attribute naming
                                         AttributeType = attr.Type, // If you have a type field in your attributes
                                         AttributeOptionId = aop.Id,
                                         AttributeOption = aop.Value,
                                         ColorCode = aop.Color // Ensure this field exists in your AttributeOptions model
                                     }).ToList();


            var matchingSkus = productVariations
           .GroupBy(x => x.SkuId) // Group by SkuId
           .Where(g => attributeOptionIdList.All(attr => g.Select(i => i.AttributeOptionId).Contains(attr))) // Filter groups
           .Select(g => g.Key) // Get SkuIds
           .ToList();

            var distinctAttibutOptionIdBySku = productVariations.Where(w => matchingSkus.Contains(w.SkuId)).Select(g => g.AttributeOptionId).Distinct().ToList();

            var groupedData = productVariations
            .GroupBy(p => new { p.AttributeId, p.Attribute, p.AttributeType })
            .Select(g => new ProductVariationVM
            {
                Attribute = g.Key.Attribute,
                AttributeType = g.Key.AttributeType,
                Options = g.Select(p => new VariationAttributeOptionVM
                {
                    AttributeOptionId = p.AttributeOptionId,
                    AttributeOption = p.AttributeOption,
                    ColorCode = p.ColorCode,
                    Enabled = distinctAttibutOptionIdBySku.Contains(p.AttributeOptionId),
                    IsSelected = attributeOptionIdList.Contains(p.AttributeOptionId) // Flag if the option ID is in the selected list
                }).ToList()
            }).ToList();


            var distinctGroupedData = groupedData.Select(g => new ProductVariationVM
            {
                Attribute = g.Attribute,
                AttributeType = g.AttributeType,
                Options = g.Options
                .Distinct(new VariationAttributeOptionVMComparer()) // Apply distinct operation with custom comparer
                .ToList()
            }).ToList();

            // Remove element if option doesnot exists.

            // Check if we get only one distinct sku if yes then set.
            var productMain = new ProductMainVM();
            List<ProductReview> pr = await _productService.GetPublishedProductReviewByOfProduct(productId);

            if (matchingSkus.Count() == 1)
            {
                // Get the details and set to productmainvm
                var sku = await _context.SKUs.Where(s => s.Id == matchingSkus.First()).Include(ss => ss.SkuStock).Include(s => s.Product).FirstOrDefaultAsync();
                string skuCodeDisplayText = string.IsNullOrEmpty(sku.SkuCode) ? "" : $"({sku.SkuCode})";
                productMain.Price = sku.Price.ToString();
                productMain.Quantity = sku.SkuStock.Sum(s => s.Quantity);
                productMain.StockStatus = (productMain.Quantity > 0) ? "In Stock" : "Out Of Stock";
                productMain.SalePrice = 0;
                productMain.Id = sku.ProductId;
                productMain.SkuId = sku.Id;
                productMain.Ratings = (int)(pr.Count() > 0 ? Math.Round(pr.Average(s => s.Rating)) : 0);
                productMain.RatingCount = pr.Count();
                productMain.Name = $"{sku.Product.Name}{skuCodeDisplayText}";
                productMain.CurrencySymbol = _companyService.GetCurrencySymbol();
                productMain.PictureURL = new List<string> { sku.Product.MainImage, "" };
                productMain.PictureURL = GetPictureURLs(sku, sku.Product.MainImage);
            }
            else
            {
                var sku = await _context.SKUs.Where(s => matchingSkus.Contains(s.Id)).Include(ss => ss.SkuStock).Include(s => s.Product).ToListAsync();
                var highPrice = sku.Max(sku => sku.Price);
                var lowPrice = sku.Min(sku => sku.Price);
                productMain.Name = sku[0].Product.Name;
                productMain.Price = (highPrice == lowPrice) ? highPrice.ToString() : $"{lowPrice} - {highPrice}";
                productMain.Quantity = sku.Min(sku => sku.Quantity);
                productMain.StockStatus = (productMain.Quantity > 0) ? "In Stock" : "Out Of Stock";
                productMain.SalePrice = 0;
                productMain.Id = sku[0].ProductId;
                productMain.Ratings = (int)(pr.Count() > 0 ? Math.Round(pr.Average(s => s.Rating)) : 0);
                productMain.RatingCount = pr.Count();
                productMain.CurrencySymbol = _companyService.GetCurrencySymbol();
                productMain.PictureURL = new List<string> { sku[0].Product.MainImage };
            }
            productMain.ProductVariation = distinctGroupedData;
            //return Ok(productMain);
            return PartialView("_ProductVariant", productMain);
        }


        public async Task<IActionResult> Index(long? id)
        {
            if (id is null)
            {
                return BadRequest();
            }
            Product product = await _productService.GetProductBySkuIdAsync((long)id);
            List<ProductReview> pr = await _productService.GetPublishedProductReviewByOfProduct(product.Id);
            List<ProductVariationVM> pv = await GetProductVariationAsync(product.Id);

            if (product is null)
            {
                return NotFound();
            }
            var model = product.SKUs.Select(sku => new ProductSkuListVM
            {
                ProductId = product.Id,
                Name = product.Name + " (" + sku.SkuCode + ")",
                Description = product.Description,
                MainImage = !string.IsNullOrEmpty(sku.ImageUrl1) ? sku.ImageUrl1 : !string.IsNullOrEmpty(sku.ImageUrl2) ? sku.ImageUrl2
                            : !string.IsNullOrEmpty(sku.ImageUrl3) ? sku.ImageUrl3 : !string.IsNullOrEmpty(sku.ImageUrl4) ? sku.ImageUrl4 : product.MainImage,
                SkuId = sku.Id,
                SkuCode = sku.SkuCode,
                Price = sku.Price,
                Quantity = sku.SkuStock.Sum(s => s.Quantity),
                Rating = (int)(sku.ProductReviews.Count() > 0 ? Math.Round(sku.ProductReviews.Average(s => s.Rating)) : 0),
                RatingCount = sku.ProductReviews.Count(),
                ImageUrls = new List<string> { sku.ImageUrl1, sku.ImageUrl2, sku.ImageUrl3, sku.ImageUrl4 }
                        .Where(url => !string.IsNullOrEmpty(url))
                        .ToList(),
                ProductReview = pr.Select(s => new ProductReviewVM
                {
                    ReviewDescription = s.ReviewDescription,
                    Rating = s.Rating,
                    CustomerName = s.User.UserName,
                    ReviewDate = s.ReviewDate
                }).ToList(),
                ProductVariation = pv
            }).FirstOrDefault();

            if (!model.ImageUrls.Any())
            {
                model.ImageUrls.Add(product.MainImage);
            }

            ViewBag.CurrencySymbol = _companyService.GetCurrencySymbol();
            return View(model);
        }

        public async Task<IActionResult> IndexOld(long? id)
        {
            if (id is null)
            {
                return BadRequest();
            }
            //Product product = await _productService.GetProductByIdAsync((int)id);
            Product product = await _productService.GetProductBySkuIdAsync((long)id);
            if (product is null)
            {
                return NotFound();
            }
            List<ProductReview> pr = await _productService.GetPublishedProductReviewByOfProduct(product.Id);

            var model = product.SKUs.Select(sku => new ProductSkuListVM
            {
                ProductId = product.Id,
                Name = product.Name + " (" + sku.SkuCode + ")",
                Description = product.Description,
                MainImage = !string.IsNullOrEmpty(sku.ImageUrl1) ? sku.ImageUrl1 : !string.IsNullOrEmpty(sku.ImageUrl2) ? sku.ImageUrl2
                            : !string.IsNullOrEmpty(sku.ImageUrl3) ? sku.ImageUrl3 : !string.IsNullOrEmpty(sku.ImageUrl4) ? sku.ImageUrl4 : product.MainImage,
                SkuId = sku.Id,
                SkuCode = sku.SkuCode,
                Price = sku.Price,
                SalePrice = sku.SalePrice,
                Quantity = sku.SkuStock.Sum(s => s.Quantity),
                Rating = (int)(sku.ProductReviews.Count() > 0 ? Math.Round(sku.ProductReviews.Average(s => s.Rating)) : 0),
                RatingCount = sku.ProductReviews.Count(),
                ImageUrls = new List<string> { sku.ImageUrl1, sku.ImageUrl2, sku.ImageUrl3, sku.ImageUrl4 }
                        .Where(url => !string.IsNullOrEmpty(url))
                        .ToList(),
                Color = sku.AttributeOptionSKUs.FirstOrDefault(aos => aos.AttributeOption.Attribute.Name == "Color")?
                                                .AttributeOption.Color,
                Size = sku.AttributeOptionSKUs.FirstOrDefault(aos => aos.AttributeOption.Attribute.Name == "Size")?
                                                .AttributeOption.Value,
                RelatedSizes = product.SKUs.Where(s => //s.Id != sku.Id &&
                                            s.AttributeOptionSKUs.Any(aos =>
                                                aos.AttributeOption.Attribute.Name == "Color" &&
                                                aos.AttributeOption.Value == sku.AttributeOptionSKUs.FirstOrDefault(ao => ao.AttributeOption.Attribute.Name == "Color")?
                                                    .AttributeOption.Value) &&
                                            s.AttributeOptionSKUs.Any(aos => aos.AttributeOption.Attribute.Name == "Size"))
                                .Select(s => new RelatedSizeVM
                                {
                                    SkuId = s.Id,
                                    Size = s.AttributeOptionSKUs.FirstOrDefault(aos => aos.AttributeOption.Attribute.Name == "Size")?
                                        .AttributeOption.Value
                                }).ToList(),
                DialSize = product.SKUs.Where(s => //s.Id != sku.Id &&
                                          s.AttributeOptionSKUs.Any(aos =>
                                              aos.AttributeOption.Attribute.Name == "Color" &&
                                              aos.AttributeOption.Value == sku.AttributeOptionSKUs.FirstOrDefault(ao => ao.AttributeOption.Attribute.Name == "Color")?
                                                  .AttributeOption.Value) &&
                                          s.AttributeOptionSKUs.Any(aos => aos.AttributeOption.Attribute.Name == "Dial Size"))
                                .Select(s => new RelatedSizeVM
                                {
                                    SkuId = s.Id,
                                    Size = s.AttributeOptionSKUs.FirstOrDefault(aos => aos.AttributeOption.Attribute.Name == "Dial Size")?
                                        .AttributeOption.Value
                                }).ToList(),
                Mechanism = product.SKUs.Where(s => //s.Id != sku.Id &&
                                          s.AttributeOptionSKUs.Any(aos =>
                                              aos.AttributeOption.Attribute.Name == "Color" &&
                                              aos.AttributeOption.Value == sku.AttributeOptionSKUs.FirstOrDefault(ao => ao.AttributeOption.Attribute.Name == "Color")?
                                                  .AttributeOption.Value) &&
                                          s.AttributeOptionSKUs.Any(aos => aos.AttributeOption.Attribute.Name == "Mechanism"))
                                .Select(s => new RelatedSizeVM
                                {
                                    SkuId = s.Id,
                                    Size = s.AttributeOptionSKUs.FirstOrDefault(aos => aos.AttributeOption.Attribute.Name == "Mechanism")?
                                        .AttributeOption.Value
                                }).ToList(),
                ProductReview = pr.Select(s => new ProductReviewVM
                {
                    ReviewDescription = s.ReviewDescription,
                    Rating = s.Rating,
                    CustomerName = s.User.UserName,
                    ReviewDate = s.ReviewDate
                }).ToList(),
                RelatedSku = product.SKUs.Select(s => new RelatedSkuVM
                {
                    SkuId = s.Id,
                    Color = s.AttributeOptionSKUs.FirstOrDefault(aos => aos.AttributeOption.Attribute.Name == "Color")?
                                .AttributeOption.Color,
                    /*Size = s.AttributeOptionSKUs.FirstOrDefault(aos => aos.AttributeOption.Attribute.Name == "Size")?
                                .AttributeOption.Value*/
                }).DistinctBy(r => r.Color).OrderBy(s => sku.Id).ToList()

            }).FirstOrDefault();

            if (!model.ImageUrls.Any())
            {
                model.ImageUrls.Add(product.MainImage);
            }

            ViewBag.CurrencySymbol = _companyService.GetCurrencySymbol();
            return View(model);
        }

        public async Task<IActionResult> ProductDetails(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            ViewBag.CurrencySymbol = _companyService.GetCurrencySymbol();
            // Get wishlist from cookies
            return View("_ProductDetail", product);
        }

        public async Task<IActionResult> Wishlist()
        {
            if (!User.Identity.IsAuthenticated) return Ok();

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var products = await _userWishList.GetUserSavedWishList(user.Id);
            List<Product> allproducts = await _productService.GetAllWithSkusAsync();

            var result = _productService.GetProductSkuListVM(allproducts);
            var hashproducts = new HashSet<long>(products.Select(x => x.SkuId).ToList());
            var wishlistProducts = result.Where(p => hashproducts.Contains(p.SkuId)).ToList();

            ViewBag.CurrencySymbol = _companyService.GetCurrencySymbol();

            return View(wishlistProducts);
        }

        public async Task<IActionResult> WishListProducts()
        {
            // Get wishlist from cookies
            if (!User.Identity.IsAuthenticated) return Ok();

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var products = await _userWishList.GetUserSavedWishList(user.Id);
            List<Product> allproducts = await _productService.GetAllWithSkusAsync();

            var result = _productService.GetProductSkuListVM(allproducts);
            var hashproducts = new HashSet<long>(products.Select(x => x.SkuId).ToList());
            var wishlistProducts = result.Where(p => hashproducts.Contains(p.SkuId)).ToList();

            ViewBag.CurrencySymbol = _companyService.GetCurrencySymbol();
            ViewBag.CurrencyRate = _companyService.GetCurrencyRate();
            // Code to add product to user's wish list
            return PartialView("_WistListProducts", wishlistProducts);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProductReview(ProductReview model)
        {
            if (!User.Identity.IsAuthenticated)
                return Ok();
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var reviews = await _productService.GetProductReviewById(user.Id, model.ProductId, model.SkuId);
            if (reviews != null && reviews.Count > 0)
            {
                TempData["ErrorMessage"] = "Product Review Already Submitted";
                return RedirectToAction("Index", "Product", new { id = model.SkuId });
            }
            var review = new ProductReview
            {
                UserId = user.Id,
                ProductId = model.ProductId,
                SkuId = model.SkuId,
                Rating = model.Rating,
                ReviewDescription = model.ReviewDescription,
                ReviewDate = DateTime.Now
            };
            var res = await _productService.AddProductReview(review);
            if (res)
            {
                TempData["SuccessMessage"] = "Product review added successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Error in Product review adding.";
            }
            return RedirectToAction("Index", "Product", new { id = model.SkuId });
        }

        public async Task<dynamic> SetCurrencyRate(string symbol)
        {
            await _companyService.SetCurrencyRate(symbol);
            return Json(true);
        }
    }
    public class VariationAttributeOptionVMComparer : IEqualityComparer<VariationAttributeOptionVM>
    {
        public bool Equals(VariationAttributeOptionVM x, VariationAttributeOptionVM y)
        {
            if (x == null || y == null)
                return false;

            // Compare based on AttributeOptionId
            return x.AttributeOptionId == y.AttributeOptionId;
        }

        public int GetHashCode(VariationAttributeOptionVM obj)
        {
            return obj.AttributeOptionId.GetHashCode();
        }
    }
}
