using Microsoft.AspNetCore.Mvc;
using TamVaxti.Services.Interfaces;
using TamVaxti.Models;
using Microsoft.AspNetCore.Identity;
using TamVaxti.Helpers;
using TamVaxti.Services;

namespace TamVaxti.Controllers
{
    public class NewsletterController : Controller
    {
        private readonly INewsletterService _newsletterService;
        readonly UserManager<AppUser> _userManager;
        private readonly ICompanyService _companyService;
        public NewsletterController(INewsletterService newsletterService, UserManager<AppUser> userManager, ICompanyService companyService)
        {
            _newsletterService = newsletterService;
            _userManager = userManager;
            _companyService = companyService;
            
        }

        [HttpPost]
        public async Task<IActionResult> Subscribe(string? email)
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                email = user.Email;
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                TempData["ErrorMessage"] = "Email is required.";
                return RedirectToAction("Index", "Home");
            }

            // Check if the email is already subscribed
            bool isSubscribed = _newsletterService.IsEmailSubscribed(email);
            if (isSubscribed)
            {
                TempData["ErrorMessage"] = "This email is already subscribed.";
                return RedirectToAction("Index", "Home");
            }

            // Add new subscription
            _newsletterService.SubscribeEmail(email);

            var company = await _companyService.GetFirstOrDefaultCompany();

            var verifyemail = new
            {
                Name = "Subscriber",
                Link = "",
                Company = company
            };
            string emailBody = GetEmailBodyFromFile($"{Directory.GetCurrentDirectory()}/wwwroot/emailtemplate/subscribe.html", verifyemail);

            await EmailHelper.SendEmailAsync(email, "Dear Subscriber", "Thanks for Joining Our Newsletter", emailBody);


            TempData["SuccessMessage"] = "Thank you for subscribing!";
            return RedirectToAction("Index", "Home");
        }

        public string GetEmailBodyFromFile(string filePath, dynamic request)
        {
            string emailBody = System.IO.File.ReadAllText(filePath);
            emailBody = emailBody.Replace("{{Name}}", request.Name);
            emailBody = emailBody.Replace("{{Link}}", request.Link);
            emailBody = emailBody.Replace("{{fbLink}}", request.Company.FacebookUrl);
            emailBody = emailBody.Replace("{{insLink}}", request.Company.InstagramUrl);
            emailBody = emailBody.Replace("{{twLink}}", request.Company.XUrl);
            emailBody = emailBody.Replace("{{gpLink}}", request.Company.GoogleUrl);
            return emailBody;
        }
    }
}
