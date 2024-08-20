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
        var cart = basketProducts.Select(basketProduct =>
            new BasketVM { Product = result.FirstOrDefault(m => m.SkuId == basketProduct.Key), Quantity = basketProduct.Value, SubTotal = basketProduct.Value * result.FirstOrDefault(m => m.SkuId == basketProduct.Key)?.Price ?? 0 }).ToList();


        FooterVM response = new()
        {
            Company = companyModel,
            Cart = cart
        };

        bool isSubscribed = _newsletterService.IsEmailSubscribed(user.Email);

        ViewBag.CurrencySymbol = _companyService.GetCurrencySymbol();
        ViewBag.IsSubscribed = isSubscribed;
        return await Task.FromResult(View(response));
    }
}