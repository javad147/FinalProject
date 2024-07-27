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

        public ProductController(IProductService productService, UserManager<AppUser> userManager, IUserWishList userWishList)
        {
            _productService = productService;
            _userManager = userManager;
            _userWishList = userWishList;
        }
        public async Task<IActionResult> Index(int? id)
        {
            if(id is null)
            {
                return BadRequest();
            }
            Product product = await _productService.GetProductByIdAsync((int)id);

            if(product is null)
            {
                return NotFound();  
            }
            return View(product);
        }

        public async Task<IActionResult> ProductDetails(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            // Get wishlist from cookies
            return View("_ProductDetail", product);
        }

        public async Task<IActionResult> Wishlist()
        {
            if (!User.Identity.IsAuthenticated) return Ok();

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var products = await _userWishList.GetUserSavedWishList(user.Id);
            var wishlistProducts = (await _productService.GetAllWithImagesAsync()).Where(p => products.Select(x => x.ProductId).Contains(p.Id)).ToList();

            return View(wishlistProducts);
        }

        public async Task<IActionResult> WishListProducts()
        {
            // Get wishlist from cookies
            if (!User.Identity.IsAuthenticated) return Ok();

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var products = await _userWishList.GetUserSavedWishList(user.Id);
            var wishlistProducts = (await _productService.GetAllWithImagesAsync()).Where(p => products.Select(x => x.ProductId).Contains(p.Id)).ToList();

            // Code to add product to user's wish list
            return PartialView("_WistListProducts", wishlistProducts);
        }
    }
}
