using TamVaxti.Models;
using TamVaxti.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using TamVaxti.Services;

namespace TamVaxti.Controllers
{
    public class ContactUsController : Controller
    {


        private readonly IContactUsService _contactUsService;
        private readonly ICompanyService _companyService;


        public ContactUsController(IContactUsService contactUsService, ICompanyService companyService)
        {
            _contactUsService = contactUsService;
            _companyService = companyService;
        }


        public async Task<IActionResult> Index()
        {
            var companyModel = await _companyService.GetFirstOrDefaultCompany();
            return View(companyModel);
        }


        [HttpPost]
        public IActionResult SendMessage(Enquiry enquiry)
        {
           
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
