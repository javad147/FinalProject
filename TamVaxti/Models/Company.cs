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
        public string InstagramUrl { get; set; }
        public string GoogleUrl { get; set; }
        public string XUrl { get; set; }
        public string RssUrl { get; set; }
        public string FacebookUrl { get; set; }
        public string ContactMapUrl { get; set; }
        public string CurrencySymbol { get; set; }
    }
}
