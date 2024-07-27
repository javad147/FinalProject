namespace TamVaxti.Models
{
    public class Company:BaseEntity
    {
        public string Name { get; set; }
        public string LogoUrl { get; set; }
        public string AddressLineOne { get; set; }
        public string AddressLineTwo { get; set; }
        public string Fax { get; set; }
        public string ContactNumber { get; set; }
        public string AlternateNumber { get; set; }
        public string SupportEmailId { get; set; }
        public string EmailId { get; set; }
    }
}
