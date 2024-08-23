using TamVaxti.Data;
using TamVaxti.Services.Interfaces;
using TamVaxti.ViewModels.Baskets;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TamVaxti.Models;

namespace TamVaxti.Controllers;

public class BasketController : Controller
{
    private readonly IHttpContextAccessor _accessor;
    private readonly AppDbContext _context;
    private readonly IProductService _productService;
    private readonly ICompanyService _companyService;

    public BasketController(AppDbContext context,
        IHttpContextAccessor accessor,
        IProductService productService,
        ICompanyService companyService)
    {
        _context = context;
        _accessor = accessor;
        _productService = productService;
        _companyService = companyService;
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
        ViewBag.CurrencySymbol = _companyService.GetCurrencySymbol();
        return View(cartItems);
    }

    public async Task<IActionResult> CartPartial()
    {
        var cartItems = await GetCartItemsFromCookies();
        ViewBag.CurrencySymbol = _companyService.GetCurrencySymbol();
        return PartialView("_CartItems", cartItems);
    }

    public async Task<IActionResult> SideCartPartial()
    {
        var cartItems = await GetCartItemsFromCookies();
        ViewBag.CurrencySymbol = _companyService.GetCurrencySymbol();
        return PartialView("_SideCartPartial", cartItems);
    }

    private async Task<IEnumerable<BasketVM>> GetCartItemsFromCookies()
    {
        var basketProducts = new Dictionary<long, int>();

        if (_accessor.HttpContext.Request.Cookies["cart"] is not null)
            basketProducts =
                JsonConvert.DeserializeObject<Dictionary<long, int>>(_accessor.HttpContext.Request.Cookies["cart"]);

        List<Product> products = await _productService.GetAllWithSkusAsync();
        var result = _productService.GetProductSkuListVM(products);

        return (from basketProduct in basketProducts
            let product = result.FirstOrDefault(m => m.SkuId == basketProduct.Key)
            where product != null
            let subtotal = basketProduct.Value * (product.SalePrice > 0 ? product.SalePrice : product.Price)
            select new BasketVM { Product = product, Quantity = basketProduct.Value, SubTotal = subtotal }).ToList();
    }
}