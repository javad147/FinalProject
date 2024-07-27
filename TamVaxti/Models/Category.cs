using TamVaxti.ViewModels.SubCategory;
using System.ComponentModel.DataAnnotations;

namespace TamVaxti.Models
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }

        public string CategoryImage { get; set; }

        public bool IsPublished { get; set; }

        public bool ShowInMenu { get; set; }
        public bool ShowOnCategoryHomePage { get; set; }

        public bool ShowOnTrendingHomePage { get; set; }

        public ICollection<SubCategory> Subcategories { get; set; }
    }
}
