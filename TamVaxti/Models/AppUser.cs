using Microsoft.AspNetCore.Identity;

namespace TamVaxti.Models
{
    public class AppUser : IdentityUser
    {
        public string? FullName { get; set; }
    }
}
