using TamVaxti.Models;
using TamVaxti.Services;
using TamVaxti.Services.Interfaces;
using TamVaxti.ViewModels;
using TamVaxti.ViewModels.Baskets;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;

public class FooterViewComponent : ViewComponent
{
    private readonly ISettingService _settingService;
    private readonly IHttpContextAccessor _accessor;
    private readonly UserManager<AppUser> _userManager;
    private readonly ICompanyService _companyService;
    private readonly IProductService _productService;
    private readonly INewsletterService _newsletterService;
    public FooterViewComponent(ISettingService settingService,
                               IHttpContextAccessor accessor,
                               UserManager<AppUser> userManager,
                               ICompanyService companyService, IProductService productService, 
                               INewsletterService newsletterService)
    {
        _settingService = settingService;
        _accessor = accessor;
        _userManager = userManager;
        _companyService = companyService;
        _productService = productService;
        _newsletterService = newsletterService;
    }
    public async Task<IViewComponentResult> InvokeAsync()
    {

        AppUser user = new();

        if (User.Identity.IsAuthenticated)
        {
            user = await _userManager.FindByNameAsync(User.Identity.Name);
        }
        var companyModel = await _companyService.GetFirstOrDefaultCompany();
       

        var basketProducts = new Dictionary<long, int>();

        if (_accessor.HttpContext.Request.Cookies["cart"] is not null)
        {
            basketProducts = JsonConvert.DeserializeObject<Dictionary<long, int>>(_accessor.HttpContext.Request.Cookies["cart"]);
        }

        List<Product> products = await _productService.GetAllWithSkusAsync();
        var result = _productService.GetProductSkuListVM(products);
        var cart = (from basketProduct in basketProducts
                let product = result.FirstOrDefault(m => m.SkuId == basketProduct.Key)
                where product != null
                let subtotal = basketProduct.Value * (product.SalePrice > 0 ? product.SalePrice : product.Price)
                select new BasketVM { Product = product, Quantity = basketProduct.Value, SubTotal = subtotal }).ToList();

        FooterVM response = new()
        {
            Company = companyModel,
            Cart = cart
        };

        bool isSubscribed = false;
        try
        {
            isSubscribed = _newsletterService.IsEmailSubscribed(user.Email);
        }
        catch(Exception)
        {
            isSubscribed = false;
        }
      

        ViewBag.CurrencySymbol = _companyService.GetCurrencySymbol();
        ViewBag.CurrencyRate = _companyService.GetCurrencyRate();
        ViewBag.IsSubscribed = isSubscribed;
        return await Task.FromResult(View(response));
    }
}