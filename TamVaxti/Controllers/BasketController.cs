using TamVaxti.Data;
using TamVaxti.Services.Interfaces;
using TamVaxti.ViewModels.Baskets;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace TamVaxti.Controllers;

public class BasketController : Controller
{
    private readonly IHttpContextAccessor _accessor;
    private readonly AppDbContext _context;
    private readonly IProductService _productService;

    public BasketController(AppDbContext context,
        IHttpContextAccessor accessor,
        IProductService productService)
    {
        _context = context;
        _accessor = accessor;
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var cartItems = await GetCartItemsFromCookies();
        return Ok(cartItems);
    }

    [HttpGet]
    public async Task<IActionResult> Cart()
    {
        var cartItems = await GetCartItemsFromCookies();
        return View(cartItems);
    }

    public async Task<IActionResult> CartPartial()
    {
        var cartItems = await GetCartItemsFromCookies();
        return PartialView("_CartItems", cartItems);
    }

    public async Task<IActionResult> SideCartPartial()
    {
        var cartItems = await GetCartItemsFromCookies();
        return PartialView("_SideCartPartial", cartItems);
    }

    private async Task<IEnumerable<BasketVM>> GetCartItemsFromCookies()
    {
        var basketProducts = new Dictionary<int, int>();

        if (_accessor.HttpContext.Request.Cookies["cart"] is not null)
            basketProducts =
                JsonConvert.DeserializeObject<Dictionary<int, int>>(_accessor.HttpContext.Request.Cookies["cart"]);

        var products = await _productService.GetAllWithImagesAsync();

        return (from basketProduct in basketProducts
            let product = products.FirstOrDefault(m => m.Id == basketProduct.Key)
            where product != null
            let subtotal = basketProduct.Value * product.Price
            select new BasketVM { Product = product, Quantity = basketProduct.Value, SubTotal = subtotal }).ToList();
    }
}