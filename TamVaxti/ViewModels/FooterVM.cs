using TamVaxti.Models;
using TamVaxti.ViewModels.Baskets;

namespace TamVaxti.ViewModels
{
    public class FooterVM
    {
        public TamVaxti.Models.Company Company { get; set; }
        public UserInfo UserInfo{ get; set; }
        public List<BasketVM> Cart { get; set; }
    }
}
