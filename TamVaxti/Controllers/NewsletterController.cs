using Microsoft.AspNetCore.Mvc;
using TamVaxti.Services.Interfaces;
using TamVaxti.Models;

namespace TamVaxti.Controllers
{
    public class NewsletterController : Controller
    {
        private readonly INewsletterService _newsletterService;

        public NewsletterController(INewsletterService newsletterService)
        {
            _newsletterService = newsletterService;
        }

        [HttpPost]
        public IActionResult Subscribe(string email)
        {
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
