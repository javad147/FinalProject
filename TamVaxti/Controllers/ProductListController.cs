using TamVaxti.Models;
using TamVaxti.Services;
using TamVaxti.Services.Interfaces;
using TamVaxti.ViewModels;
using TamVaxti.ViewModels.Products;
using Microsoft.AspNetCore.Mvc;

namespace TamVaxti.Controllers
{
    public class ProductListController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public ProductListController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index(int? categoryId)
        {
            List<CategoryFilterVM> categories = await _categoryService.GetAllAsFilterAsync();


            foreach (var category in categories)
            {
                if (categoryId is not null && categoryId == category.Id)
                {
                    category.IsSelected = true;
                }
            }

            List<Product> products = await _productService.GetAllWithImagesAsync();
            var model = new ProductListVM()
            {
                Categories = categories,
                Products = products
            };
            return View(model);
        }

        [HttpPost]
      //  [ValidateAntiForgeryToken]
        public async Task<IActionResult> FilterProducts([FromBody] FilterProductsRequest request)
        {
            List<Product> products = await _productService.GetAllWithImagesAsync();

            if (request.SelectedCategoryIds != null && request.SelectedCategoryIds.Any())
            {
                products = products.Where(p => request.SelectedCategoryIds.Contains(p.CategoryId)).ToList();
            }

            if (request.PriceRange != null)
            {
                var priceRange = request.PriceRange.Split(';');
                var minPrice = decimal.Parse(priceRange[0]);
                var maxPrice = decimal.Parse(priceRange[1]);

                products = products.Where(p => p.Price >= minPrice && p.Price <= maxPrice).ToList();
            }

            return PartialView("_ProductListPartial", products);
        }

        public class FilterProductsRequest
        {
            public List<int> SelectedCategoryIds { get; set; }
            public string PriceRange { get; set; }
        }
    }
}
