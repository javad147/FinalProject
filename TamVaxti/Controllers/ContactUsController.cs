using TamVaxti.Models;
using TamVaxti.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace TamVaxti.Controllers
{
    public class ContactUsController : Controller
    {


        private readonly IContactUsService _contactUsService;


        public ContactUsController(IContactUsService contactUsService)
        {
            _contactUsService = contactUsService;
        }


        public IActionResult Index()
        {
            return View();
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
