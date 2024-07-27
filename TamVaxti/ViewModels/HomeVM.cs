using TamVaxti.Models;
using TamVaxti.ViewModels.Sliders;
using TamVaxti.ViewModels.SubCategory;

namespace TamVaxti.ViewModels
{
    public class HomeVM
    {
        public List<Category> Categories { get; set; }
        public List<Product> Products { get; set; }
        public About AboutParts { get; set; }
        public List<Expert> Experts { get; set; }   
        public List<Position> Positions { get; set; }
        public List<Blog> Blogs { get; set; }
        public List<SliderVM> InstaSliders { get; set; }

        public List<SubCategoryVM> SubCategories { get; set; }
    }
}
