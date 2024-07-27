
// Models/NewsletterSubscription.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace TamVaxti.Models
{
    public class NewsletterSubscriptions
    {
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}