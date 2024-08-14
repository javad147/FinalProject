
using TamVaxti.Data;
using TamVaxti.Models;
using TamVaxti.Services;
using TamVaxti.Services.Interfaces;
using TamVaxti.ViewModels.Baskets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace TamVaxti.Controllers
{
    [Authorize]
    public class WishListController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IUserWishList _userWishList;
        private readonly IHttpContextAccessor _accessor;
        readonly UserManager<AppUser> _userManager;
        private readonly IProductService _productService;
        private readonly ICompanyService _companyService;

        public WishListController(AppDbContext context,
                                IHttpContextAccessor accessor,
                                IUserWishList userWishList, UserManager<AppUser> userManager, IProductService productService, ICompanyService companyService)
        {
            _context = context;
            _accessor = accessor;
            _userWishList = userWishList;
            _userManager = userManager;
            _productService = productService;
            _companyService = companyService;
        }
        public async Task<IActionResult> Index()
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



        [HttpGet]
        public async Task<IActionResult> AddToWishList(long productId)
        {
            if (!User.Identity.IsAuthenticated) return Ok();

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var userWishList = new UserWishList
            {
                Id = 0,
                UserId = user.Id,
                SkuId = productId
            };

            if (await _userWishList.WishListExists(userWishList) == false)
                await _userWishList.AddToWishList(userWishList);

            var products = await _userWishList.GetUserSavedWishListProducts(user.Id);

            return Ok(products);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserSavedWishList()
        {
            if (!User.Identity.IsAuthenticated) return Ok();

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var products = await _userWishList.GetUserSavedWishListProducts(user.Id);
            // Code to add product to user's wish list
            return Ok(products);
        }

        [HttpGet]
        public async Task<IActionResult> GetWishListProducts()
        {
            if (!User.Identity.IsAuthenticated) return Ok();

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var products = await _userWishList.GetUserSavedWishList(user.Id);
            List<Product> allproducts = await _productService.GetAllWithSkusAsync();

            var result = _productService.GetProductSkuListVM(allproducts);
            var hashproducts = new HashSet<long>(products.Select(x => x.SkuId).ToList());
            var wishlistProducts = result.Where(p => hashproducts.Contains(p.SkuId)).ToList();
            ViewBag.CurrencySymbol = _companyService.GetCurrencySymbol();

            // Code to add product to user's wish list
            return View("_WistListProducts", wishlistProducts);
        }

        [HttpGet]
        public async Task<IActionResult> RemoveProductFromWishList(long productId)
        {
            if (!User.Identity.IsAuthenticated) return Ok();

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var userWishList = new UserWishList
            {
                Id = 0,
                UserId = user.Id,
                SkuId = productId
            };
            var isExists = await _userWishList.WishListExists(userWishList);
            if (isExists)
            {
                await _userWishList.RemoveProductFromWishList(userWishList);
            }
            var products = await _userWishList.GetUserSavedWishListProducts(user.Id);
            // Code to add product to user's wish list
            return Ok(products);
        }
    }
}
