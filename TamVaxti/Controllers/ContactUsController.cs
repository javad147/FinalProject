using TamVaxti.Models;
using TamVaxti.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using TamVaxti.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace TamVaxti.Controllers
{
    public class ContactUsController : Controller
    {


        private readonly IContactUsService _contactUsService;
        private readonly ICompanyService _companyService;
        private readonly UserManager<AppUser> _userManager;

        public ContactUsController(IContactUsService contactUsService, ICompanyService companyService, UserManager<AppUser> userManager)
        {
            _contactUsService = contactUsService;
            _companyService = companyService;
            _userManager = userManager;
        }


        public async Task<IActionResult> Index()
        {
            var companyModel = await _companyService.GetFirstOrDefaultCompany();
            return View(companyModel);
        }


        [HttpPost]
        public async Task<IActionResult> SendMessage(Enquiry enquiry)
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                enquiry.FirstName =user.FullName;
                enquiry.EmailId = user.Email;
                enquiry.Phone = user.PhoneNumber;
            }
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "All fields are required.";
                return RedirectToAction("Index");
            }

            enquiry.CreatedOn = DateTime.UtcNow;

            _contactUsService.SaveEnquiry(enquiry);

            TempData["SuccessMessage"] = "Your message has been sent successfully!";
            return RedirectToAction("Index");
        }
    }
}
