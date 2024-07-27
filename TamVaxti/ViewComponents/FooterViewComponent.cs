﻿using TamVaxti.Models;
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
    public FooterViewComponent(ISettingService settingService,
                               IHttpContextAccessor accessor,
                               UserManager<AppUser> userManager,
                               ICompanyService companyService, IProductService productService)
    {
        _settingService = settingService;
        _accessor = accessor;
        _userManager = userManager;
        _companyService = companyService;
        _productService = productService;
    }
    public async Task<IViewComponentResult> InvokeAsync()
    {

        AppUser user = new();

        if (User.Identity.IsAuthenticated)
        {
            user = await _userManager.FindByNameAsync(User.Identity.Name);
        }
        var companyModel = await _companyService.GetFirstOrDefaultCompany();
       

        var basketProducts = new Dictionary<int, int>();

        if (_accessor.HttpContext.Request.Cookies["cart"] is not null)
        {
            basketProducts = JsonConvert.DeserializeObject<Dictionary<int, int>>(_accessor.HttpContext.Request.Cookies["cart"]);
        }

        var products = await _productService.GetAllWithImagesAsync();
        var cart = basketProducts.Select(basketProduct =>
            new BasketVM { Product = products.FirstOrDefault(m => m.Id == basketProduct.Key), Quantity = basketProduct.Value, SubTotal = basketProduct.Value * products.FirstOrDefault(m => m.Id == basketProduct.Key)?.Price ?? 0 }).ToList();


        FooterVM response = new()
        {
            Company = companyModel,
            Cart = cart
        };


        return await Task.FromResult(View(response));
    }
}