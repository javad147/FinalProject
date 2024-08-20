using Microsoft.AspNetCore.Mvc;
using TamVaxti.Services.Interfaces;
using TamVaxti.Models;
using Microsoft.AspNetCore.Identity;

namespace TamVaxti.Controllers
{
    public class NewsletterController : Controller
    {
        private readonly INewsletterService _newsletterService;
        readonly UserManager<AppUser> _userManager;

        public NewsletterController(INewsletterService newsletterService, UserManager<AppUser> userManager)
        {
            _newsletterService = newsletterService;
            _userManager = userManager;
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

            TempData["SuccessMessage"] = "Thank you for subscribing!";
            return RedirectToAction("Index", "Home");
        }
    }
}
