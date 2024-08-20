using System.Linq;
using TamVaxti.Data; // Adjust namespace as necessary
using TamVaxti.Models;
using TamVaxti.Services.Interfaces;

namespace TamVaxti.Services
{
    public class NewsletterService : INewsletterService
    {
        private readonly AppDbContext _context;

        public NewsletterService(AppDbContext context)
        {
            _context = context;
        }

        public bool IsEmailSubscribed(string email)
        {
            return _context.NewsletterSubscriptions.Any(n => n.Email == email);
        }

        public void SubscribeEmail(string email)
        {
            var newSubscription = new NewsletterSubscriptions{ Email = email, IsActive = true };
            _context.NewsletterSubscriptions.Add(newSubscription);
            _context.SaveChanges();
        }
    }
}
