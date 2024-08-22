using Microsoft.AspNetCore.Identity;

namespace TamVaxti.Models
{
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; }
        public string ProfileImageUrl { get; set; }
        public ICollection<Order> Orders { get; set; }

        public bool SoftDeleted { get; set; }
    }
}
