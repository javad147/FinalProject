using System.ComponentModel.DataAnnotations;

namespace TamVaxti.ViewModels.Company
{
    public class CompanyVM
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        public string LogoUrl { get; set; }

        [Required(ErrorMessage = "Address Line 1 is required")]
        public string AddressLineOne { get; set; }
        public string AddressLineTwo { get; set; }
        public string Fax { get; set; }

        [Required(ErrorMessage = "Contact Number is required")]
        [RegularExpression(@"^\+?(\d[\d-. ]+)?(\([\d-. ]+\))?[\d-. ]+\d$", ErrorMessage = "Please Enter a valid mobile number.")]
        public string ContactNumber { get; set; }
        public string AlternateNumber { get; set; }
        public string SupportEmailId { get; set; }

        [Required(ErrorMessage = "Email Id is required")]
        [EmailAddress(ErrorMessage = "Please Enter a valid email address.")]
        public string EmailId { get; set; }
        public bool SoftDeleted { get; set; }

        public IFormFile ImageFile { get; set; } // For file upload
    }
}
