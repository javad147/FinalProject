using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TamVaxti.Models;
using TamVaxti.Services.Interfaces;
using TamVaxti.ViewModels;

public class AdminFooterViewComponent : ViewComponent
{
    private readonly IHttpContextAccessor _accessor;
    private readonly UserManager<AppUser> _userManager;
    private readonly ICompanyService _companyService;
    public AdminFooterViewComponent(IHttpContextAccessor accessor,
                               UserManager<AppUser> userManager,
                               ICompanyService companyService)
    {
        _accessor = accessor;
        _userManager = userManager;
        _companyService = companyService;
    }
    public async Task<IViewComponentResult> InvokeAsync()
    {

        AppUser user = new();

        if (User.Identity.IsAuthenticated)
        {
            user = await _userManager.FindByNameAsync(User.Identity.Name);
        }
        var companyModel = await _companyService.GetFirstOrDefaultCompany();
        FooterVM response = new()
        {
           Company = companyModel
        };


        return await Task.FromResult(View(response));
    }
}