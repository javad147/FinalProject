using TamVaxti.Data;
using TamVaxti.Models;
using TamVaxti.Services;
using TamVaxti.Services.Interfaces;
using TamVaxti.ViewModels;
using TamVaxti.ViewModels.Baskets;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace TamVaxti.ViewComponents
{
    public class HeaderViewComponent : ViewComponent
    {
        private readonly ISettingService _settingService;
        private readonly IHttpContextAccessor _accessor;
        private readonly UserManager<AppUser> _userManager;
        private readonly ICategoryService _categoryService;
        private readonly ICompanyService _companyService;
        public HeaderViewComponent(ISettingService settingService,
                                   IHttpContextAccessor accessor,
                                   UserManager<AppUser> userManager, ICategoryService categoryService,ICompanyService companyService)
        {
            _settingService = settingService;
            _accessor = accessor;
            _userManager = userManager;
            _categoryService = categoryService;
            _companyService = companyService;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            AppUser user = new();

            if (User.Identity.IsAuthenticated)
            {
                user = await _userManager.FindByNameAsync(User.Identity.Name);
            }

            Dictionary<string, string> settingDatas = await _settingService.GetAllAsync();

            List<BasketVM> basketProducts = new();

            if (_accessor.HttpContext.Request.Cookies["basket"] is not null)
            {
                basketProducts = JsonConvert.DeserializeObject<List<BasketVM>>(_accessor.HttpContext.Request.Cookies["basket"]);
            }

            var companyModel = await _companyService.GetFirstOrDefaultCompany();
            HeaderVM response = new()
            {
                Settings = settingDatas,
                BasketCount = basketProducts.Sum(m => m.Quantity),
                //BasketTotalPrice = basketProducts.Sum(m => m.Quantity * m.Price),
                UserFullName = user?.FullName,
                Categories = await _categoryService.GetAllAsync(),
                Company = companyModel
            };
            ViewBag.CurrencyList = await _companyService.GetCurrencyList();

            return await Task.FromResult(View(response));
        }
    }
}
