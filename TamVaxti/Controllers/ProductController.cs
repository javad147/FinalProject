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

        public ProductController(IProductService productService, UserManager<AppUser> userManager, IUserWishList userWishList, ICompanyService companyService)
        {
            _productService = productService;
            _userManager = userManager;
            _userWishList = userWishList;
            _companyService = companyService;
        }
        public async Task<IActionResult> Index(long? id)
        {
            if (id is null)
            {
                return BadRequest();
            }
            //Product product = await _productService.GetProductByIdAsync((int)id);
            Product product = await _productService.GetProductBySkuIdAsync((long)id);
            List<ProductReview> pr = await _productService.GetPublishedProductReviewByOfProduct(product.Id);
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
                Color = sku.AttributeOptionSKUs.FirstOrDefault(aos => aos.AttributeOption.Attribute.Name == "Color")?
                                                .AttributeOption.Value,
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
                                .AttributeOption.Value,
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
    }
}
