using TamVaxti.Models;
using TamVaxti.Services;
using TamVaxti.Services.Interfaces;
using TamVaxti.ViewModels;
using TamVaxti.ViewModels.Products;
using Microsoft.AspNetCore.Mvc;
using TamVaxti.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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

        public async Task<IActionResult> Index(int? categoryId, string sortOrder, int? pageNumber)
        {
            int pageSize = 10;
            int pageIndex = pageNumber ?? 1;

            ViewData["CurrentSort"] = sortOrder;
            ViewData["CurrentPageSize"] = pageSize;

            List<CategoryFilterVM> categories = await _categoryService.GetAllAsFilterAsync();

            foreach (var category in categories)
            {
                if (categoryId is not null && categoryId == category.Id)
                {
                    category.IsSelected = true;
                }
            }

            List<Product> productList = await _productService.GetAllWithSkusAsync();
            var products = _productService.GetProductSkuListVM(productList);

            if (categoryId is not null)
            {
                products = products.Where(p => p.CategoryId == categoryId).ToList();
            }

            var result = _productService.GetProductSkuListVMPaginated(products, pageIndex, pageSize, sortOrder);

            var model = new ProductListVM()
            {
                Categories = categories,
                Products = result
            };
            return View(model);
        }

        [HttpPost]
      //  [ValidateAntiForgeryToken]
        public async Task<IActionResult> FilterProducts([FromBody] FilterProductsRequest request)
        {
            int pageSize = request.pageSize ?? 10;
            int pageIndex = request.pageNumber ?? 1;

            ViewData["CurrentSort"] = request.sortOrder;
            ViewData["CurrentPageSize"] = pageSize;

            List<Product> productList = await _productService.GetAllWithSkusAsync();
            var products = _productService.GetProductSkuListVM(productList);

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

            var result = _productService.GetProductSkuListVMPaginated(products, pageIndex, pageSize, request.sortOrder);

            return PartialView("_ProductListPartial", result);
        }

        public class FilterProductsRequest
        {
            public List<int> SelectedCategoryIds { get; set; }
            public string PriceRange { get; set; }
            public string? sortOrder { get; set; }
            public int? pageSize { get; set; }
            public int? pageNumber { get; set; }
        }
    }
}
