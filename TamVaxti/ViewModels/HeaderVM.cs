using TamVaxti.Models;

namespace TamVaxti.ViewModels
{
    public class HeaderVM
    {
        public Dictionary<string, string> Settings { get; set; }
        public int BasketCount { get; set; }
        public decimal BasketTotalPrice { get; set; }
        public string UserFullName { get; set; }

        public List<Category> Categories { get; set; }
        public TamVaxti.Models.Company Company { get; set; }
    }
}
