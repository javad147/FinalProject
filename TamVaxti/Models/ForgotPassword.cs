using System.ComponentModel.DataAnnotations;
namespace TamVaxti.Models
{
    public class ResetPassword
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

    }
    public class ConfirmResetPassword
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        public string Token { get; set; }
        [Required]
        public string NewPassword { get; set; }
    }
    public class TokenRequest
    {
        public string Token { get; set; }
    }
    public class ForgotPasswordEmail
    {
        public string Name { get; set; }
        public string Banner { get; set; }
        public string Link { get; set; }
        public Company Company { get; set; }
        public dynamic Icons { get; set; }

    }
}
