using TamVaxti.Models;
using TamVaxti.Services.Interfaces;
using TamVaxti.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

public class AdminHeaderViewComponent : ViewComponent
{
    private readonly ISettingService _settingService;
    private readonly IHttpContextAccessor _accessor;
    private readonly UserManager<AppUser> _userManager;
    private readonly ICompanyService _companyService;
    public AdminHeaderViewComponent(ISettingService settingService,
                               IHttpContextAccessor accessor,
                               UserManager<AppUser> userManager,
                               ICompanyService companyService)
    {
        _settingService = settingService;
        _accessor = accessor;
        _userManager = userManager;
        _companyService = companyService;
    }
    public async Task<IViewComponentResult> InvokeAsync()
    {

        AppUser user = new();
        UserInfo UserInfo = new UserInfo();

        if (User.Identity.IsAuthenticated)
        {
            user = await _userManager.FindByNameAsync(User.Identity.Name);
            UserInfo.Name = user.FullName;
            UserInfo.ProfileImageURL = user.ProfileImageUrl;
            if (User.IsInRole("SuperAdmin"))
            {
                UserInfo.Role = "Super Administrator";
            }
            if (User.IsInRole("Admin"))
            {
                UserInfo.Role = "Administrator";
            }
        }
        var companyModel = await _companyService.GetFirstOrDefaultCompany();
        FooterVM response = new()
        {
           Company = companyModel,
           UserInfo = UserInfo
        };


        return await Task.FromResult(View(response));
    }
}